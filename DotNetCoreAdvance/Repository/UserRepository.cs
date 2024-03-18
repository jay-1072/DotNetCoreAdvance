using DotNetCoreAdvance.DataContext;
using DotNetCoreAdvance.Interfaces;
using DotNetCoreAdvance.Models;

namespace DotNetCoreAdvance.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository 
    {
        public UserRepository(DatabaseContext databaseContext) : base(databaseContext)
        {
        }

        public async Task<bool> IsUserExist(string userId)
        {
            var user = await (this.GetEntitySet().FindAsync(userId));
            return user != null;
        }
    }
}
