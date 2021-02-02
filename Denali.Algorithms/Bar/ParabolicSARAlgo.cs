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
                var segment = SARSegments.Last();
                var priorSAR = segment.SARs.Last().Value;
                var acceleratedExtreme = segment.AccelerationFactor * (segment.ExtremePoint - priorSAR);

                if (segment.Trend == Trend.UpTrend)
                {
                    var currentSAR = priorSAR + acceleratedExtreme;
                }
                else
                {
                    var currentSAR = priorSAR - acceleratedExtreme;
                }
            }



        }
    }
}
