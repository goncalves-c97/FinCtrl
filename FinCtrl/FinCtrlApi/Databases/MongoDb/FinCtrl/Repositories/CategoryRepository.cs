using FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces;
using FinCtrlLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace FinCtrlApi.Databases.MongoDb.FinCtrl.Repositories
{
    public class CategoryRepository : ICategory
    {
        private FinCtrlAppDbContext _context;

        public CategoryRepository(FinCtrlAppDbContext appDbContext) => _context = appDbContext;

        public void DeleteById(int id)
        {
            Category? category = GetById(id);

            try
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public Category? GetById(int id)
        {
            return _context.Categories
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id);
        }

        public List<Category> GetList()
        {
            return _context.Categories
                .AsNoTracking()
                .ToList();
        }

        public void InsertNew(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }
    }
}
