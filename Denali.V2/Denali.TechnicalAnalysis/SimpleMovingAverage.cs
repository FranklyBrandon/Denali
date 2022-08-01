namespace Denali.TechnicalAnalysis
{
    public class SimpleMovingAverage
    {
        private readonly int _backlog;
        private readonly IList<decimal> _rawValues;
        public IList<decimal> MovingAverages { get; set; }

        public SimpleMovingAverage(int backlog)
        {
            _backlog = backlog;
            _rawValues = new List<decimal>();
            MovingAverages = new List<decimal>();
        }

        public void Analyze(decimal value)
        {
            _rawValues.Add(value);
            var length = _rawValues.Count() - 1;
            if (length < _backlog - 1)
                return;

            MovingAverages.Add(CalculateMovingAverageValue(length));
        }

        private decimal CalculateMovingAverageValue(int length)
        {
            var limit = length - (_backlog - 1);
            var sum = 0M;
            for (int i = length; i >= limit; i--)
            {
                sum += _rawValues.ElementAt(i);
            }

            return sum / _backlog;
        }
    }
}
