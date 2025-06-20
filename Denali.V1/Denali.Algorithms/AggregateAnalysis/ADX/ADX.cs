﻿using Denali.Algorithms.AggregateAnalysis.TR;
using Denali.Algorithms.AggregateAnalysis.Utilities;
using Denali.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Denali.Algorithms.AggregateAnalysis.ADX
{
    public class ADX
    {
        private readonly TrueRange _trueRange;
        private int _backlog;

        public IList<ADXResult> InitialADXResults { get; set; }
        public IList<ADXResult> ADXResults { get; set; }

        public ADX()
        {
            _trueRange = new TrueRange();
            ADXResults = new List<ADXResult>();
        }

        /// <summary>
        /// Initiate the ADX calculation by giving a lookback period of history
        /// </summary>
        /// <param name="history"></param>
        public void Initiate(IEnumerable<IAggregateData> history, int backlog)
        {
            _backlog = backlog;
            InitialADXResults = new List<ADXResult>();

            var length = history.Count();

            //Start at index one because there is no previous value to use
            for (int i = 0; i < length; i++)
            {
                ADXResult result;
 
                var previous = history.ElementAtOrDefault(i - 1);
                if (previous == null)
                    continue;

                var current = history.ElementAtOrDefault(i);

                var tr = _trueRange.Analyze(previous, current);
                var dms = CalculateDirectionalMovements(previous, current);

                result = new ADXResult
                {
                    DMPlus = dms.Item1,
                    DMMinus = dms.Item2,
                    TrueRange = tr,
                    Time = current.Time
                };

                if (i <= backlog - 1)
                {
                    InitialADXResults.Add(result);
                    continue;
                }

                ADXResult previousResult = default;
                //First day after backlog
                if (i == backlog)
                {
                    //Use sums for the first day of smoothing
                    var trueRangeSum = InitialADXResults.Sum(x => x.TrueRange);
                    var dmpSum = InitialADXResults.Sum(x => x.DMPlus);
                    var dmmSum = InitialADXResults.Sum(x => x.DMMinus);

                    result.SmoothedTrueRange = GetSmoothedValue(trueRangeSum, result.TrueRange, backlog);
                    result.SmoothedDMPlus = GetSmoothedValue(dmpSum, result.DMPlus, backlog);
                    result.SmoothedDMMinus = GetSmoothedValue(dmmSum, result.DMMinus, backlog);
                }
                //Smoothing period
                else
                {
                    //Use previous for smoothing
                    previousResult = InitialADXResults.Last();
                    result.SmoothedTrueRange = GetSmoothedValue(previousResult.SmoothedTrueRange, result.TrueRange, backlog);
                    result.SmoothedDMPlus = GetSmoothedValue(previousResult.SmoothedDMPlus, result.DMPlus, backlog);
                    result.SmoothedDMMinus = GetSmoothedValue(previousResult.SmoothedDMMinus, result.DMMinus, backlog);
                }

                //Calculate increments
                result.DIPlus = Math.Round(100 * result.SmoothedDMPlus / result.SmoothedTrueRange, 0, MidpointRounding.AwayFromZero);
                result.DIMinus = Math.Round(100 * result.SmoothedDMMinus / result.SmoothedTrueRange, 0, MidpointRounding.AwayFromZero);
                result.DX = Math.Round(100 * Math.Abs(result.DIPlus - result.DIMinus) / (result.DIPlus + result.DIMinus), 0, MidpointRounding.AwayFromZero);

                if (i == (2 * backlog) - 1)
                {
                    //Initial ADX
                    InitialADXResults.Add(result);
                    result.ADX = Math.Round(InitialADXResults.Sum(x => x.DX) / backlog, 0, MidpointRounding.AwayFromZero);
                }
                else if (i > (2 * backlog) - 1)
                {
                    //average DX
                    result.ADX = Math.Round((previousResult.ADX * (backlog - 1) + result.DX)/ backlog, 0, MidpointRounding.AwayFromZero);
                }

                InitialADXResults.Add(result);
            }
        }

        public void Analyze(IEnumerable<IAggregateData> barData)
        {
            var dataCount = barData.Count();
            var previousData = barData.ElementAt(dataCount - 1);
            var currentData = barData.Last();
            var previousADX = dataCount == 1 ? InitialADXResults.Last() : ADXResults.Last();


            var tr = _trueRange.Analyze(previousData, currentData);
            var dms = CalculateDirectionalMovements(previousData, currentData);

            var result = new ADXResult
            {
                DMPlus = dms.Item1,
                DMMinus = dms.Item2,
                TrueRange = tr,
                Time = currentData.Time
            };

            result.SmoothedTrueRange = GetSmoothedValue(previousADX.SmoothedTrueRange, result.TrueRange, _backlog);
            result.SmoothedDMPlus = GetSmoothedValue(previousADX.SmoothedDMPlus, result.DMPlus, _backlog);
            result.SmoothedDMMinus = GetSmoothedValue(previousADX.SmoothedDMMinus, result.DMMinus, _backlog);

            result.DIPlus = Math.Round(100 * result.SmoothedDMPlus / result.SmoothedTrueRange, 0, MidpointRounding.AwayFromZero);
            result.DIMinus = Math.Round(100 * result.SmoothedDMMinus / result.SmoothedTrueRange, 0, MidpointRounding.AwayFromZero);
            result.DX = Math.Round(100 * Math.Abs(result.DIPlus - result.DIMinus) / (result.DIPlus + result.DIMinus), 0, MidpointRounding.AwayFromZero);

            result.ADX = Math.Round((previousADX.ADX * (_backlog - 1) + result.DX) / _backlog, 0, MidpointRounding.AwayFromZero);

            ADXResults.Add(result);
        }

        public void AnalyzeAll(IEnumerable<IAggregateData> barData, int lookbackPeriod)
        {

        }

        private (decimal, decimal) CalculateDirectionalMovements(IAggregateData previous, IAggregateData current)
        {
            var highDifference = current.HighPrice - previous.HighPrice;
            var lowDifference = previous.LowPrice - current.LowPrice;

            var DMPlus = highDifference > lowDifference ?
                Math.Max(highDifference, 0) : 0;
            var DMMinus = lowDifference > highDifference ?
                Math.Max(lowDifference, 0) : 0;

            return (DMPlus, DMMinus);
        }

        private decimal GetSmoothedValue(decimal previousValue, decimal currentValue, int backlog)
        {
            return AlgorithmUtils.RoundMoney(previousValue - (previousValue / backlog) + currentValue);
        }
    }
}
