
using System.Collections;
using System.Collections.Generic;

namespace WBPlatform.StaticClasses
{
    public class AutoDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TValue : class
    {
        public new TValue this[TKey key]
        {
            get => ContainsKey(key) ? ((Dictionary<TKey, TValue>)this)[key] : null;
            set
            {
                if (ContainsKey(key)) this[key] = value;
                else Add(key, value);
            }
        }
    }
}
