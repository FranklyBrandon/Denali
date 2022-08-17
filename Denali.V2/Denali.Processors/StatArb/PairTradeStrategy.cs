using Alpaca.Markets;
using Denali.Models;
using Denali.Services;
using Denali.Services.Aggregators;
using Denali.Services.YahooFinanceService;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Denali.Processors.StatArb
{
    public class PairTradeStrategy
    {
        private readonly Dictionary<string, BarAggregator> _aggregatorMap;

        private readonly IYahooFinanceService _yahooFinanceService;
        private readonly ILogger<PairTradeStrategy> _logger;

        private System.Threading.Timer _timer;

        private string _tickerX;
        private string _tickerY;
        private BarTimeFrame _barTimeFrame;
        private int _lookback;

        private static readonly object _intervalLock = new ();
        private static bool _intervalReceived = false;

        public PairTradeStrategy(IYahooFinanceService yahooFinanceService, ILogger<PairTradeStrategy> logger)
        {
            _yahooFinanceService = yahooFinanceService;
            _logger = logger;
            _aggregatorMap = new();
        }

        public async Task Initialize(string tickerX, string tickerY, DateTime startDate, BarTimeFrame timeFrame, int lookback, CancellationToken cancellationToken)
        {
            _barTimeFrame = timeFrame;
            _lookback = lookback;
            _tickerX = tickerX;
            _tickerY = tickerY;

            StartTimer();

        }

        
        public async void StartTimer()
        {
            var la = DateTime.UtcNow;
            var laa = await _yahooFinanceService.GetQuotes("VTI", "1m", "1d");
            MinuteQuote(la);
            ScheduleTimer(new DateTime(2022, 8, 16, 16, 08, 0, DateTimeKind.Utc));
        }

        private void ScheduleTimer(DateTime alertTime)
        {
            DateTime current = DateTime.UtcNow;
            TimeSpan timeToGo = alertTime.TimeOfDay - current.TimeOfDay;
            if (timeToGo < TimeSpan.Zero)
            {
                return;//time already passed
            }
            this._timer = new System.Threading.Timer(x =>
            {
                this.MinuteQuote(alertTime);
            }, null, timeToGo, Timeout.InfiniteTimeSpan);
        }

        private async void MinuteQuote(DateTime alertTime)
        {
            var xTask = _yahooFinanceService.GetLatestQuote(_tickerX);
            var yTask = _yahooFinanceService.GetLatestQuote(_tickerY);
            Task[] tasks = new Task[2] { xTask, yTask };

            var allTasks = Task.WhenAll(tasks);
            await allTasks;

            var xResult = xTask.Result;
            var yResult = yTask.Result;

            _logger.LogInformation($"{xResult.Symbol}: {xResult.Close} {xResult.TimeUtc}");
            _logger.LogInformation($"{yResult.Symbol}: {yResult.Close} {yResult.TimeUtc}");

            ScheduleTimer(alertTime.AddMinutes(1));
        }     
    }
}
