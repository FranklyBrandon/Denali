using Denali.Models.Polygon;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Denali.Algorithms.BarAnalysis.ParabolicSAR
{
    /// <summary>
    /// Encapsalates the logic and tracking of the Parabolic SAR (stop and reversals) values of a ticker in real time.
    /// </summary>
    public class ParabolicSARAlgo
    {
        /// <summary>
        /// The list of segments for this analyzation of the aggregate stock data. Segments represent the long and short trends across time.
        /// </summary>
        public IList<SARSegment> SARSegments;

        public ParabolicSARAlgo()
        {
            SARSegments = new List<SARSegment>();
        }

        /// <summary>
        /// Calculate the parabolic SAR values for a given group of aggregate stock data. To calculate the parabolic SAR values over a time frame, continually call this method with the entirety of the aggregate stock data whenever a new period is added.
        /// </summary>
        /// <param name="barData"></param>
        public void Analyze(IList<Bar> barData)
        {
            //There needs to be at least two bars to calculate SAR from scratch.
            if (barData.Count < 2)
            {
                return;
            }
            //Calculate initial SAR
            //If the trend is long, use the low of the first bar as the initial value
            //If the trend is short, use the high of the first bar as the initial value
            else if (barData.Count == 2)
            {
                //Determine initial segment trend by the close prices of the first two bars
                var trend = (barData[0].ClosePrice < barData[1].ClosePrice) ? Trend.UpTrend : Trend.DownTrend;
                var extremePoint = trend == Trend.UpTrend ? barData[1].HighPrice : barData[1].LowPrice;
                var segment = new SARSegment(extremePoint, barData[0].Time, trend);

                //The first SAR is the same as the extreme point between the first two bars
                var firstSAR = new SAR(extremePoint, barData[1].Time);
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

                //If the trend has been reversed, start a new segment with the 1a rules
                if (IsTrendReversing(newSar, currentBar))
                {
                    relevantSegment = SignalReversal(currentSegment, currentBar);
                    SARSegments.Add(relevantSegment);
                }
                //Add the new SAR to the existing segment
                else
                {
                    relevantSegment = currentSegment;
                    newSar = ValidateSARMove(newSar, barData, relevantSegment.Trend);
                    relevantSegment.SARs.Add(new SAR(newSar, currentBar.Time));
                }

                relevantSegment.UpdateExtremePoint(currentBar);
            }
        }

        private double CalculateSARValue(double priorSAR, double extremePoint, double accelerationFactor, Trend trend)
        {
            var acceleratedExtreme = accelerationFactor * (extremePoint - priorSAR);

            return trend == Trend.UpTrend ? 
                priorSAR + acceleratedExtreme :
                priorSAR - acceleratedExtreme;
        }
    
        private bool IsTrendReversing(double sarValue, Bar currentBar)
        {
            if (sarValue > currentBar.LowPrice && sarValue < currentBar.HighPrice)
                return true;

            return false;
        }

        private SARSegment SignalReversal(SARSegment currentSegment, Bar currentBar)
        {
            var newTrend = currentSegment.Trend == Trend.UpTrend ? Trend.DownTrend : Trend.UpTrend;
            var firstSAR = new SAR(currentSegment.ExtremePoint, currentBar.Time);
            var firstExtremePoint = newTrend == Trend.UpTrend ? currentBar.HighPrice : currentBar.LowPrice;

            var segment = new SARSegment(firstExtremePoint, currentBar.Time, newTrend);
            segment.SARs.Add(firstSAR);

            return segment;
        }

        private double ValidateSARMove(double calculatedSar, IList<Bar> barData, Trend trend)
        {
            var length = barData.Count - 1;

            var tMinusOne = barData.ElementAtOrDefault(length - 1);
            var tMinusTwo = barData.ElementAtOrDefault(length - 2);

            if (tMinusOne == null || tMinusTwo == null)
                return calculatedSar;

            if (trend == Trend.UpTrend)
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
