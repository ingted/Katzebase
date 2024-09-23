namespace NTDLS.Katzebase.Client.Types
{
    /// <summary>
    /// The katzebase case-insensitive dictionary.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class KbInsensitiveDictionary<TValue> : Dictionary<string, TValue>
    {
        public KbInsensitiveDictionary()
            : base(StringComparer.InvariantCultureIgnoreCase) { }

        public KbInsensitiveDictionary<TValue> Clone()
        {
            var clone = new KbInsensitiveDictionary<TValue>();
            foreach (var source in this)
            {
                clone.Add(source.Key, source.Value);
            }
            return clone;
        }
    }


    [Serializable]
    public class KbInsensitiveDictionary<TKey, TValue> : Dictionary<TKey, TValue>
        where TKey : notnull
    {
        Func<TKey, TKey, bool> _keyComparer;
        public KbInsensitiveDictionary(Func<TKey, TKey, bool>  keyComparer)
            : base() {
            _keyComparer = keyComparer;
        }

        public KbInsensitiveDictionary<TKey, TValue> Clone()
        {
            var clone = new KbInsensitiveDictionary<TKey, TValue>(_keyComparer);
            foreach (var source in this)
            {
                clone.Add(source.Key, source.Value);
            }
            return clone;
        }
        public new bool TryGetValue(TKey key, out TValue value)
        {
            foreach (var entry in this)
            {
                if (_keyComparer(entry.Key, key)) // 使用自訂比較器
                {
                    value = entry.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }

        // 覆寫 ContainsKey，使用自訂的比較函數來判斷是否包含 key
        public new bool ContainsKey(TKey key)
        {
            return this.Keys.Any(k => _keyComparer(k, key)); // 使用自訂比較器檢查 key
        }

        // 覆寫索引器（get）以便使用自訂比較函數
        public TValue this[TKey key]
        {
            get
            {
                foreach (var entry in this)
                {
                    if (_keyComparer(entry.Key, key)) // 使用自訂比較器
                    {
                        return entry.Value;
                    }
                }
                throw new KeyNotFoundException("The given key was not found in the dictionary.");
            }
            set
            {
                base[key] = value;
            }
        }
    }
}
