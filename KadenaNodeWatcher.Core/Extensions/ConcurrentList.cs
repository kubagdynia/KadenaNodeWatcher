using System.Collections;

namespace KadenaNodeWatcher.Core.Extensions;

public class ConcurrentList<T> : IList<T>
{
    private readonly List<T> _list = [];
    private readonly object _syncRoot = new();
    
    public IEnumerator<T> GetEnumerator()
    {
        lock (_syncRoot)
        {
            return _list.ToList().GetEnumerator();
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(T item)
    {
        lock (_syncRoot)
        {
            _list.Add(item);
        }
    }

    public void Clear()
    {
        lock (_syncRoot)
        {
            _list.Clear();
        }
    }

    public bool Contains(T item)
    {
        lock (_syncRoot)
        {
            return _list.Contains(item);
        }
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        lock (_syncRoot)
        {
            _list.CopyTo(array, arrayIndex);
        }
    }

    public bool Remove(T item)
    {
        lock (_syncRoot)
        {
            return _list.Remove(item);
        }
    }

    public int Count
    {
        get
        {
            if (_list == null) return 0;
            lock (_syncRoot)
            {
                return _list.Count;
            }
        }
    }

    public bool IsReadOnly { get; }
    public int IndexOf(T item)
    {
        lock (_syncRoot)
        {
            return _list.IndexOf(item);
        }
    }

    public void Insert(int index, T item)
    {
        lock (_syncRoot)
        {
            _list.Insert(index, item);
        }
    }

    public void RemoveAt(int index)
    {
        lock (_syncRoot)
        {
            _list.RemoveAt(index);
        }
    }

    public T this[int index]
    {
        get
        {
            lock (_syncRoot)
            {
                return _list[index];
            }
        }
        set
        {
            lock (_syncRoot)
            {
                _list[index] = value;
            }
        }
    }

    public void AddRange(IEnumerable<T> items)
    {
        lock (_syncRoot)
        {
            foreach (var item in items)
            {
                _list.Add(item);
            }
        }
    }

    public void RemoveAll(Predicate<T> match)
    {
        lock (_syncRoot)
        {
            _list.RemoveAll(match);
        }
    }

    public void Sort(Comparison<T> comparison)
    {
        lock (_syncRoot)
        {
            _list.Sort(comparison);
        }
    }

    public IList<T> GetSnapshot()
    {
        lock (_syncRoot)
        {
            return _list.ToList();
        }
    }
}