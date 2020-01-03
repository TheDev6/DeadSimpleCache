namespace DeadSimpleCache
{
    public class SimpleCacheResult<T>
    {
        /// <summary>
        /// Always check for cache null, especially for value types like false(not nullable) bool where the default would give the incorrect answer of false, when the cache was actually empty.
        /// </summary>
        public bool IsCacheNull { get; set; }
        public T ValueOrDefault { get; set; }
    }
}
