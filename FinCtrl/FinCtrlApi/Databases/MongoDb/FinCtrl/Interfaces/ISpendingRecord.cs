using FinCtrlLibrary.Models;

namespace FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces
{
    public interface ISpendingRecord
    {
        public Task<List<SpendingRecord>> GetListAsync();
        public Task InsertNewAsync(SpendingRecord spendingRecord);
        public Task DeleteByIdAsync(int id);
        public Task<SpendingRecord> GetByIdAsync(int id);
        public Task<SpendingRecord> GetByBsonIdAsync(string id);
    }
}
