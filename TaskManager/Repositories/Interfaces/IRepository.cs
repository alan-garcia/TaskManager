namespace TaskManager.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        public IQueryable<T> GetAll();
        public Task<T?> GetById(int? id);
        public void Add(T entity);
        public void Update(T entity);
        public Task<bool> Delete(int? id);
    }
}
