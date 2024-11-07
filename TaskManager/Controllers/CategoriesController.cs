using Microsoft.AspNetCore.Mvc;
using TaskManager.Models;
using TaskManager.Repositories.Interfaces;
using TaskManager.UnitOfWork.Interfaces;
using TaskManager.ViewModels;

namespace TaskManager.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Models.Task> _taskRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IRepository<Category> categoryRepository,
            IRepository<Models.Task> taskRepository,
            IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        ///     Listar las categorías
        /// </summary>
        /// <returns>El listado de tareas.</returns>
        public async Task<IActionResult> Index()
        {
            var categories = _categoryRepository.GetAll();
            List<CategoryViewModel> categoriesViewModel = new();

            foreach (var category in categories)
            {
                categoriesViewModel.Add(new CategoryViewModel
                {
                    Id = category.Id,
                    Name = category.Name
                });
            }

            return View(categoriesViewModel);
        }

        /// <summary>
        ///     Vista para crear una categoría
        /// </summary>
        /// <returns>Vista de la creación de la categoría.</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        ///     Crea la categoría
        /// </summary>
        /// <param name="categoryVM">Devuelve la propia categoría en caso de haber errores de validación. De lo contrario, regresa al listado de categorías.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel categoryVM)
        {
            List<CategoryViewModel> categoriesViewModel = new();

            if (ModelState.IsValid)
            {
                try
                {
                    foreach (var category in _categoryRepository.GetAll())
                    {
                        categoriesViewModel.Add(new CategoryViewModel
                        {
                            Id = category.Id,
                            Name = category.Name
                        });
                    }

                    Category categoryNew = new Category
                    {
                        Name = categoryVM.Name,
                        Id = categoryVM.Id
                    };

                    _categoryRepository.Add(categoryNew);
                    await _unitOfWork.Save();

                    TempData["CategoriaCreada"] = "Categoría creada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error al crear la categoría (POST): {ex.Message}");
                }
            }

            return View(categoriesViewModel);
        }

        /// <summary>
        ///     Vista de la eliminación de la categoría
        /// </summary>
        /// <param name="id">Id de la categoría a eliminar</param>
        /// <returns>La categoría a eliminar</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            CategoryViewModel categoryVM = new();
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var category = await _categoryRepository.GetById(id);
                if (category == null)
                {
                    return NotFound();
                }

                categoryVM = new()
                {
                    Name = category.Name,
                    Id = category.Id
                };
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al eliminar la categoría (GET): {ex.Message}");
            }

            return View(categoryVM);
        }

        /// <summary>
        ///     Elimina una categoría
        /// </summary>
        /// <param name="id">Id de la categoría a eliminar</param>
        /// <returns>Regresa al listado de las categorías.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var tasks = _taskRepository.GetAll();
                var existsTaskInCategory = tasks.Where(task => task.CategoryId == id)
                    .ToList();

                if (existsTaskInCategory.Any())
                {
                    TempData["CategoriaEliminar"] = "No puedes eliminar la categoría, existen tareas asociadas.";
                    return RedirectToAction("Index");
                }

                var categoryDeleted = await _categoryRepository.Delete(id);
                if (categoryDeleted)
                {
                    TempData["CategoriaEliminada"] = "Categoría eliminada correctamente.";
                }
                else
                {
                    TempData["CategoriaEliminadaFail"] = "Fallo al eliminar la categoría.";
                }

                await _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al eliminar la categoría (POST ): {ex.Message}");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
