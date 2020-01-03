namespace DeadSimpleCache
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class SimpleCache
    {
        private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

        public void Set(string key, object objectOrValue)
        {
            _cache[key] = objectOrValue;
        }

        public SimpleCacheResult<T> Get<T>(string key)
        {
            var result = new SimpleCacheResult<T>() { IsNull = true, ValueOrDefault = default(T) };
            if (_cache.ContainsKey(key))
            {
                result.ValueOrDefault = (T)_cache[key];
                result.IsNull = result.ValueOrDefault == null;
            }
            return result;
        }

        public async Task<SimpleCacheResult<T>> GetCacheThenSourceAsync<T>(string key, Func<Task<T>> sourceDelegate)
        {
            var result = Get<T>(key);
            if (result.IsNull
                && sourceDelegate != null)
            {
                var source = await sourceDelegate();
                if (source != null)
                {
                    Set(key, source);
                    result = new SimpleCacheResult<T>
                    {
                        IsNull = false,
                        ValueOrDefault = source
                    };
                }
            }
            return result;
        }
    }
}
