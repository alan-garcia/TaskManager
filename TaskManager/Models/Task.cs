using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models;

public class Task
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título de la tarea es obligatorio.")]
    [StringLength(100, ErrorMessage = "El título no puede exceder de 100 caracteres.")]
    public string? Title { get; set; }

    public string? Description { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? DueDate { get; set; }

    [Range(1, 5, ErrorMessage = "La prioridad debe estar entre 1 y 5.")]
    public int? Priority { get; set; }

    public bool? IsCompleted { get; set; }

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }
}
