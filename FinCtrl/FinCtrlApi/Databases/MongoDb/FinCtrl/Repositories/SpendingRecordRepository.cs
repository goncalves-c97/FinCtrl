using FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces;
using FinCtrlApi.Utilities;
using FinCtrlLibrary.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace FinCtrlApi.Databases.MongoDb.FinCtrl.Repositories
{
    public class SpendingRecordRepository : ISpendingRecord
    {
        private readonly FinCtrlAppDbContext _context;

        public SpendingRecordRepository(FinCtrlAppDbContext appDbContext) => _context = appDbContext;

        public async Task DeleteByIdAsync(int id)
        {
            await ProjPolicies.ExecuteWithRetryAsync(async () =>
            {
                SpendingRecord? spendingRecord = await GetByIdAsync(id);

                if (spendingRecord == null)
                    return;

                _context.SpendingRecords.Remove(spendingRecord);
                await _context.SaveChangesAsync();
            });
        }

        public async Task<SpendingRecord?> GetByBsonIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                throw new ArgumentException("Invalid ObjectId format.", nameof(id));
            }

            return await ProjPolicies.ExecuteWithRetryAsync(() =>
                _context.SpendingRecords
                    .AsNoTracking()
                    .AsQueryable()
                    .Where(x => x._id.ToString() == id)
                    .FirstOrDefaultAsync()
            );
        }

        public async Task<SpendingRecord?> GetByIdAsync(int id)
        {
            return await ProjPolicies.ExecuteWithRetryAsync(() =>
                _context.SpendingRecords
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id)
            );
        }

        public async Task<List<SpendingRecord>> GetListAsync()
        {
            return await ProjPolicies.ExecuteWithRetryAsync(() =>
                _context.SpendingRecords
                    .AsNoTracking()
                    .ToListAsync()
            );
        }

        public async Task InsertNewAsync(SpendingRecord spendingRecord)
        {
            await ProjPolicies.ExecuteWithRetryAsync(async () =>
            {
                _context.SpendingRecords.Add(spendingRecord);
                await _context.SaveChangesAsync();
            });
        }
    }
}
