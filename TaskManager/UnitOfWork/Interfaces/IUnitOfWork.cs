namespace TaskManager.UnitOfWork.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public Task<int> Save();
    }
}
