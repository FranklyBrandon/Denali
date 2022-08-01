using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.TechnicalAnalysis
{
    public class SimpleMovingAverageDouble
    {
        private readonly int _backlog;
        private readonly IList<double> _rawValues;
        public IList<double> MovingAverages { get; set; }

        public SimpleMovingAverageDouble(int backlog)
        {
            _backlog = backlog;
            _rawValues = new List<double>();
            MovingAverages = new List<double>();
        }

        public void Analyze(double value)
        {
            _rawValues.Add(value);
            var length = _rawValues.Count() - 1;
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
                sum += _rawValues.ElementAt(i);
            }

            return sum / _backlog;
        }
    }
}
