using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors.DenaliDescentStrategy
{
    public class DenaliDescentStrategySettings
    {
        public const string Settings = "DenaliDescentStrategySettings";
        public decimal MinimumStockPrice { get; set; }
        public decimal MinimumGapUpPercentage { get; set; }
    }
}
