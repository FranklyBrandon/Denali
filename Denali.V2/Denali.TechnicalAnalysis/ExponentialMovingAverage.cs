using Denali.Models;
using Denali.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.TechnicalAnalysis
{
    public class ExponentialMovingAverage
    {
        private readonly int _backlog;
        private readonly decimal _smoothingConstant;
        private readonly SimpleMovingAverage _sma;
        public IList<decimal> MovingAverages { get; set; }

        public ExponentialMovingAverage(int backlog)
        {
            this._backlog = backlog;
            this._sma = new SimpleMovingAverage(backlog);
            this.MovingAverages = new List<decimal>();
            this._smoothingConstant = (2m / (_backlog + 1m));
        }

        public void Analyze(IEnumerable<IAggregateBar> data)
        {
            //Calculate initial EMA 
            if (!MovingAverages.Any())
            {
                _sma.Analyze(data);

                if (_sma.MovingAverages.Any())
                    MovingAverages.Add(_sma.MovingAverages.Last());
            }
            else
            {
                //Calculate running EMA value 
                var currentBar = data.Last();
                var previousEma = MovingAverages.Last();
                var newEma = ((currentBar.Close - previousEma) * _smoothingConstant + previousEma).RoundToMoney();
                MovingAverages.Add(newEma);
            }
        }
    }
}
