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
        public HashSet<DateTime> Elephants { get; }
        public readonly AverageRange AverageRange;
        private readonly ElephantBarSettings _settings;
        private bool _isElephant;

        public ElephantBars(ElephantBarSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            AverageRange = new AverageRange(_settings.RangeAveragesBacklog);
            Elephants = new();
        }

        public void Initialize(IEnumerable<IAggregateBar> bars)
        {
            AverageRange.Analyze(bars);
        }

        public void Analyze(IEnumerable<IAggregateBar> bars)
        {
            AverageRange.Analyze(bars);
            var lastBar = bars.Last();

            if (IsElephantBar(lastBar, AverageRange.AverageRanges.Last()))
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
            return  (bar.BodyRange() >= average.AverageBodyRange * _settings.OverAverageThreshold) 
                 && (bar.BodyRange() / bar.TotalRange() > _settings.BodyPercentageThreshold);
        }


    }
}
