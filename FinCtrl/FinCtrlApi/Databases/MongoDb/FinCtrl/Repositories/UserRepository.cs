using FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces;
using FinCtrlLibrary.Models;

namespace FinCtrlApi.Databases.MongoDb.FinCtrl.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(FinCtrlAppDbContext context) : base(context) { }

        public async Task<User> GetByEmailAndPasswordAsync(string email, string password)
        {
            Dictionary<string, object> filter = new()
            {
                {nameof(User.Email), email},
                {nameof(User.Password), password}
            };

            return await GetFirstOrDefaultByPropertiesAsync(filter);
        }
    }
}
