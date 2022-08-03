using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.TechnicalAnalysis
{
    public class SimpleMovingAverageDouble
    {
        public IList<double> RawValues { get; }
        private readonly int _backlog;

        public IList<double> MovingAverages { get; set; }

        public SimpleMovingAverageDouble(int backlog)
        {
            _backlog = backlog;
            RawValues = new List<double>();
            MovingAverages = new List<double>();
        }

        public void Analyze(double value)
        {
            RawValues.Add(value);
            var length = RawValues.Count() - 1;
            if (length < _backlog - 1)
                return;

            MovingAverages.Add(CalculateMovingAverageValue(length));
        }

        private double CalculateMovingAverageValue(int length)
        {
            var limit = length - (_backlog - 1);
            var sum = 0d;
            for (int i = length; i >= limit; i--)
            {
                sum += RawValues.ElementAt(i);
            }

            return sum / _backlog;
        }
    }
}
