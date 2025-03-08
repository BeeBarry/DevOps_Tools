using System.ComponentModel.DataAnnotations;

namespace Devops_tools.Models.DTOs;

public class ToolDto
{
    public int Id { get; set; }
        
    [Required(ErrorMessage = "Verktygsnamnet är obligatoriskt")]
    [StringLength(100, ErrorMessage = "Verktygsnamnet får inte vara längre än 100 tecken")]
    public string Name { get; set; } = string.Empty;
        
    [Required(ErrorMessage = "Beskrivningen är obligatorisk")]
    [StringLength(1000, ErrorMessage = "Beskrivningen får inte vara längre än 1000 tecken")]
    public string Description { get; set; } = string.Empty;
        
    [Required(ErrorMessage = "Kategori måste väljas")]
    public int CategoryId { get; set; }
        
    public string? CategoryName { get; set; }
        
    [StringLength(255, ErrorMessage = "Logo-URL får inte vara längre än 255 tecken")]
    public string? LogoUrl { get; set; }
        
    [Required(ErrorMessage = "Alt-text för logotyp är obligatorisk")]
    [StringLength(255, ErrorMessage = "Alt-text får inte vara längre än 255 tecken")]
    public string LogoAltText { get; set; } = string.Empty;
        
    [StringLength(255, ErrorMessage = "GitHub-URL får inte vara längre än 255 tecken")]
    public string? GitHubUrl { get; set; }
        
    [StringLength(255, ErrorMessage = "Officiell URL får inte vara längre än 255 tecken")]
    public string? OfficialUrl { get; set; }
        
    public List<string>? Features { get; set; }
    
    // GitHub-relaterad data som kan hämtas via API
    public int? GitHubStars { get; set; }
}