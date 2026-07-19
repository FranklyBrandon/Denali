using Alpaca.Markets;
using AutoMapper;
using Denali.Models;
using Denali.Services;
using Denali.TechnicalAnalysis.ElephantBars;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Denali.Processors
{
    public class ElephantBackLook(DataLayerComponent DataLayer, IMapper Mapper, ILogger<PreMarketHours> Logger)
    {
        public async Task Process(DateTime date)
        {
            await DataLayer.Initialize();
            var elephantBarSettings = new ElephantBarSettings
            {
                BodyPercentageThreshold = 0.01m,
                OverAverageThreshold = 1.3m,
                RangeAveragesBacklog = 200

            };
            var elephantBarTA = new ElephantBars(elephantBarSettings);

            var marketBacklogDays = await DataLayer.GetPastMarketDays(date, 4);
            var startTime = marketBacklogDays.SkipLast(1).Last().GetTradingOpenTimeUtc();
            var endTime = marketBacklogDays.Last().GetTradingCloseTimeUtc();
            var data = (await DataLayer.GetAggregateDataMulti(new List<string> { "VTI" }, startTime, endTime, BarTimeFrame.Minute))["VTI"];

            List<AggregateBar> bars = new List<AggregateBar>();
            foreach (var item in data)
            {
                bars.Add(Mapper.Map<AggregateBar>(item));
                elephantBarTA.Analyze(bars);
            }

            var forwardDays = bars.Where(x => x.TimeUtc >= marketBacklogDays.Last().GetSessionOpenTimeUtc()).ToList();

            for ( var i = 0; i < forwardDays.Count; i++ )
            {
                var bar = forwardDays[i];
                if (elephantBarTA.Elephants.Contains(bar.TimeUtc))
                {
                    Logger.LogInformation($"Elephant Bar! {bar.TimeUtc.ToShortTimeString}");
                }

            }
        }
    }
}
