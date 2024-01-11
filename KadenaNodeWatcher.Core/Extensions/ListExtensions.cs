using KadenaNodeWatcher.Core.Models;

namespace KadenaNodeWatcher.Core.Extensions;

internal static class ListExtensions
{
    internal static void AddUnique<T>(this IList<T> self, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            if (self.Contains(item))
            {
                continue;
            }
                
            self.Add(item);
        }
    }
        
    internal static void AddUniqueAddress(this IList<Peer> self, IEnumerable<Peer> items)
    {
        foreach (Peer item in items)
        {
            if (self.Any(peer => peer.Address.Hostname.Equals(item.Address.Hostname)))
            {
                continue;
            }
    
            self.Add(item);
        }
    }
    
    internal static void AddUniqueAddress(this ConcurrentList<Peer> self, IEnumerable<Peer> items)
    {
        foreach (Peer item in items)
        {
            if (self.Any(peer => peer.Address.Hostname.Equals(item.Address.Hostname)))
            {
                continue;
            }
    
            self.Add(item);
        }
    }
}