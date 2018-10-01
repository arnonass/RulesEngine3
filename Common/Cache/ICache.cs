namespace RulesService.Caching
{
    public interface ICache<T>
    {
        void Add(string name, T item);
        void Clear(string name);
        object GetCachedItem(string name);
    }
}