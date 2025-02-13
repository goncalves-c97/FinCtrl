using FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces;
using FinCtrlApi.Utilities;
using FinCtrlLibrary.Interfaces;
using FinCtrlLibrary.Models.GenericModels;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using System.Linq.Expressions;
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

        public async Task<List<T>> GetByPropertiesAsync(Dictionary<string, object> filters)
        {
            return await ProjPolicies.ExecuteWithRetryAsync(async () =>
            {
                IQueryable<T> query = _context.Set<T>().AsNoTracking();

                foreach (var filter in filters)
                {
                    string propertyName = filter.Key;
                    object propertyValue = filter.Value;

                    var parameter = Expression.Parameter(typeof(T), "x");
                    var property = Expression.Property(parameter, propertyName);

                    var constant = Expression.Constant(propertyValue);
                    Expression body;

                    if (propertyValue is string)
                    {
                        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                        var toLowerProperty = Expression.Call(property, toLowerMethod);
                        var toLowerConstant = Expression.Call(constant, toLowerMethod);
                        body = Expression.Equal(toLowerProperty, toLowerConstant);
                    }
                    else
                        body = Expression.Equal(property, Expression.Convert(constant, property.Type));

                    var predicate = Expression.Lambda<Func<T, bool>>(body, parameter);
                    query = query.Where(predicate);
                }

                return await query.ToListAsync();
            });
        }

    }
}
