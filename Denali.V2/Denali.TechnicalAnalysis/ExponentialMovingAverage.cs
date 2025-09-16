using Alpaca.Markets;
using Denali.Models;
using Denali.Shared.Extensions;

namespace Denali.TechnicalAnalysis
{
    public class ExponentialMovingAverage
    {
        private readonly int _backlog;
        private readonly decimal _smoothingConstant;
        private readonly SimpleMovingAverageClose _sma;
        public IList<EMA> MovingAverages { get; set; }

        public ExponentialMovingAverage(int backlog)
        {
            this._backlog = backlog;
            this._sma = new SimpleMovingAverageClose(backlog);
            this.MovingAverages = new List<EMA>();
            this._smoothingConstant = (2m / (_backlog + 1m));
        }

        public void Analyze(IEnumerable<IBar> data)
        {
            // Calculate initial EMA 
            if (!MovingAverages.Any())
            {
                _sma.Analyze(data);

                if (_sma.MovingAverages.Any())
                    MovingAverages.Add(new EMA(_sma.MovingAverages.Last(), data.Last().TimeUtc));
            }
            //Calculate running EMA value 
            else
            {
                var currentBar = data.Last();
                var previousEma = MovingAverages.Last();
                var newValue = ((currentBar.Close - previousEma.Value) * _smoothingConstant + previousEma.Value).RoundToMoney();
                MovingAverages.Add(new EMA(newValue, currentBar.TimeUtc));
            }
        }

        public void AnalyzeAll(IEnumerable<IBar> data)
        {
            var runningList = new List<IBar>();
            foreach (var dataItem in data)
            {
                runningList.Add(dataItem);
                Analyze(runningList);
            }
        }
    }
}
