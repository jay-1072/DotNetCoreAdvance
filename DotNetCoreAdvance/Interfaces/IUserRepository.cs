using DotNetCoreAdvance.Models;

namespace DotNetCoreAdvance.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<bool> IsUserExist(string userId);
    }
}