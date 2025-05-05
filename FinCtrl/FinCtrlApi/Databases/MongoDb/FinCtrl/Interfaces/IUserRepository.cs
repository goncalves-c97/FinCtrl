using FinCtrlLibrary.Models;

namespace FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        public Task<User> GetByEmailAndPasswordAsync(string email, string password);
        public Task<bool> CheckIfAlreadyExistsUserByEmail(string email);
        public new Task InsertNewAsync(User user);
    }
}
