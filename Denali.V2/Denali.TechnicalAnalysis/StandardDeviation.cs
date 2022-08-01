using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.TechnicalAnalysis
{
    public class StandardDeviation
    {
        public double CalculateStandardDeviation(IEnumerable<double> series, double mean, int backlog)
        {
            var length = series.Count() - 1;
            var limit = length - (backlog - 1);
            var sum = 0d;
            for (int i = length; i >= limit; i--)
            {
                sum += Math.Pow((double)mean - series.ElementAt(i), 2);
            }

            return Math.Sqrt(sum / backlog);
        }
    }
}
