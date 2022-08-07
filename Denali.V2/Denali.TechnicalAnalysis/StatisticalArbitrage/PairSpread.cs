using Denali.Models;
using Denali.Services.PythonInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.TechnicalAnalysis.StatisticalArbitrage
{
    public class PairReturnsCalculation
    {
        private readonly IPythonInteropClient _pythonInteropClient;
        private readonly int _backlog;

        public PairReturnsCalculation(int backlog, IPythonInteropClient pythonInteropClient)
        {
            _pythonInteropClient = pythonInteropClient;
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

                currentA.Returns = Returns(originalA.Close, currentA.Close);
                currentB.Returns = Returns(originalB.Close, currentB.Close);
            }
        }

        public void AnalyzeStep(AggregateBar originalA, AggregateBar currentA, AggregateBar originalB, AggregateBar currentB)
        {
            currentA.Returns = Returns(originalA.Close, currentA.Close);
            currentB.Returns = Returns(originalB.Close, currentB.Close);
        }

        public void CalculateStats(IEnumerable<AggregateBar> tickerXData, IEnumerable<AggregateBar> tickerYData)
        {
            var result = _pythonInteropClient.GetOLSCalculation(tickerXData.Select(x => x.Returns), tickerYData.Select(x => x.Returns));
        }

        private double Returns(decimal originalValue, decimal newValue) =>
            (double)((originalValue / newValue) - 1) * 100;

    }
}
