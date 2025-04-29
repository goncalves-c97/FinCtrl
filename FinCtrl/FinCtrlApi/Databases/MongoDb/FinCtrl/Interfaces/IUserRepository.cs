using FinCtrlLibrary.Models;

namespace FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        public Task<User> GetByEmailAndPasswordAsync(string email, string password);
    }
}
