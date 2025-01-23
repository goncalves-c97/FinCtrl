using FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces;
using FinCtrlApi.Utilities;
using FinCtrlLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace FinCtrlApi.Databases.MongoDb.FinCtrl.Repositories
{
    public class CategoryRepository : ICategory
    {
        private readonly FinCtrlAppDbContext _context;

        public CategoryRepository(FinCtrlAppDbContext appDbContext) => _context = appDbContext;

        public async Task DeleteByIdAsync(int id)
        {
            await ProjPolicies.ExecuteWithRetryAsync(async () =>
            {
                Category? category = await GetByIdAsync(id);

                if (category == null) 
                    return;

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            });
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await ProjPolicies.ExecuteWithRetryAsync(() =>
                _context.Categories
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id)
            );
        }

        public async Task<List<Category>> GetListAsync()
        {
            return await ProjPolicies.ExecuteWithRetryAsync(() =>
                _context.Categories
                    .AsNoTracking()
                    .ToListAsync()
            );
        }

        public async Task InsertNewAsync(Category category)
        {
            await ProjPolicies.ExecuteWithRetryAsync(async () =>
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            });
        }
    }
}
