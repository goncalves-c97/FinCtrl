using FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces;
using FinCtrlLibrary.Models;

namespace FinCtrlApi.Databases.MongoDb.FinCtrl.Repositories
{
    public class EarningRecordRepository : GenericRepository<EarningRecord>, IEarningRecordRepository
    {
        public EarningRecordRepository(FinCtrlAppDbContext context) : base(context) { }

        public async Task<List<EarningRecord>> GetListWithIncludesAsync(IGenericRepository<EarningCategory> spendingCategoryRepo)
        {
            List<EarningRecord> earningRecords = await GetListAsync();

            List<EarningCategory> earningCategories = await spendingCategoryRepo.GetListAsync();

            foreach (EarningRecord earningRecord in earningRecords)
            {
                EarningCategory? earningCategory = earningCategories.FirstOrDefault(x => x._id.ToString() == earningRecord.EarningCategoryBsonId);
                earningRecord.EarningCategory = earningCategory;
            }

            return earningRecords;
        }
    }
}
