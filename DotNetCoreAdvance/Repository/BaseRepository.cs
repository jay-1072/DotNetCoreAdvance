using DotNetCoreAdvance.DataContext;
using DotNetCoreAdvance.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreAdvance.Repository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly DatabaseContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public BaseRepository(DatabaseContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task Add(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(string id)
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<TEntity> Get(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public DbSet<TEntity> GetEntitySet()
        {
            return _dbSet;
        }

        public async Task<bool> Update(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            var res = await _context.SaveChangesAsync();
           
            return res > 0;
        }
    }
}