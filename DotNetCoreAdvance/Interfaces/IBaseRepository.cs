using DotNetCoreAdvance.DataContext;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreAdvance.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : class 
    {
        Task Add(TEntity entity);
        Task<bool> Update(TEntity entity);
        Task Delete(string id);
        Task<TEntity> Get(string id);
        Task<IEnumerable<TEntity>> GetAll();
        DbSet<TEntity> GetEntitySet();
    }
}
