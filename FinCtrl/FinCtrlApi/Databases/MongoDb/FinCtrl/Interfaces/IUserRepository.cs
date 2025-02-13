using FinCtrlLibrary.Models;

namespace FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        public User GetByEmailAndPassword(string email, string password);
        public User GetByUsernameAndPassword(string username, string password);
    }
}
