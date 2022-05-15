namespace Denali.Shared.Extensions
{
    public static class ListExtensions
    {
        public static T GetHistoricValue<T>(this IList<T> collection, int history) => collection[collection.Count - 1 - history];
    }
}
