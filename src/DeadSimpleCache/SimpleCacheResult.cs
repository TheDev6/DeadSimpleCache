namespace DeadSimpleCache
{
    public class SimpleCacheResult<T>
    {
        public bool IsNull { get; set; }
        public T ValueOrDefault { get; set; }
    }
}
