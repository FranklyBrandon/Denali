using Alpaca.Markets;
using Denali.Services;
using Denali.Shared.Time;
using Denali.TechnicalAnalysis;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Denali.Processors.PreMarketFadeStrategy
{
    public class PreMarketFadeLiveProcessor
    {
        private readonly DataLayerComponent _dataLayer;
        private readonly ILogger _logger;

        private ScheduledTask _screenScheduledTask;

        public PreMarketFadeLiveProcessor(DataLayerComponent dataLayer, ILogger<PreMarketGainers> logger)
        {
            _dataLayer = dataLayer;
            _logger = logger;
        }

        public async Task Process(DateTime date)
        {
            _logger.LogInformation($"{GetType().Name} starting for date {date.ToShortDateString()}");

            await _dataLayer.Initialize();

            var marketDay = (await _dataLayer.GetMarketDays(date)).FirstOrDefault();
            if (marketDay is null)
            {
                _logger.LogError($"No post market detected for {date.ToShortDateString()}");
                return;
            }

            var nextMarketDay = (await _dataLayer.GetMarketDays(date.AddDays(1))).FirstOrDefault();
            if (nextMarketDay is null)
            {
                _logger.LogError($"No pre market detected for {date.AddDays(1).ToShortDateString()}");
            }

            var postMarketStartUTC = marketDay.GetTradingCloseTimeUtc();
            var postMarketEndUTC = marketDay.GetSessionCloseTimeUtc();
            var scheduledScreenTime = postMarketEndUTC.AddHours(1);

            _logger.LogInformation($"{date.ToShortDateString()} ETH start at {postMarketStartUTC.ToShortTimeString()}, end at {postMarketEndUTC.ToShortTimeString()}");
            _logger.LogInformation($"Scheduled screen for {scheduledScreenTime}");

            _screenScheduledTask = new ScheduledTask(scheduledScreenTime, () => ScreenStocks(postMarketStartUTC, postMarketEndUTC));
        }
         
        public async Task ScreenStocks(DateTime startTime, DateTime endTime)
        {
            var allAssets = await _dataLayer.GetAllTradableAssets();
            var aggregateData = await _dataLayer.GetAggregateDataMulti(allAssets.Select(x => x.Symbol), startTime, endTime, BarTimeFrame.Minute);

            List<ScreenResult> results = new List<ScreenResult>();
            foreach (var data in aggregateData)
            {
                var postMarketData = data.Value;
                if (!postMarketData.Any())
                    continue;

                var asset = allAssets.FirstOrDefault(x => x.Symbol == data.Key);
                if (asset is null)
                    continue;

                var change = ChangePercentage.Calculate(postMarketData.First().Open, postMarketData.Last().Close);
                results.Add(new ScreenResult(asset, change));
            }
        }
    }

    public record ScreenResult(IAsset Asset, decimal ChangePercentage);
}
