using Denali.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Algorithms.AggregateAnalysis.SMA
{
    public class SMA
    {
        private readonly int _backlog;
        public IList<decimal> MovingAverages { get; set; }

        public SMA(int backlog)
        {
            _backlog = backlog;
            MovingAverages = new List<decimal>();
        }

        public void Analyze(IEnumerable<IAggregateData> data)
        {
            var length = data.Count() - 1;
            if (length < _backlog - 1)
                return;

            var limit = length - (_backlog - 1);

            var sum = 0M;
            for (int i = length; i >= limit; i--)
            {
                sum += data.ElementAt(i).ClosePrice;
            }

            MovingAverages.Add(sum / _backlog);
        }
    }
}
