using FinCtrlLibrary.Interfaces;

namespace FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces
{
    public interface IGenericRepository<T> where T : class, IMongoDbItem
    {
        public Task<List<T>> GetListAsync();
        public Task<T?> GetByIdAsync(string id);
        public Task InsertNewAsync(T entity);
        public Task DeleteByIdAsync(string id);
    }
}
