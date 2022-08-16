using Alpaca.Markets;
using Denali.Models;
using Denali.Services;
using Denali.Services.Aggregators;
using Denali.Services.AlphaAdvantage;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Denali.Processors.StatArb
{
    public class PairTradeStrategy
    {
        private readonly Dictionary<string, BarAggregator> _aggregatorMap;

        private readonly AlpacaService _alpacaService;
        private readonly ILogger<PairTradeStrategy> _logger;
        private readonly IAlphaAdvanatgeClient _alphaAdvantageClient;

        private System.Threading.Timer _timer;

        private string _tickerX;
        private string _tickerY;
        private BarTimeFrame _barTimeFrame;
        private int _lookback;

        private static readonly object _intervalLock = new ();
        private static bool _intervalReceived = false;

        public PairTradeStrategy(AlpacaService alpacaService, IAlphaAdvanatgeClient alphaAdvantageClient, ILogger<PairTradeStrategy> logger)
        {
            _alpacaService = alpacaService;
            _alphaAdvantageClient = alphaAdvantageClient;
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

        
        public void StartTimer()
        {
            var la = DateTime.UtcNow;
            ScheduleTimer(new DateTime(2022, 8, 15, 2, 48, 0, DateTimeKind.Utc));
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
            var xTask = _alphaAdvantageClient.GetQuote(_tickerX);
            var yTask = _alphaAdvantageClient.GetQuote(_tickerY);
            Task[] tasks = new Task[2] { xTask, yTask };

            _logger.LogInformation("Time evented");

            var allTasks = Task.WhenAll(tasks);
            await allTasks;

            _logger.LogInformation($"{xTask.Result.Quote.Symbol}: {xTask.Result.Quote.Price}");
            _logger.LogInformation($"{yTask.Result.Quote.Symbol}: {yTask.Result.Quote.Price}");

            ScheduleTimer(alertTime.AddMinutes(1));
        }     
    }
}
