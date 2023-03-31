using Alpaca.Markets;
using AutoMapper;
using Denali.Models;
using Denali.Processors.Exceptions;
using Denali.Services;
using Denali.Shared.Time;
using Denali.TechnicalAnalysis;
using Microsoft.Extensions.Logging;

namespace Denali.Processors.GapMomentum
{
    public class GapMomentumAnalysis : StrategyProcessorBase
    {
        private readonly ILogger _logger;

        public GapMomentumAnalysis(AlpacaService alpacaService,
            IMapper mapper, 
            ILogger<GapMomentumAnalysis> logger
        ) : base(alpacaService, mapper)
        {
            _logger = logger;
        }
        public async Task Process(string ticker, DateTime startDate, DateTime endDate, CancellationToken stoppingToken)
        {
            _alpacaService.InitializeTradingclient();
            _alpacaService.InitializeDataClient();

            var today = TimeUtils.GetNewYorkTime(DateTime.UtcNow);
            var marketDays = await GetOpenMarketDays(startDate, endDate);
            if (marketDays?.FirstOrDefault()?.GetTradingDate().Day != startDate.Day)
            {
                _logger.LogInformation($"No trading window detected for day {today.Day}");
                throw new NoTradingWindowException();
            }

            var aggregateBars = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                new HistoricalBarsRequest(
                    ticker,
                    marketDays.First().GetTradingOpenTimeUtc(),
                    marketDays.Last().GetTradingCloseTimeUtc(),
                    BarTimeFrame.Day
                )
            ).ConfigureAwait(false);

            const bool FULL_GAP = false;
            var gap = new Gap(FULL_GAP);

            for (int i = 1; i < aggregateBars.Items.Count(); i++)
            {
                var currentBar = _mapper.Map<AggregateBar>(aggregateBars.Items[i]);
                var previousBar = _mapper.Map<AggregateBar>(aggregateBars.Items[i - 1]);

                if (gap.IsGapUp(currentBar, previousBar))
                {
                    _logger.LogInformation(
                        $"Gap Up detected on {TimeUtils.GetNewYorkTime(currentBar.TimeUtc).ToString("MM-dd-yyyy")}");
                }

                if (gap.IsGapDown(currentBar, previousBar))
                {
                    _logger.LogInformation(
                        $"Gap Down detected on {TimeUtils.GetNewYorkTime(currentBar.TimeUtc).ToString("MM-dd-yyyy")}");
                }
            }
        }
    }
}
