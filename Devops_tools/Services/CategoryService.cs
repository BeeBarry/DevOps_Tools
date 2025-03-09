using Devops_tools.Models.Domain;
using Devops_tools.Models.DTOs;
using Devops_tools.Repositories.Interfaces;
using Devops_tools.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Devops_tools.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CategoryService> _logger;
    
    public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }
    
    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        try
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(MapToCategoryDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fel vid hämtning av alla kategorier");
            return Enumerable.Empty<CategoryDto>();
        }
    }
    
    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category != null ? MapToCategoryDto(category) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fel vid hämtning av kategori med ID {Id}", id);
            return null;
        }
    }
    
    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesWithToolsAsync()
    {
        try
        {
            var categories = await _categoryRepository.GetAllWithToolsAsync();
            return categories.Select(MapToCategoryDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fel vid hämtning av kategorier med verktyg");
            return Enumerable.Empty<CategoryDto>();
        }
    }
    
    public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
    {
        try
        {
            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description
            };
            
            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();
            
            categoryDto.Id = category.Id;
            return categoryDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fel vid skapande av kategori: {Name}", categoryDto.Name);
            throw; // Låt anroparen hantera felet
        }
    }
    
    public async Task<CategoryDto?> UpdateCategoryAsync(CategoryDto categoryDto)
    {
        try
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(categoryDto.Id);
            if (existingCategory == null)
            {
                return null;
            }
            
            existingCategory.Name = categoryDto.Name;
            existingCategory.Description = categoryDto.Description;
            
            await _categoryRepository.UpdateAsync(existingCategory);
            await _categoryRepository.SaveChangesAsync();
            
            return MapToCategoryDto(existingCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fel vid uppdatering av kategori med ID {Id}", categoryDto.Id);
            throw;
        }
    }
    
    public async Task<bool> DeleteCategoryAsync(int id)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return false;
            }
            
            await _categoryRepository.DeleteAsync(category);
            await _categoryRepository.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fel vid borttagning av kategori med ID {Id}", id);
            return false;
        }
    }
    
    // Hjälpmetod för att mappa från domänmodell till DTO
    private CategoryDto MapToCategoryDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }
}