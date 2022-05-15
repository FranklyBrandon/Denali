using Denali.Models;
using Denali.Shared;
using Denali.TechnicalAnalysis;
using Denali.TechnicalAnalysis.ElephantBars;
using Denali.Shared.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors.ElephantStrategy
{
    public class ElephantStrategy
    {
        private readonly ElephantStrategySettings _settings;
        public ElephantBars ElephantBars;
        public List<DateTime> Retracements;
        public List<DateTime> Continuations;

        public ElephantStrategy(ElephantStrategySettings settings)
        {
            this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.ElephantBars = new ElephantBars(_settings.ElephantBarSettings);
            this.Retracements = new();
            this.Continuations = new();
        }

        public void Initialize(List<IAggregateBar> barData)
        {
            this.ElephantBars.Initialize(barData);
        }

        public void ProcessTick(List<IAggregateBar> barData)
        {
            ElephantBars.Analyze(barData);

            //Retracement
            if (ElephantBars.Elephants.Contains(barData.GetHistoricValue(1).TimeUtc) && !ElephantBars.Elephants.Contains(barData.Last().TimeUtc))
            {
                Retracements.Add(barData.Last().TimeUtc);
            }

        }
    }
}

/*
 - The size of the candle is determined by comparing a range of previous candles (you can set the amount at your discretion)
- Search factor: by default 1.3, this means that all bars that have a range greater than the average range of previous candles + 30%, are considered elephant candles (can be configured at your discretion)
- Possibility to configure the percentage of the body that the elephant candle must have.
- Possibility of filtering up to 2 means with direction detection and color change (fully configurable)
- Possibility of filtering by mobile averages

https://www.tradingview.com/script/yV11ifDS-Elephant-Bar-by-Oliver-Velez/
 */
