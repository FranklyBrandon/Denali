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

        public ElephantBars(ElephantBarSettings settings)
        {
            settings = _settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void Analyze(IEnumerable<IAggregateBar> bars)
        {
            _averageRange.Analyze(bars);
        }
    }
}
