﻿using Denali.Models.Polygon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Algorithms.BarAnalysis.ParabolicSAR
{
    public class SARSegment
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

        public void UpdateExtremePoint(Bar currentBar)
        {
            if (Trend == Trend.UpTrend)
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

        public void IncrementAccelerationFactor()
        {
            if (AccelerationFactor != SARSegment.MaxMomuntum)
                AccelerationFactor += SARSegment.Acceleration;
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
}