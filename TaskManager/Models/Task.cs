namespace TaskManager.Models;

public class Task : IBaseEntity
{
    public int Id { get; set; }

    public string? Title { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    public int? Priority { get; set; }
    
    public bool? IsCompleted { get; set; }
    
    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }
}
