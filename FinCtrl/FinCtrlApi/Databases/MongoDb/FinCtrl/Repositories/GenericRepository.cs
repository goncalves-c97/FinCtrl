using FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces;
using FinCtrlApi.Utilities;
using FinCtrlLibrary.Interfaces;
using FinCtrlLibrary.Models.GenericModels;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using System.Xml.Linq;

namespace FinCtrlApi.Databases.MongoDb.FinCtrl.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IMongoDbItem
    {
        private readonly FinCtrlAppDbContext _context;

        public GenericRepository(FinCtrlAppDbContext context) => _context = context;

        public async Task DeleteByIdAsync(string id)
        {
            await ProjPolicies.ExecuteWithRetryAsync(async () =>
            {
                T? entity = await GetByIdAsync(id);
                if (entity != null)
                {
                    _context.Set<T>().Remove(entity);
                    await _context.SaveChangesAsync();
                }
            });
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            return await ProjPolicies.ExecuteWithRetryAsync(() =>
                _context.Set<T>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => EF.Property<ObjectId>(x, nameof(IMongoDbItem._id)).ToString().Equals(id, StringComparison.CurrentCultureIgnoreCase))
            );
        }

        public async Task<List<T>> GetListAsync()
        {
            return await ProjPolicies.ExecuteWithRetryAsync(() =>
                _context.Set<T>()
                    .AsNoTracking()
                    .ToListAsync()
            );
        }

        public async Task InsertNewAsync(T entity)
        {
            await ProjPolicies.ExecuteWithRetryAsync(async () =>
            {
                await _context.Set<T>().AddAsync(entity);
                await _context.SaveChangesAsync();
            });
        }
    }
}
