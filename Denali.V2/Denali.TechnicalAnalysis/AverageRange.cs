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

            var currentBodyRange = 0M;
            var currentTotalRange = 0M;
            var totalRange = 0M;
            var totalBodyRange = 0M;
            for (int i = length; i >= limit; i--)
            {
                var bar = bars.ElementAt(i);

                currentTotalRange = (bar.High - bar.Low);
                totalRange += currentTotalRange;
                currentBodyRange = (Math.Abs(bar.Open - bar.Close));
                totalBodyRange += currentBodyRange;
            }

            AverageRanges.Add(
                new BarRange(currentBodyRange, currentTotalRange, (totalBodyRange / _backlog).RoundToMoney(), (totalRange / _backlog).RoundToMoney())
            );
        }
    }

    public struct BarRange
    {
        public decimal BodyRange;
        public decimal TotalRange;
        public decimal AverageBodyRange;
        public decimal AverageTotalRange;

        public BarRange(decimal bodyRange, decimal totalRange, decimal averageBodyRange, decimal averageTotalRange)
        {
            this.BodyRange = bodyRange;
            this.TotalRange = totalRange;
            this.AverageBodyRange = averageBodyRange;
            this.AverageTotalRange = averageTotalRange;
        }
    }
}
