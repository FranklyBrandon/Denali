using Denali.Models.Polygon;
using Denali.Models.Shared;
using Denali.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Denali.Algorithms.BarAnalysis.ParabolicSAR
{
    /// <summary>
    /// Encapsalates the logic and tracking of the Parabolic SAR (stop and reversals) values of a ticker in real time.
    /// </summary>
    public class ParabolicSAR
    {
        /// <summary>
        /// The list of segments for this analyzation of the aggregate stock data. Segments represent the long and short trends across time.
        /// </summary>
        public IList<SARSegment> SARSegments;

        public ParabolicSAR()
        {
            SARSegments = new List<SARSegment>();
        }

        /// <summary>
        /// Calculate the parabolic SAR values for a given group of aggregate stock data. To calculate the parabolic SAR values over a time frame, continually call this method with the entirety of the aggregate stock data whenever a new period is added.
        /// </summary>
        /// <param name="barData"></param>
        public void Analyze(IList<Bar> barData)
        {
            //If no intial SAR value is given, and there are less than two bars, SAR caclulation cannot begin.
            if (barData.Count < 2 && SARSegments.Count == 0)
            {
                return;
            }
            //If no initial SAR value is given, approximate an initial SAR value to use as the previous SAR value in further calculations.
            else if (barData.Count == 2 && SARSegments.Count == 0)
            {
                //Determine initial segment trend by the close prices of the first two bars
                var trend = (barData[0].ClosePrice < barData[1].ClosePrice) ? MarketSide.Bullish : MarketSide.Bearish;

                var maxPrice = Math.Max(barData[0].HighPrice, barData[1].HighPrice);
                var minPrice = Math.Min(barData[0].LowPrice, barData[1].LowPrice);

                //If trending up, the first SAR would be the lowest price of the last short segment, if trending down, the first SAR would be the highest price of the previous long segment.
                var firstSar = trend == MarketSide.Bullish ? minPrice : maxPrice;

                //The initial extreme point is the highest high of an uptrend, or the lowest low of a downtrend.
                var extremePoint = trend == MarketSide.Bullish ? maxPrice : minPrice;

                //Create the first segment and initial SAR value.
                var segment = new SARSegment(extremePoint, barData[0].Time, trend);
                var firstSAR = new SAR(firstSar, barData[1].Time);

                segment.SARs.Add(firstSAR);
                SARSegments.Add(segment);
            }
            else
            {
                var currentSegment = SARSegments.Last();
                var priorSAR = currentSegment.SARs.Last().Value;
                var currentBar = barData.Last();

                var newSar = CalculateSARValue(priorSAR, currentSegment.ExtremePoint, currentSegment.AccelerationFactor, currentSegment.Trend);

                SARSegment relevantSegment;

                //If the trend has been reversed, start a new segment.
                if (IsTrendReversing(newSar, currentBar))
                {
                    relevantSegment = SignalReversal(currentSegment, currentBar);
                    SARSegments.Add(relevantSegment);
                }
                //Otherwise add the new SAR value to the existing segment.
                else
                {
                    relevantSegment = currentSegment;
                    newSar = ValidateSARMove(newSar, barData, relevantSegment.Trend);
                    relevantSegment.SARs.Add(new SAR(newSar, currentBar.Time));
                }

                //Update the extreme point and acceleration values if necessary.
                relevantSegment.UpdateExtremePoint(currentBar);
            }
        }

        public bool IsTrendBeginning(MarketSide trend)
        {
            var segment = SARSegments.LastOrDefault();
            if (segment == null)
                return false;

            if (segment.SARs.Count <= 1 && segment.Trend == trend)
                return true;  

            return false;
        }
         
        public void SetInitialSegment(SARSegment segment)
        {
            if (SARSegments.Any())
                throw new InvalidOperationException("There are previously calculated segments");

            SARSegments.Add(segment);
        }
        private decimal CalculateSARValue(decimal priorSAR, decimal extremePoint, decimal accelerationFactor, MarketSide trend)
        {
            decimal newSARValue;
            if (trend == MarketSide.Bullish)
            {
                newSARValue = priorSAR + accelerationFactor * (extremePoint - priorSAR);
            }
            else
            {
                newSARValue = priorSAR - accelerationFactor * (priorSAR - extremePoint);
            }

            return Math.Round(newSARValue, 2, MidpointRounding.AwayFromZero);
        }
    
        private bool IsTrendReversing(decimal sarValue, Bar currentBar)
        {
            if (sarValue >= currentBar.LowPrice && sarValue <= currentBar.HighPrice)
                return true;

            return false;
        }

        private SARSegment SignalReversal(SARSegment currentSegment, Bar currentBar)
        {
            var newTrend = currentSegment.Trend == MarketSide.Bullish ? MarketSide.Bearish : MarketSide.Bullish;
            var firstSAR = new SAR(currentSegment.ExtremePoint, currentBar.Time);
            var firstExtremePoint = newTrend == MarketSide.Bullish ? currentBar.HighPrice : currentBar.LowPrice;

            var segment = new SARSegment(firstExtremePoint, currentBar.Time, newTrend);
            segment.SARs.Add(firstSAR);

            return segment;
        }

        private decimal ValidateSARMove(decimal calculatedSar, IList<Bar> barData, MarketSide trend)
        {
            var length = barData.Count - 1;

            var tMinusOne = barData.ElementAtOrDefault(length - 1);
            var tMinusTwo = barData.ElementAtOrDefault(length - 2);

            if (tMinusOne == null || tMinusTwo == null)
                return calculatedSar;

            if (trend == MarketSide.Bullish)
            {
                if (calculatedSar > tMinusOne.LowPrice || calculatedSar > tMinusTwo.LowPrice)
                    return Math.Min(tMinusOne.LowPrice, tMinusTwo.LowPrice);
            }
            else
            {
                if (calculatedSar < tMinusOne.HighPrice || calculatedSar < tMinusTwo.HighPrice)
                    return Math.Max(tMinusOne.HighPrice, tMinusTwo.HighPrice);
            }

            return calculatedSar;
        }
    }
}
