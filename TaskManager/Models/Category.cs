using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models;

public class Category
{
    public int Id { get; set; }

    [StringLength(100, ErrorMessage = "La categoría no puede exceder de 100 caracteres")]
    public string? Name { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
