namespace Denali.Processors.DenaliClimbStrategy
{
    public class DenaliClimbStrategySettings
    {
        public const string Settings = "DenaliClimbStrategySettings";
        public int AfterMarketOpenStartTimeMinutes { get; set; }
        public decimal MinimumStockPrice { get; set; }
        public decimal MinimumGapUpPercentage { get; set; }
    }
}
