using System;
using System.Collections.Generic;

namespace Engine2D.Tiles.Caching
{
    public sealed class MemoryLruCache<TKey, TValue>
        where TKey : notnull
    {
        private readonly int _maxEntries;
        private readonly Action<TKey, TValue>? _onEvict;

        private readonly Dictionary<TKey, LinkedListNode<Entry>> _map = new();
        private readonly LinkedList<Entry> _lru = new();

        private sealed record Entry(TKey Key, TValue Value);

        public MemoryLruCache(int maxEntries, Action<TKey, TValue>? onEvict = null)
        {
            if (maxEntries <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxEntries));

            _maxEntries = maxEntries;
            _onEvict = onEvict;
        }

        public bool TryGet(TKey key, out TValue value)
        {
            if (_map.TryGetValue(key, out var node))
            {
                // Mark as most recently used
                _lru.Remove(node);
                _lru.AddFirst(node);

                value = node.Value.Value;
                return true;
            }

            value = default!;
            return false;
        }

        public void Add(TKey key, TValue value)
        {
            // If already present, update + promote
            if (_map.TryGetValue(key, out var existing))
            {
                _lru.Remove(existing);
                _lru.AddFirst(existing);
                return;
            }

            // Evict until under limit
            while (_map.Count >= _maxEntries)
                EvictLast();

            var entry = new Entry(key, value);
            var node = new LinkedListNode<Entry>(entry);

            _lru.AddFirst(node);
            _map[key] = node;
        }

        private void EvictLast()
        {
            var node = _lru.Last!;
            _lru.RemoveLast();
            _map.Remove(node.Value.Key);

            _onEvict?.Invoke(node.Value.Key, node.Value.Value);
        }
    }
}
