namespace VkSignBot.Repositories.Abstractions
{
    public interface IRepository<T, TKey>
    {
        Task<T> GetItemAsync(TKey id);
        Task<IEnumerable<T>> GetItemsAsync(TKey id);
        Task<T> AddItemAsync(TKey id, T item);
        Task<bool> DeleteItemAsync(TKey id);
        Task<T> FindItemAsync(TKey id);
    }
}
