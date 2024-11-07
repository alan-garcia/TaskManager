using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TaskManager.Models;

namespace TaskManager.ViewModels
{
    public class TaskViewModel
    {
        public int Id { get; set; }

        [DisplayName("Título")]
        [Required(ErrorMessage = "El título de la tarea es obligatorio.")]
        [StringLength(100, ErrorMessage = "El título no puede exceder de 100 caracteres.")]
        public string? Title { get; set; }

        [DisplayName("Descripción")]
        public string? Description { get; set; }

        [DisplayName("Fecha estimada")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [DisplayName("Prioridad")]
        [Range(1, 5, ErrorMessage = "La prioridad debe estar entre 1 y 5.")]
        public int? Priority { get; set; }

        [DisplayName("¿Completada?")]
        public bool? IsCompleted { get; set; } = false;

        [DisplayName("Categoría")]
        public int? CategoryId { get; set; }

        public virtual Category? Category { get; set; }
    }
}
