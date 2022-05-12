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
        public IList<DateTime> Elephants { get; }
        private readonly ElephantBarSettings _settings;
        private readonly AverageRange _averageRange;
        private bool _isElephant;

        public ElephantBars(ElephantBarSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _averageRange = new AverageRange(_settings.RangeAveragesBacklog);
            Elephants = new List<DateTime>();
        }

        public void Initialize(IEnumerable<IAggregateBar> bars)
        {
            _averageRange.Analyze(bars);
        }

        public void Analyze(IEnumerable<IAggregateBar> bars)
        {
            _averageRange.Analyze(bars);
            var lastBar = bars.Last();

            if (IsElephantBar(lastBar, _averageRange.AverageRanges.Last()))
            {
                _isElephant = true;
                Elephants.Add(lastBar.TimeUtc);
            }
            else
                _isElephant = false;
        }

        public bool IsLatestElephant() => _isElephant;

        private bool IsElephantBar(IAggregateBar bar, BarRange average)
        {
            //TODO: Use body percentage to weed out false elephant bars
            return  (bar.BodyRange() >= average.AverageBodyRange * _settings.OverAverageThreshold);
        }


    }
}
