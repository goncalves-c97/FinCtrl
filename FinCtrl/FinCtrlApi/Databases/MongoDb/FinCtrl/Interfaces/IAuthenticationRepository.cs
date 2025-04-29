using FinCtrlLibrary.Models;

namespace FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces
{
    public interface IAuthenticationRepository : IGenericRepository<Authentication>
    {
        /// <summary>
        /// Get a valid authentication for an user. If it still has a valid authentication
        /// it returns it, if it doesn't, it creates a new one and returns it
        /// </summary>
        /// <param name="user">User to create authentication</param>
        /// <returns>A valid user authentication</returns>
        public Task<Authentication> GetUserAuthenticationAsync(User user, string ip);
    }
}
