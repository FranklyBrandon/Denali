namespace Denali.Processors.DenaliClimbStrategy
{
    public class DenaliClimbStrategySettings
    {
        public const string Settings = "DenaliClimbStrategySettings";
        public int AfterMarketOpenBufferMinutes { get; set; } = 30;
        public int PreMarketBufferMinutes { get; set; } = 9;
        public decimal MinimumStockPrice { get; set; } = 10m;
        public int SlowEMABacklog { get; set; }  = 8;
        public int FastEMABacklog { get; set; }  = 3;
    }
}
