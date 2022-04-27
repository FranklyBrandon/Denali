using Alpaca.Markets;
using Denali.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.TechnicalAnalysis.ElephantBars
{
    public class ElephantBars
    {
        private readonly ElephantBarSettings _settings;
        private readonly AverageRange _averageRange;
        private bool _isElephant;

        public ElephantBars(ElephantBarSettings settings)
        {
            settings = _settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void Analyze(IEnumerable<IAggregateBar> bars)
        {
            _averageRange.Analyze(bars);
            _isElephant = IsElephantBar(bars.Last(), _averageRange.AverageRanges.Last());
        }

        private bool IsElephantBar(IAggregateBar bar, BarRange average)
        {
            return bar.BodyRange() >= (average.BodyRange * _settings.OverAverageThreshold)
                && (bar.TotalRange() / bar.BodyRange()) >= _settings.BodyPercentageThreshold;
        }
    }
}
