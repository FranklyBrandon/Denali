namespace Denali.TechnicalAnalysis
{
    public static class ChangePercentage
    {
        public static decimal Calculate(decimal oldValue, decimal newValue) => 
            (newValue - oldValue) / oldValue * 100;
    }
}
