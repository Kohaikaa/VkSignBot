namespace VkSignBot.Repositories
{
    internal class MongoDBUsersRepository : IMongoDBUsersRepository
    {
        public Task<VkUser> AddItemAsync(long id, VkUser item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteItemAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<VkUser> FindItemAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<VkUser> GetItemAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VkUser>> GetItemsAsync(long id)
        {
            throw new NotImplementedException();
        }
    }
}
