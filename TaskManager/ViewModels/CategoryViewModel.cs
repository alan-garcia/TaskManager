using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [DisplayName("Nombre")]
        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [StringLength(100, ErrorMessage = "La categoría no puede exceder de 100 caracteres")]
        public string? Name { get; set; }
    }
}
