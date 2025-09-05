using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractiveBrokers.Models.Configuration
{
    public class InteractiveBrokersSettings
    {
        public static string Settings = "InteractiveBrokersSettings";

        public string GatewayBaseURL { get; set; }
        public string PingGateway { get; set; }
        public string HMDSInit { get; set; }
        public string BrokerageInit { get; set; }
        public string HistoricAggregate { get; set; }
        public string HistoricAggregateBeta { get; set; }
        public string ContractIdsByExchange { get; set; }
    }
}
