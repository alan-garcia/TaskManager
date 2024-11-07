using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Models;
using TaskManager.Repositories.Interfaces;
using TaskManager.UnitOfWork.Interfaces;
using TaskManager.ViewModels;

namespace TaskManager.Controllers
{
    public class TasksController : Controller
    {
        private readonly IRepository<Models.Task> _taskRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TasksController(IRepository<Models.Task> taskRepository,
            IRepository<Category> categoryRepository,
            IUnitOfWork unitOfWork)
        {
            _taskRepository = taskRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        ///     Listar las tareas
        /// </summary>
        /// <param name="categoryId">Id de la categoría a la que pertenece la tarea.</param>
        /// <param name="isCompleted">Indica si la tarea esta completada o no.</param>
        /// <returns>El listado de tareas.</returns>
        public async Task<IActionResult> Index(int? categoryId, bool? isCompleted)
        {
            var tasksVM = new List<TaskViewModel>();
            ViewBag.Categories = _categoryRepository.GetAll();
            
            var tasksFiltered = FilterTasks(categoryId, isCompleted)
                .ToList();
            if (!tasksFiltered.Any())
            {
                TempData["TareasFilterVacias"] = "No hay tareas que cumplan con este filtro.";
            }
            else
            {
                tasksVM = tasksFiltered?.Select(task => new TaskViewModel
                {
                    Title = task.Title,
                    Description = task.Description,
                    Id = task.Id,
                    DueDate = task.DueDate,
                    Priority = task.Priority,
                    IsCompleted = task.IsCompleted,
                    Category = task.Category,
                    CategoryId = task.CategoryId
                }).ToList();
            }

            ViewData["SelectedCategoryId"] = categoryId;
            ViewData["SelectedIsCompleted"] = isCompleted;

            return View(tasksVM);
        }

        /// <summary>
        ///     Desplegable para filtrar las tareas por categoría y estados
        /// </summary>
        /// <param name="categoryId">Filtra por el Id de la categoría.</param>
        /// <param name="isCompleted">Indica si la tarea está o no completada.</param>
        /// <returns></returns>
        [HttpPost]
        public IQueryable<Models.Task> FilterTasks(int? categoryId, bool? isCompleted)
        {
            var tasks = _taskRepository.GetAll();
            try
            {
                tasks = tasks.Include(task => task.Category)
                    .AsQueryable();

                if (categoryId.HasValue)
                {
                    tasks = tasks.Where(task => task.CategoryId == categoryId.Value);
                }

                if (isCompleted.HasValue)
                {
                    tasks = tasks.Where(task => task.IsCompleted == isCompleted.Value);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al filtrar el listado de tareas por categoría y estados: {ex.Message}");
            }

            return tasks;
        }

        /// <summary>
        ///     Ver una tarea
        /// </summary>
        /// <param name="id">Id de la tarea</param>
        /// <returns>La tarea correspondiente a su Id</returns>
        public async Task<IActionResult> Details(int? id, TaskViewModel taskVM)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var task = await _taskRepository.GetAll()
                .Include(task => task.Category)
                .FirstOrDefaultAsync(task => task.Id == id);

                if (task == null)
                {
                    return NotFound();
                }

                taskVM.Title = task.Title;
                taskVM.Description = task.Description;
                taskVM.Id = task.Id;
                taskVM.DueDate = task.DueDate;
                taskVM.Priority = task.Priority;
                taskVM.IsCompleted = task.IsCompleted ?? false;
                taskVM.Category = task.Category;
                taskVM.CategoryId = task.CategoryId;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en la pantalla de 'Ver' tareas: {ex.Message}");
            }

            return View(taskVM);
        }

        /// <summary>
        ///     Vista para crear una tarea
        /// </summary>
        /// <returns>Vista de la creación de la tarea.</returns>
        public IActionResult Create()
        {
            var taskVM = new TaskViewModel
            {
                IsCompleted = false
            };

            var categories = _categoryRepository.GetAll();
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");

            return View(taskVM);
        }

        /// <summary>
        ///     Crea la tarea
        /// </summary>
        /// <param name="taskVM">Toda la información de la tarea recogida del formulario de creación</param>
        /// <returns>Devuelve la propia tarea en caso de haber errores de validación. De lo contrario, regresa al listado de tareas.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskViewModel taskVM)
        {
            List<TaskViewModel> tasksViewModel = new();
            var categories = _categoryRepository.GetAll();
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name", taskVM.CategoryId);

            if (ModelState.IsValid)
            {
                try
                {
                    Models.Task taskNew = new()
                    {
                        Title = taskVM.Title,
                        Description = taskVM.Description,
                        Id = taskVM.Id,
                        DueDate = taskVM.DueDate,
                        Priority = taskVM.Priority,
                        IsCompleted = taskVM.IsCompleted,
                        Category = taskVM.Category,
                        CategoryId = taskVM.CategoryId
                    };

                    _taskRepository.Add(taskNew);
                    await _unitOfWork.Save();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error al crear la tarea: {ex.Message}");
                }

                TempData["TareaCreada"] = "Tarea creada correctamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(tasksViewModel);
        }

        /// <summary>
        ///     Vista de la modificación de una tarea
        /// </summary>
        /// <param name="id">Id de la tarea a modificar</param>
        /// <returns>La tarea asociada a su Id</returns>
        public async Task<IActionResult> Edit(int? id, TaskViewModel taskVM)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var task = await _taskRepository.GetById(id);
                if (task == null)
                {
                    return NotFound();
                }

                var categories = _categoryRepository.GetAll();
                ViewData["CategoryId"] = new SelectList(categories, "Id", "Name", task.CategoryId);

                taskVM.Title = task.Title;
                taskVM.Description = task.Description;
                taskVM.Id = task.Id;
                taskVM.DueDate = task.DueDate;
                taskVM.Priority = task.Priority;
                taskVM.IsCompleted = task.IsCompleted ?? false;
                taskVM.Category = task.Category;
                taskVM.CategoryId = task.CategoryId;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al modificar la tarea (GET): {ex.Message}");
            }

            return View(taskVM);
        }

        /// <summary>
        ///     Modifica la tarea
        /// </summary>
        /// <param name="id">Id de la tarea a modificar</param>
        /// <param name="taskVM">Toda la información de la tarea recogida del formulario de la modificación</param>
        /// <returns>Devuelve la propia tarea en caso de haber errores de validación. De lo contrario, regresa al listado de tareas.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskViewModel taskVM)
        {
            try
            {
                if (id != taskVM.Id)
                {
                    return NotFound();
                }

                var categories = _categoryRepository.GetAll();
                ViewData["CategoryId"] = new SelectList(categories, "Id", "Name", taskVM.CategoryId);

                taskVM.IsCompleted ??= false;

                if (ModelState.IsValid)
                {
                    try
                    {
                        // Validar que al actualizar una tarea a IsCompleted, la fecha de finalización (DueDate) sea pasada.
                        if (taskVM.IsCompleted == true && taskVM.DueDate > DateTime.Now)
                        {
                            ModelState.AddModelError("DueDate", "La fecha de finalización debe ser una fecha pasada.");
                            return View(taskVM);
                        }

                        Models.Task task = new()
                        {
                            Category = taskVM.Category,
                            CategoryId = taskVM.CategoryId,
                            IsCompleted = taskVM.IsCompleted,
                            Description = taskVM.Description,
                            DueDate = taskVM.DueDate,
                            Id = taskVM.Id,
                            Priority = taskVM.Priority,
                            Title = taskVM.Title
                        };

                        _taskRepository.Update(task);
                        await _unitOfWork.Save();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                    }

                    TempData["TareaModificada"] = "Tarea modificada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al modificar la tarea (POST): {ex.Message}");
            }

            return View(taskVM);
        }

        /// <summary>
        ///     Vista de la eliminación de la tarea
        /// </summary>
        /// <param name="id">Id de la tarea a eliminar</param>
        /// <returns>La tarea a eliminar</returns>
        public async Task<IActionResult> Delete(int? id, TaskViewModel taskVM)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var task = await _taskRepository.GetAll()
                    .Include(task => task.Category)
                    .FirstOrDefaultAsync(task => task.Id == id);

                if (task == null)
                {
                    return NotFound();
                }

                taskVM.Title = task.Title;
                taskVM.Description = task.Description;
                taskVM.Id = task.Id;
                taskVM.DueDate = task.DueDate;
                taskVM.Priority = task.Priority;
                taskVM.IsCompleted = task.IsCompleted ?? false;
                taskVM.Category = task.Category;
                taskVM.CategoryId = task.CategoryId;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al eliminar la tarea (GET): {ex.Message}");
            }

            return View(taskVM);
        }

        /// <summary>
        ///     Elimina una tarea
        /// </summary>
        /// <param name="id">Id de la tarea a eliminar</param>
        /// <returns>Regresa al listado de las tareas.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var taskDeleted = await _taskRepository.Delete(id);
                if (taskDeleted)
                {
                    TempData["TareaEliminada"] = "Tarea eliminada correctamente.";
                }
                else
                {
                    TempData["TareaEliminadaFail"] = "Fallo al eliminar la tarea.";
                }

                await _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al eliminar la tarea (POST): {ex.Message}");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
