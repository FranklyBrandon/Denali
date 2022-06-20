using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractiveBrokers.API
{
    public class HistoricalDataManager
    {
        public ConcurrentDictionary<int, ManualResetEventSlim> ResponseSignals { get; set; }

        public HistoricalDataManager()
        {
            ResponseSignals = new ConcurrentDictionary<int,ManualResetEventSlim>();
        }
    }
}
