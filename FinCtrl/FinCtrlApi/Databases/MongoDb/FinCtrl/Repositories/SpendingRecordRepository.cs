using FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces;
using FinCtrlLibrary.Models;

namespace FinCtrlApi.Databases.MongoDb.FinCtrl.Repositories
{
    public class SpendingRecordRepository : GenericRepository<SpendingRecord>, ISpendingRecordRepository
    {
        public SpendingRecordRepository(FinCtrlAppDbContext context) : base(context) { }

        public async Task<List<SpendingRecord>> GetListWithIncludesAsync(IGenericRepository<SpendingCategory> spendingCategoryRepo, IGenericRepository<PaymentCategory> paymentCategoryRepo, IGenericRepository<TagCategory> tagCategoryRepo, IGenericRepository<SpendingRule> spendingRuleRepo)
        {
            List<SpendingRecord> spendingRecords = await GetListAsync();

            List<SpendingCategory> spendingCategories = await spendingCategoryRepo.GetListAsync();
            List<PaymentCategory> paymentCategories = await paymentCategoryRepo.GetListAsync();
            List<TagCategory> tagCategories = await tagCategoryRepo.GetListAsync();
            List<SpendingRule> spendingRules = await spendingRuleRepo.GetListAsync();

            foreach (SpendingRecord spendingRecord in spendingRecords)
            {
                SpendingCategory? spendingCategory = spendingCategories.FirstOrDefault(x => x._id.ToString() == spendingRecord.CategoryBsonId);
                PaymentCategory? paymentCategory = paymentCategories.FirstOrDefault(x => x._id.ToString() == spendingRecord.PaymentCategoryBsonId);
                List<TagCategory>? tagCategoriesList = tagCategories.Where(x => spendingRecord.Tags.Select(x => x._id.ToString()).ToList().Contains(x._id.ToString())).ToList();
                SpendingRule? spendingRule = spendingRules.FirstOrDefault(x => x._id.ToString() == spendingRecord.SpendingRuleBsonId);

                spendingRecord.Category = spendingCategory;
                spendingRecord.PaymentCategory = paymentCategory;
                spendingRecord.Tags = tagCategoriesList;
                spendingRecord.SpendingRule = spendingRule;
            }

            return spendingRecords;
        }
    }
}
