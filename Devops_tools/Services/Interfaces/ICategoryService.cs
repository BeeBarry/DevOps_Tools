using Devops_tools.Models.DTOs;

namespace Devops_tools.Services.Interfaces;

public interface ICategoryService
{
    /// <summary>
    /// Hämtar alla kategorier
    /// </summary>
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    
    /// <summary>
    /// Hämtar en kategori med dess ID
    /// </summary>
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
    
    /// <summary>
    /// Hämtar alla kategorier med tillhörande verktyg
    /// </summary>
    Task<IEnumerable<CategoryDto>> GetAllCategoriesWithToolsAsync();
    
    /// <summary>
    /// Skapar en ny kategori
    /// </summary>
    Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto);
    
    /// <summary>
    /// Uppdaterar en befintlig kategori
    /// </summary>
    Task<CategoryDto?> UpdateCategoryAsync(CategoryDto categoryDto);
    
    /// <summary>
    /// Tar bort en kategori
    /// </summary>
    Task<bool> DeleteCategoryAsync(int id);
}