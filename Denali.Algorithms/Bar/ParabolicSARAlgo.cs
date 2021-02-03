using Denali.Models.Polygon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Denali.Algorithms.BarAnalysis
{
    public struct SARSegment
    {
        public const double Acceleration = 0.02;
        public const double MaxMomuntum = 0.2;

        public double AccelerationFactor { get; set; }
        public double ExtremePoint { get; set; }
        public IList<SAR> SARs { get; set; }
        public long Begin { get; set; }
        public long End { get; set; }
        public Trend Trend { get; set; }

        public SARSegment(double extremePoint, long begin, Trend trend)
        {
            ExtremePoint = extremePoint;
            Begin = begin;
            End = 0;
            SARs = new List<SAR>();
            Trend = trend;
            AccelerationFactor = Acceleration;
        }
    }

    public enum Trend
    {
        UpTrend,
        DownTrend
    }

    public struct SAR
    {
        public double Value { get; set; }
        public long Time { get; set; }

        public SAR(double value, long time)
        {
            Value = value;
            Time = time;
        }
    }

    public class ParabolicSARAlgo
    {
        Dictionary<long, double> SARMap;
        IList<SARSegment> SARSegments;

        public void Analyze(IList<Bar> barData)
        {
            //Need at least two bars to calculate SAR from scratch
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
                var trend = (barData[0].ClosePrice < barData[0].ClosePrice) ? Trend.UpTrend : Trend.DownTrend;
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

                //If the trend has been reversed, start a new segment 
                if (ReverseTrend(newSar, currentBar))
                {
                    relevantSegment = SignalReversal(currentSegment, currentBar);
                    SARSegments.Add(relevantSegment);
                }
                else
                {
                    relevantSegment = currentSegment;
                    newSar = ValidateSARMove(newSar, barData);
                    relevantSegment.SARs.Add(new SAR(newSar, currentBar.Time));
                }

                UpdateExtremePointAndExceleration(currentBar, relevantSegment);
            }
        }

        private double CalculateSARValue(double priorSAR, double extremePoint, double accelerationFactor, Trend trend)
        {
            var acceleratedExtreme = accelerationFactor * (extremePoint - priorSAR);

            return trend == Trend.UpTrend ? 
                priorSAR + acceleratedExtreme :
                priorSAR - acceleratedExtreme;
        }
    
        private bool ReverseTrend(double sarValue, Bar currentBar)
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

        private double ValidateSARMove(double calculatedSar, IList<Bar> barData)
        {
            //TODO
            return calculatedSar;
        }

        private void UpdateExtremePointAndExceleration(Bar currentBar, SARSegment segment)
        {
            //TODO: move this to segment object?
            if (segment.Trend == Trend.UpTrend)
            {
                if (currentBar.HighPrice > segment.ExtremePoint)
                {
                    segment.ExtremePoint = currentBar.HighPrice;
                    segment.AccelerationFactor = IncrementAccelerationFactor(segment.AccelerationFactor);
                }
            }
            else
            {
                if (currentBar.LowPrice < segment.ExtremePoint)
                {
                    segment.ExtremePoint = currentBar.LowPrice;
                    segment.AccelerationFactor = IncrementAccelerationFactor(segment.AccelerationFactor);
                }
            }
        }

        private double IncrementAccelerationFactor(double currentFactor)
        {
            //TODO: Move this to segment object?
            if (currentFactor != SARSegment.MaxMomuntum)
                currentFactor += SARSegment.Acceleration;

            return currentFactor;
        }
    }
}
