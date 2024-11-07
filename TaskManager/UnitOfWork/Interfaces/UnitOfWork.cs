using TaskManager.Models;

namespace TaskManager.UnitOfWork.Interfaces
{
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        ///     Aplicar el guardado de los cambios en la misma unidad de trabajo
        /// </summary>
        private readonly TaskmanagerContext _context;

        public UnitOfWork(TaskmanagerContext context)
        {
            _context = context;
        }

        public async Task<int> Save()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
