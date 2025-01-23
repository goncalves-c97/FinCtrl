using FinCtrlLibrary.Models;

namespace FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces
{
    public interface ICategory
    {
        public Task<List<Category>> GetListAsync();
        public Task InsertNewAsync(Category category);
        public Task DeleteByIdAsync(int id);
        public Task<Category> GetByIdAsync(int id);
    }
}
