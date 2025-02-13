using FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces;
using FinCtrlLibrary.Models;

namespace FinCtrlApi.Databases.MongoDb.FinCtrl.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(FinCtrlAppDbContext context) : base(context) { }

        public User GetByEmailAndPassword(string email, string password)
        {
            throw new NotImplementedException();
        }

        public User GetByUsernameAndPassword(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
