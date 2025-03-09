using Devops_tools.Models.DTOs;

namespace Devops_tools.Services.Interfaces;

public interface IToolService
{
    /// <summary>
    /// Hämtar alla verktyg med GitHub-information
    /// </summary>
    Task<IEnumerable<ToolDto>> GetAllToolsAsync();
    
    /// <summary>
    /// Hämtar ett verktyg med dess ID
    /// </summary>
    Task<ToolDto?> GetToolByIdAsync(int id);
    
    /// <summary>
    /// Hämtar alla verktyg för en specifik kategori
    /// </summary>
    Task<IEnumerable<ToolDto>> GetToolsByCategoryAsync(int categoryId);
    
    /// <summary>
    /// Skapar ett nytt verktyg
    /// </summary>
    Task<ToolDto> CreateToolAsync(ToolDto toolDto);
    
    /// <summary>
    /// Uppdaterar ett befintligt verktyg
    /// </summary>
    Task<ToolDto?> UpdateToolAsync(ToolDto toolDto);
    
    /// <summary>
    /// Tar bort ett verktyg
    /// </summary>
    Task<bool> DeleteToolAsync(int id);
    
    /// <summary>
    /// Parsar features från semikolon-separerad text till lista
    /// </summary>
    List<string>? ParseFeatures(string? features);
    
    /// <summary>
    /// Sammanfogar features från lista till semikolon-separerad text
    /// </summary>
    string? JoinFeatures(List<string>? features);
}