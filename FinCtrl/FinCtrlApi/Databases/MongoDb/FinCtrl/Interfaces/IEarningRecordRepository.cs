using FinCtrlLibrary.Models;

namespace FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces
{
    public interface IEarningRecordRepository : IGenericRepository<EarningRecord>
    {
        public Task<List<EarningRecord>> GetListWithIncludesAsync(IGenericRepository<EarningCategory> spendingCategoryRepo);
    }
}
