using FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces;
using FinCtrlApi.Services;
using FinCtrlLibrary.Models;

namespace FinCtrlApi.Databases.MongoDb.FinCtrl.Repositories
{
    public class AuthenticationRepository : GenericRepository<Authentication>, IAuthenticationRepository
    {
        public AuthenticationRepository(FinCtrlAppDbContext context) : base(context) { }

        public async Task<Authentication> GetUserAuthenticationAsync(User user, string ip)
        {
            Dictionary<string, object> filter = new()
            {
                { nameof(Authentication.UserBsonId), user._id.ToString() },
                { nameof(Authentication.Ip), ip }
            };

            List<Authentication> userAuthentications = await GetByPropertiesAsync(filter);

            userAuthentications = userAuthentications
                .OrderByDescending(x => x.DateTime)
                .ToList();

            Authentication? lastAuthentication = userAuthentications.FirstOrDefault();

            if (lastAuthentication == null || !lastAuthentication.ValidTokenLifetime)
            {
                Authentication newAuthentication = new()
                {
                    DateTime = DateTime.UtcNow,
                    Ip = ip,
                    Token = TokenService.GenerateToken(user),
                    UserBsonId = user._id.ToString()
                };

                await InsertNewAsync(newAuthentication);

                return newAuthentication;
            }
            else
                return lastAuthentication;
        }
    }
}
