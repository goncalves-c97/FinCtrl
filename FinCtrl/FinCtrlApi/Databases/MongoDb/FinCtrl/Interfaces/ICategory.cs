using FinCtrlLibrary.Models;

namespace FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces
{
    public interface ICategory
    {
        public List<Category> GetList();
        public void InsertNew(Category category);
        public void DeleteById(int id);
        public Category GetById(int id);
    }
}
