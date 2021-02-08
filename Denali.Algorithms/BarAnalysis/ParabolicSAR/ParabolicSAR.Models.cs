using Denali.Models.Polygon;
using Denali.Models.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Algorithms.BarAnalysis.ParabolicSAR
{
    public class SARSegment
    {
        public const decimal Acceleration = 0.02M;
        public const decimal MaxMomuntum = 0.2M; 

        public decimal AccelerationFactor { get; set; }
        public decimal ExtremePoint { get; set; }
        public IList<SAR> SARs { get; set; }
        public long Begin { get; set; }
        public long End { get; set; }
        public MarketSide Trend { get; set; }

        public SARSegment(decimal extremePoint, long begin, MarketSide trend)
        {
            ExtremePoint = extremePoint;
            Begin = begin;
            End = 0;
            SARs = new List<SAR>();
            Trend = trend;
            AccelerationFactor = Acceleration;
        }

        public void UpdateExtremePoint(IAggregateData currentBar)
        {
            if (Trend == MarketSide.Bullish)
            {
                if (currentBar.HighPrice > ExtremePoint)
                {
                    ExtremePoint = currentBar.HighPrice;
                    IncrementAccelerationFactor();
                }
            }
            else
            {
                if (currentBar.LowPrice < ExtremePoint)
                {
                    ExtremePoint = currentBar.LowPrice;
                    IncrementAccelerationFactor();
                }
            }
        }

        private void IncrementAccelerationFactor()
        {
            if (AccelerationFactor != SARSegment.MaxMomuntum)
                AccelerationFactor = Math.Round(AccelerationFactor + SARSegment.Acceleration, 2, MidpointRounding.AwayFromZero);
        }
    }

    public struct SAR
    {
        public decimal Value { get; set; }
        public long Time { get; set; }

        public SAR(decimal value, long time)
        {
            Value = value;
            Time = time;
        }
    }
}
