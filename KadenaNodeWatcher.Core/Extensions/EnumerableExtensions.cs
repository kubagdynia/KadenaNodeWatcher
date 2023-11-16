namespace KadenaNodeWatcher.Core.Extensions;

public static class EnumerableExtensions
{
    public static List<T> GetRandomElements<T>(this IEnumerable<T> list, int elementsCount)
        => list.OrderBy(_ => Guid.NewGuid()).Take(elementsCount).ToList();
}