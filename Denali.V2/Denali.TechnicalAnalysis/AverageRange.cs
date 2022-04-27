using Alpaca.Markets;
using Denali.Models;
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

        public AverageRange(int backlog, IEnumerable<IAggregateBar> bars)
        {
            this._backlog = backlog;
            this.AverageRanges = new List<BarRange>();
            Analyze(bars);
        }

        public void Analyze(IEnumerable<IAggregateBar> bars)
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

                currentTotalRange = bar.TotalRange();
                totalRange += currentTotalRange;
                currentBodyRange = bar.BodyRange();
                totalBodyRange += currentBodyRange;
            }

            AverageRanges.Add(
                new BarRange(currentBodyRange, currentTotalRange, (totalBodyRange / _backlog).RoundToMoney(), (totalRange / _backlog).RoundToMoney())
            );
        }
    }

    public record BarRange(decimal BodyRange, decimal TotalRange, decimal AverageBodyRange, decimal AverageTotalRange);
}
