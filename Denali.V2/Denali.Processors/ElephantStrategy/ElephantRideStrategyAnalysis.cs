﻿using Alpaca.Markets;
using AutoMapper;
using Denali.Models;
using Denali.Services;
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
                new HistoricalBarsRequest("AAPL", backlogDay1.GetSessionOpenTimeUtc(), backlogDay1.GetTradingCloseTimeUtc(), new BarTimeFrame(5, BarTimeFrameUnit.Minute)));

            var backlog2 = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                new HistoricalBarsRequest("AAPL", backlogDay2.GetTradingOpenTimeUtc(), backlogDay2.GetTradingCloseTimeUtc(), new BarTimeFrame(5, BarTimeFrameUnit.Minute)));

            var alpacaCurrentData = await _alpacaService.AlpacaDataClient.ListHistoricalBarsAsync(
                new HistoricalBarsRequest("AAPL", currentDay.GetTradingOpenTimeUtc(), currentDay.GetTradingCloseTimeUtc(), new BarTimeFrame(5, BarTimeFrameUnit.Minute)));

            var alpacaBackLog = backlog2.Items.ToList();
            alpacaBackLog.AddRange(backlog1.Items.ToList());

            var backlogData = _mapper.Map<List<AggregateBar>>(alpacaBackLog);
            var currentData = _mapper.Map<List<AggregateBar>>(alpacaCurrentData.Items);

            var elephantBars = new ElephantBars(_settings);
            elephantBars.Initialize(backlogData);

            int start = 1;
            int count = currentData.Count - 1;

            // Start analysis at start of day (not including premarket bars)
            decimal total = 0.0m;
            for (int i = start; i < count; i++)
            {
                var bars = currentData.Take(i);
                elephantBars.Analyze(bars);

                var lastBar = bars.Last();
                var nextBar = currentData.ElementAt(i);
                var elephantBodyentry = (elephantBars.AverageRange.AverageRanges.Last().AverageBodyRange * _settings.OverAverageThreshold);

                if (elephantBars.IsLatestElephant() || ((lastBar.High - lastBar.Open) >= elephantBodyentry || (lastBar.Open - lastBar.Low) > elephantBodyentry))
                {
                    decimal diff = 0.0m;
                    if (lastBar.Green())
                    {
                        diff = nextBar.Open - (lastBar.Open + elephantBodyentry);
                    }
                    else
                    {
                        diff = (lastBar.Open - elephantBodyentry) - nextBar.Open;
                    }

                    diff = (diff * 20) - 0.70m;
                    _logger.LogInformation($"Elephant at {lastBar.TimeUtc}: Gain of {diff}");
                    total += diff;
                }
            }

            _logger.LogInformation($"Total Profit: {total}");
        }

        private async Task<IEnumerable<IIntervalCalendar>> GeOpenMarketDays(int pastDays, DateTime day)
        {
            var calenders = await _alpacaService.AlpacaTradingClient.ListIntervalCalendarAsync((new CalendarRequest().SetInclusiveTimeInterval(day.AddDays(-pastDays), day)));
            return calenders.OrderByDescending(x => x.GetTradingDate());
        }

    }
}