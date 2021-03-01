using Denali.Algorithms.AggregateAnalysis.SMAStrategy;
using Denali.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Algorithms.AggregateAnalysis.EMA
{
    public class EMA
    {
        private readonly int _backlog;
        private readonly decimal _smoothingConstant;
        private readonly SMA _sma;
        public IList<decimal> MovingAverages { get; set; }

        public EMA(int backlog)
        {
            this._backlog = backlog;
            this._sma = new SMA(backlog);
            this.MovingAverages = new List<decimal>();
            this._smoothingConstant = 2 / (_backlog + 1);
        }

        public void Analyze(IEnumerable<IAggregateData> data)
        { 
            //Calculate initial EMA 
            if (!MovingAverages.Any())
            {
                _sma.Analyze(data);

                if (_sma.MovingAverages.Any())
                    MovingAverages.Add(_sma.MovingAverages.First());
            }
            else
            {
                //Calculate running EMA value 
                var previousEma = MovingAverages.Last();
                var newEma = (data.Last().ClosePrice - previousEma) * _smoothingConstant + previousEma;
                MovingAverages.Add(newEma);
            }
        }
    }
}
