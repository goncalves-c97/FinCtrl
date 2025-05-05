using FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces;
using FinCtrlLibrary.Models;

namespace FinCtrlApi.Databases.MongoDb.FinCtrl.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(FinCtrlAppDbContext context) : base(context) { }

        public async Task<bool> CheckIfAlreadyExistsUserByEmail(string email)
        {
            Dictionary<string, object> filter = new()
            {
                {nameof(User.Email), email}
            };

            User? existingUser = await GetFirstOrDefaultByPropertiesAsync(filter);

            return existingUser != null;
        }

        public async Task<User> GetByEmailAndPasswordAsync(string email, string password)
        {
            Dictionary<string, object> filter = new()
            {
                {nameof(User.Email), email},
                {nameof(User.Password), password}
            };

            return await GetFirstOrDefaultByPropertiesAsync(filter);
        }

        async Task IUserRepository.InsertNewAsync(User user)
        {
            user.CreationDateTime = DateTime.UtcNow;
            await InsertNewAsync(user);
        }
    }
}
