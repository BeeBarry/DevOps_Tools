using System.ComponentModel.DataAnnotations;

namespace Devops_tools.Models.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
        
    [Required(ErrorMessage = "Kategorinamnet är obligatoriskt")]
    [StringLength(100, ErrorMessage = "Kategorinamnet får inte vara längre än 100 tecken")]
    public string Name { get; set; } = string.Empty;
        
    [StringLength(500, ErrorMessage = "Beskrivningen får inte vara längre än 500 tecken")]
    public string? Description { get; set; }
}