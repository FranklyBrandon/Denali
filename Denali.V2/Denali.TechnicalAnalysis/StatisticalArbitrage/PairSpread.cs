﻿using Denali.Models;
using Denali.Services.PythonInterop;
using Denali.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.TechnicalAnalysis.StatisticalArbitrage
{
    public class PairReturnsCalculation
    {

        private readonly int _backlog;

        public PairReturnsCalculation(int backlog)
        {
            _backlog = backlog;
        }

        public void Initialize(IEnumerable<AggregateBar> tickerXData, IEnumerable<AggregateBar> tickerYData)
        {
            var length = tickerXData.Count() - 1;
            if (length < _backlog - 1)
                return;

            // Skip the first bar, because spread is calculated using the previous bar
            int start = 1;

            for (int i = start; i < length; i++)
            {
                var originalA = tickerXData.ElementAt(i - 1);
                var currentA = tickerXData.ElementAt(i);
                var originalB = tickerYData.ElementAt(i - 1);
                var currentB = tickerYData.ElementAt(i);

                // TODO: In this scenario we should get historic quote price
                if (!originalA.TimeUtc.Equals(originalB.TimeUtc))
                {

                }

                currentA.Returns = Returns(originalA.Close, currentA.Close).Round(8);
                currentB.Returns = Returns(originalB.Close, currentB.Close).Round(8);
            }
        }

        public void AnalyzeStep(AggregateBar originalA, AggregateBar currentA, AggregateBar originalB, AggregateBar currentB)
        {
            currentA.Returns = Returns(originalA.Close, currentA.Close).Round(8);
            currentB.Returns = Returns(originalB.Close, currentB.Close).Round(8);
        }

        // https://quant.stackexchange.com/questions/54062/correct-calculation-of-returns-from-a-pairs-trade
        private double Returns(decimal originalValue, decimal newValue) =>
            (double)((originalValue / newValue) - 1) * 100;

    }
}
