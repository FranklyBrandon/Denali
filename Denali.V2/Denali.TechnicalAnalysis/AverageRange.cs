using Alpaca.Markets;
using Denali.Shared;

namespace Denali.TechnicalAnalysis
{
    /// <summary>
    /// TA for finding the moving average range of a bar list. 
    /// Note: This is NOT the 'average true range' (ATR) indicator but rather the literal average range of the bars.
    /// </summary>
    public class AverageRange
    {
        public List<BarRange> AverageRanges;

        private readonly int _backlog;

        public AverageRange(int backlog)
        {
            this._backlog = backlog;
            this.AverageRanges = new List<BarRange>();
        }

        public AverageRange(int backlog, IEnumerable<IBar> bars)
        {
            this._backlog = backlog;
            this.AverageRanges = new List<BarRange>();
            Analyze(bars);
        }

        public void Analyze(IEnumerable<IBar> bars)
        {
            var length = bars.Count() - 1;
            if (length < _backlog - 1)
                return;

            var limit = length - (_backlog - 1);

            var totalRange = 0M;
            var bodyRange = 0M;
            for (int i = length; i >= limit; i--)
            {
                var bar = bars.ElementAt(i);
                totalRange += (bar.High - bar.Low);
                bodyRange += (Math.Abs(bar.Open - bar.Low));
            }

            AverageRanges.Add(
                new BarRange((bodyRange / _backlog).RoundToMoney(), (totalRange / _backlog).RoundToMoney())
            );
        }
    }

    public struct BarRange
    {
        public decimal BodyRange;
        public decimal TotalRange;

        public BarRange(decimal bodyRange, decimal totalRange)
        {
            this.BodyRange = bodyRange;
            this.TotalRange = totalRange;
        }
    }
}
