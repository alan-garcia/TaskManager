using Microsoft.EntityFrameworkCore;
using TaskManager.Models;
using TaskManager.Repositories.Interfaces;

namespace TaskManager.Repositories.Impl
{
    public class Repository<T> : IRepository<T> where T : class, IBaseEntity
    {
        public readonly TaskmanagerContext _context;
        public DbSet<T> Entity => _context.Set<T>();

        public Repository(TaskmanagerContext context)
        {
            _context = context;
        }

        public IQueryable<T> GetAll()
        {
            return Entity.AsNoTracking();
        }

        public async Task<T?> GetById(int? id)
        {
            return await Entity.AsNoTracking().FirstOrDefaultAsync(entity => entity.Id == id);
        }

        public void Add(T entity)
        {
            Entity.Add(entity);
        }

        public void Update(T entity)
        {
            Entity.Update(entity);
        }

        public async Task<bool> Delete(int? id)
        {
            var entity = await Entity.FindAsync(id);
            if (entity != null)
            {
                Entity.Remove(entity);
                return true;
            }

            return false;
        }
    }
}
