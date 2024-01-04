namespace KadenaNodeWatcher.Core.Extensions;

internal static class EnumerableExtensions
{
    internal static List<T> GetRandomElements<T>(this IEnumerable<T> list, int elementsCount)
        => list.OrderBy(_ => Guid.NewGuid()).Take(elementsCount).ToList();
}