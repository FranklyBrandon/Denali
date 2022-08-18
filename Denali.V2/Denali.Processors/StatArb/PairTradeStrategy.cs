using Alpaca.Markets;
using Denali.Models;
using Denali.Models.PythonInterop;
using Denali.Services;
using Denali.Services.Aggregators;
using Denali.Services.PythonInterop;
using Denali.Services.YahooFinanceService;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Denali.Shared.Extensions;

namespace Denali.Processors.StatArb
{
    public class PairTradeStrategy
    {
        private readonly Dictionary<string, BarAggregator> _aggregatorMap;

        private readonly IYahooFinanceService _yahooFinanceService;
        private readonly IPythonInteropClient _pythonInteropClient;
        private readonly ILogger<PairTradeStrategy> _logger;

        private System.Threading.Timer _timer;

        private string _tickerX;
        private string _tickerY;
        private List<IAggregateBar> _tickerXBars;
        private List<IAggregateBar> _tickerYBars;
        private string _barTimeFrame;
        private int _lookback;

        private List<LinearRegressionResult> _regressionResults;

        public PairTradeStrategy(IYahooFinanceService yahooFinanceService, ILogger<PairTradeStrategy> logger)
        {
            _yahooFinanceService = yahooFinanceService;
            _logger = logger;
            _aggregatorMap = new();
            _regressionResults = new();
        }

        public async Task Initialize(string tickerX, string tickerY, DateTime startDate, string timeFrame, int lookback, CancellationToken cancellationToken)
        {
            _barTimeFrame = timeFrame;
            _lookback = lookback;
            _tickerX = tickerX;
            _tickerY = tickerY;

            var existingXSeries = await _yahooFinanceService.GetQuotes(tickerX, _barTimeFrame, "2d");
            var existingYSeries = await _yahooFinanceService.GetQuotes(tickerY, _barTimeFrame, "2d");
            await InitializeTickers(existingXSeries, existingYSeries);

            StartTimer();
        }

        public async Task InitializeTickers(List<IAggregateBar> xSeries, List<IAggregateBar> ySeries)
        {
            _tickerXBars = xSeries;
            _tickerYBars = ySeries;
            var regressionResults = await _pythonInteropClient.GetOLSCalculation(xSeries.TakeLast(_lookback), ySeries.TakeLast(_lookback), _lookback);
            _regressionResults.Add(regressionResults);

            StartTimer();
        }

        
        public void StartTimer()
        {
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
                this.IntervalQuote(alertTime);
            }, null, timeToGo, Timeout.InfiniteTimeSpan);
        }

        public async void IntervalQuote(DateTime alertTime)
        {
            var xTask = _yahooFinanceService.GetLatestQuote(_tickerX);
            var yTask = _yahooFinanceService.GetLatestQuote(_tickerY);
            Task[] tasks = new Task[2] { xTask, yTask };

            var allTasks = Task.WhenAll(tasks);
            await allTasks;

            var xResult = xTask.Result;
            var yResult = yTask.Result;

            _tickerXBars.Add(xResult);
            _tickerYBars.Add(yResult);

            var results = await _pythonInteropClient.GetOLSCalculation(_tickerXBars.TakeLast(_lookback), _tickerYBars.TakeLast(_lookback), _lookback);
            _regressionResults.Add(results);

            _logger.LogInformation($"{xResult.Symbol}: {xResult.Close} {xResult.TimeUtc}");
            _logger.LogInformation($"{yResult.Symbol}: {yResult.Close} {yResult.TimeUtc}");

            ScheduleTimer(alertTime.AddMinutes(5));
        }     
    }
}
