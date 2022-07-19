using Alpaca.Markets;
using AutoMapper;
using Denali.Models;
using Denali.Services;
using Denali.TechnicalAnalysis;
using Denali.TechnicalAnalysis.ElephantBars;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Denali.Processors.ElephantStrategy
{
    public class ElephantRideStrategyAnalysis
    {
        private readonly AlpacaService _alpacaService;
        private readonly ElephantBarSettings _settings;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ElephantRideStrategyAnalysis(AlpacaService alpacaService, IOptions<ElephantBarSettings> settings, IMapper mapper, ILogger<ElephantRideStrategyAnalysis> logger)
        {
            _alpacaService = alpacaService;
            _mapper = mapper;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task Process(CancellationToken stoppingToken, DateTime day)
        {
            int backLogDayLength = 2;
            _alpacaService.InitializeTradingclient();
            _alpacaService.InitializeDataClient();

            // Check the last 5 days (to account for weekends and holidys)
            var lastMarketDates = await GeOpenMarketDays(5, day);
            // Use the previous two days for market data, as well ass the current day for any data already present
            var bracketDates = lastMarketDates.Take(3);
            var currentDay = lastMarketDates.First();
            var backlogDay1 = bracketDates.Skip(1).First();
            var backlogDay2 = bracketDates.Skip(2).First();

            var backlog1 = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                new HistoricalBarsRequest("AAPL", backlogDay1.GetTradingOpenTimeUtc(), backlogDay1.GetTradingCloseTimeUtc(), new BarTimeFrame(5, BarTimeFrameUnit.Minute)));

            var backlog2 = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                new HistoricalBarsRequest("AAPL", backlogDay2.GetTradingOpenTimeUtc(), backlogDay2.GetTradingCloseTimeUtc(), new BarTimeFrame(5, BarTimeFrameUnit.Minute)));

            var alpacaCurrentData = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                new HistoricalBarsRequest("AAPL", currentDay.GetTradingOpenTimeUtc(), currentDay.GetTradingCloseTimeUtc(), new BarTimeFrame(5, BarTimeFrameUnit.Minute)));

            var alpacaBackLog = backlog2.Items.ToList();
            alpacaBackLog.AddRange(backlog1.Items.ToList());

            var backlogData = _mapper.Map<List<AggregateBar>>(alpacaBackLog);
            var currentData = _mapper.Map<List<AggregateBar>>(alpacaCurrentData.Items);

            var elephantBars = new ElephantBars(_settings);
            var sma3 = new SimpleMovingAverage(3);
            var sma8 = new SimpleMovingAverage(8);
            var sma21 = new SimpleMovingAverage(21);
            sma3.Analyze(backlogData);
            sma8.Analyze(backlogData);
            sma21.Analyze(backlogData);
            elephantBars.Initialize(backlogData);

            var movingData = new List<AggregateBar>();
            movingData.AddRange(backlogData);

            int start = 0;
            int count = currentData.Count - 1;

            // Start analysis at start of day (not including premarket bars)
            decimal total = 0.0m;
            for (int i = start; i < count; i++)
            {
                var currentBar = currentData.ElementAt(i);
                movingData.Add(currentBar);

                elephantBars.Analyze(movingData);
                sma3.Analyze(movingData);
                sma8.Analyze(movingData);
                sma21.Analyze(movingData);

                _logger.LogInformation($"Time: {currentBar.TimeUtc},OHLC: ({currentBar.Open},{currentBar.High},{currentBar.Low},{currentBar.Close}), SMAS: ({sma3.MovingAverages.Last()},{sma8.MovingAverages.Last()},{sma21.MovingAverages.Last()}), Trigger: {currentBar.Open + elephantBars.Trigger}");

            }
        }

        private async Task<IEnumerable<IIntervalCalendar>> GeOpenMarketDays(int pastDays, DateTime day)
        {
            var calenders = await _alpacaService.AlpacaTradingClient.ListIntervalCalendarAsync((new CalendarRequest().SetInclusiveTimeInterval(day.AddDays(-pastDays), day)));
            return calenders.OrderByDescending(x => x.GetTradingDate());
        }

    }
}
