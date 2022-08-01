using Alpaca.Markets;
using AutoMapper;
using Denali.Models;
using Denali.Models.Alpaca;
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

        public async Task Process(CancellationToken stoppingToken, string ticker, DateTime startDate, DateTime endDate, BarTimeFrame barTimeFrame, int numberOfBacklogDays)
        {
            _alpacaService.InitializeTradingclient();
            _alpacaService.InitializeDataClient();

            var lastMarketDates = await GetOpenBacklogDays(numberOfBacklogDays + 3, startDate);

            var backlogDays = lastMarketDates.Skip(1).Take(numberOfBacklogDays).Reverse().Select(x => x);

            var backlogBars = new List<IBar>();
            foreach (var backlogDay in backlogDays)
            {
                var backlog = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                    new HistoricalBarsRequest(ticker, backlogDay.GetTradingOpenTimeUtc(), backlogDay.GetTradingCloseTimeUtc(), barTimeFrame));

                backlogBars.AddRange(backlog.Items);
            }

            var backlogData = _mapper.Map<List<AggregateBar>>(backlogBars);

            var averageRange = new AverageRange(100);
            var sma3 = new SimpleMovingAverageClose(3);
            var sma8 = new SimpleMovingAverageClose(8);
            var sma21 = new SimpleMovingAverageClose(21);

            averageRange.Analyze(backlogData);
            sma3.Analyze(backlogData);
            sma8.Analyze(backlogData);
            sma21.Analyze(backlogData);

            var rangeDays = (int)((endDate - startDate).TotalDays) + 1;
            var openMarketDays = await GetOpenMarketDays(startDate, endDate);
            for (int i = 0; i < rangeDays; i++)
            {
                var currentDayDate = startDate.AddDays(i);
                var currentDay = openMarketDays.FirstOrDefault(x => x.GetTradingDate() == DateOnly.FromDateTime(currentDayDate));

                if (currentDay != null)
                {
                    _logger.LogInformation($"Day: {currentDay.GetTradingDate()}");
                    var currentBars = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                            new HistoricalBarsRequest(ticker, currentDay.GetTradingOpenTimeUtc(), currentDay.GetTradingCloseTimeUtc(), barTimeFrame));
                    var currentData = _mapper.Map<List<AggregateBar>>(currentBars.Items);

                    int start = 0;
                    int count = currentData.Count - 1;
                    var movingData = backlogData;

                    decimal sum = 0.0m;
                    for (int j = start; j < count; j++)
                    {
                        var currentBar = currentData.ElementAt(j);
                        movingData.Add(currentBar);

                        averageRange.Analyze(movingData);
                        sma3.Analyze(movingData);
                        sma8.Analyze(movingData);
                        sma21.Analyze(movingData);

                        //_logger.LogInformation($"Time: {currentBar.TimeUtc},OHLC: ({currentBar.Open},{currentBar.High},{currentBar.Low},{currentBar.Close}), SMAS: ({sma3.MovingAverages.Last()},{sma8.MovingAverages.Last()},{sma21.MovingAverages.Last()}), Trigger: {currentBar.Open + elephantBars.Trigger}");
                        if (sma3.MovingAverages.Last() > sma8.MovingAverages.Last() && sma8.MovingAverages.Last() > sma21.MovingAverages.Last())
                        {
                            var threshold = currentBar.Open + (averageRange.AverageRanges.Last().AverageBodyRange * 1.3m);
                            if (currentBar.High >= threshold)
                            {
                                var nextBar = currentData.ElementAtOrDefault(j + 1);
                                var profit = nextBar?.Open - threshold;
                                _logger.LogInformation($"Trade at: {currentBar.TimeUtc.TimeOfDay}, Gain of: {profit}");
                                sum += profit.Value;
                            }
                        }
                        else if (sma21.MovingAverages.Last() > sma8.MovingAverages.Last() && sma8.MovingAverages.Last() > sma3.MovingAverages.Last())
                        {
                            var threshold = currentBar.Open - (averageRange.AverageRanges.Last().AverageBodyRange * 1.3m);
                            if (currentBar.Low <= threshold)
                            {
                                var nextBar = currentData.ElementAtOrDefault(j + 1);
                                var profit = threshold - nextBar?.Open;
                                _logger.LogInformation($"Trade at: {currentBar.TimeUtc.TimeOfDay}, Gain of: {profit}");
                                sum += profit.Value;
                            }
                        }
                    }

                    _logger.LogInformation($"Total Profit for Day: {sum}");
                }
            }

            //var backlog2 = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
            //    new HistoricalBarsRequest("AAPL", backlogDay2.GetTradingOpenTimeUtc(), backlogDay2.GetTradingCloseTimeUtc(), new BarTimeFrame(5, BarTimeFrameUnit.Minute)));

            //var alpacaCurrentData = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
            //    new HistoricalBarsRequest("AAPL", currentDay.GetTradingOpenTimeUtc(), currentDay.GetTradingCloseTimeUtc(), new BarTimeFrame(5, BarTimeFrameUnit.Minute)));

            //var alpacaBackLog = backlog2.Items.ToList();
            //alpacaBackLog.AddRange(backlog1.Items.ToList());


            //var currentData = _mapper.Map<List<AggregateBar>>(alpacaCurrentData.Items);

            //var elephantBars = new ElephantBars(_settings);
            //var sma3 = new SimpleMovingAverage(3);
            //var sma8 = new SimpleMovingAverage(8);
            //var sma21 = new SimpleMovingAverage(21);
            //sma3.Analyze(backlogData);
            //sma8.Analyze(backlogData);
            //sma21.Analyze(backlogData);
            //elephantBars.Initialize(backlogData);

            //var movingData = new List<AggregateBar>();
            //movingData.AddRange(backlogData);

            //int start = 0;
            //int count = currentData.Count - 1;

            //// Start analysis at start of day (not including premarket bars)
            //decimal total = 0.0m;
            //for (int i = start; i < count; i++)
            //{
            //    var currentBar = currentData.ElementAt(i);
            //    movingData.Add(currentBar);

            //    elephantBars.Analyze(movingData);
            //    sma3.Analyze(movingData);
            //    sma8.Analyze(movingData);
            //    sma21.Analyze(movingData);

            //    _logger.LogInformation($"Time: {currentBar.TimeUtc},OHLC: ({currentBar.Open},{currentBar.High},{currentBar.Low},{currentBar.Close}), SMAS: ({sma3.MovingAverages.Last()},{sma8.MovingAverages.Last()},{sma21.MovingAverages.Last()}), Trigger: {currentBar.Open + elephantBars.Trigger}");

            //}
        }

        private async Task<IEnumerable<IIntervalCalendar>> GetOpenBacklogDays(int pastDays, DateTime day)
        {
            var calenders = await _alpacaService.AlpacaTradingClient.ListIntervalCalendarAsync((new CalendarRequest().SetInclusiveTimeInterval(day.AddDays(-pastDays), day)));
            return calenders.OrderByDescending(x => x.GetTradingDate());
        }

        private async Task<IEnumerable<IIntervalCalendar>> GetOpenMarketDays(DateTime from, DateTime to)
        {
            var calenders = await _alpacaService.AlpacaTradingClient.ListIntervalCalendarAsync((new CalendarRequest().SetInclusiveTimeInterval(from, to)));
            return calenders.OrderByDescending(x => x.GetTradingDate());
        }

    }
}
