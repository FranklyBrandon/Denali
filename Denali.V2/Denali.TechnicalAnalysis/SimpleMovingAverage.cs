using Alpaca.Markets;
using Denali.Models;
using Denali.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.TechnicalAnalysis
{
    public class SimpleMovingAverage
    {
        private readonly int _backlog;
        public IList<decimal> MovingAverages { get; set; }
        public decimal ProvisionalValue { get; private set; } = 0.0m;

        public SimpleMovingAverage(int backlog)
        {
            _backlog = backlog;
            MovingAverages = new List<decimal>();
        }

        public void Analyze(IEnumerable<IAggregateBar> data)
        {
            var length = data.Count() - 1;
            if (length < _backlog - 1)
                return;

            var limit = length - (_backlog - 1);

            MovingAverages.Add((GetMovingAverageValue(data, length, limit);
        }

        public void ProvisionalChange(ITrade trade, IEnumerable<IAggregateBar> data)
        {
            var provisionalClose = new AggregateBar { Close = trade.Price };
            data.Append(provisionalClose);

            var length = data.Count() - 1;
            if (length < _backlog - 1)
                return;

            var limit = length - (_backlog - 1);

            ProvisionalValue = GetMovingAverageValue(data, length, limit);
        }

        private decimal GetMovingAverageValue(IEnumerable<IAggregateBar> data, int length, int limit)
        {
            var sum = 0M;
            for (int i = length; i >= limit; i--)
            {
                sum += data.ElementAt(i).Close;
            }

            return (sum / _backlog).RoundToMoney();
        }
    }
}
