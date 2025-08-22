using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractiveBrokers.Models.Response
{
    public class Aggregate
    {
        public long t { get; set; }
        public double o { get; set; }
        public double c { get; set; }
        public double h { get; set; }
        public double l { get; set; }
        public double v { get; set; }
    }

    public class HistoricAggregateResponse
    {
        public string startTime { get; set; }
        public long startTimeVal { get; set; }
        public string endTime { get; set; }
        public long endTimeVal { get; set; }
        public List<Aggregate> data { get; set; }
        public int points { get; set; }
        public int mktDataDelay { get; set; }
    }
}
