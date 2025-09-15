namespace Denali.Processors.DenaliClimbStrategy
{
    public class DenaliClimbStrategySettings
    {
        public const string Settings = "DenaliClimbStrategySettings";
        public int AfterMarketOpenBufferMinutes { get; set; }
        public int PreMarketBufferMinutes { get; set; }
        public decimal MinimumStockPrice { get; set; }
        public int SlowEMABacklog { get; set; } 
        public int FastEMABacklog { get; set; }
        public decimal StopLossPercentage { get; set; }
        public decimal TakeProfitPercentage { get; set; }
    }
}
