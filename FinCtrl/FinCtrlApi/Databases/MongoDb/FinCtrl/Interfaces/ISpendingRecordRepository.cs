using FinCtrlApi.Databases.MongoDb.FinCtrl.Repositories;
using FinCtrlLibrary.Models;

namespace FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces
{
    public interface ISpendingRecordRepository : IGenericRepository<SpendingRecord>
    {
        public Task<List<SpendingRecord>> GetListWithIncludesAsync(IGenericRepository<SpendingCategory> spendingCategoryRepo, IGenericRepository<PaymentCategory> paymentCategoryRepo, IGenericRepository<TagCategory> tagCategoryRepo, IGenericRepository<SpendingRule> spendingRuleRepo);
    }
}
