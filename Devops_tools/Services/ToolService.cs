using Devops_tools.Models.Domain;
using Devops_tools.Models.DTOs;
using Devops_tools.Repositories.Interfaces;
using Devops_tools.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Devops_tools.Services;

public class ToolService : IToolService
{
    private readonly IToolRepository _toolRepository;
    private readonly IGitHubService _gitHubService;
    private readonly ILogger<ToolService> _logger;
    
    public ToolService(
        IToolRepository toolRepository, 
        IGitHubService gitHubService, 
        ILogger<ToolService> logger)
    {
        _toolRepository = toolRepository;
        _gitHubService = gitHubService;
        _logger = logger;
    }
    
    public async Task<IEnumerable<ToolDto>> GetAllToolsAsync()
    {
        try
        {
            var tools = await _toolRepository.GetAllWithCategoryAsync();
            var toolDtos = tools.Select(t => MapToToolDto(t)).ToList();
            
            // Berika med GitHub-data asynkront
            var enrichmentTasks = toolDtos.Select(EnrichWithGitHubDataAsync);
            await Task.WhenAll(enrichmentTasks);
            
            return toolDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fel vid hämtning av alla verktyg");
            return Enumerable.Empty<ToolDto>();
        }
    }
    
    public async Task<ToolDto?> GetToolByIdAsync(int id)
    {
        try
        {
            var tool = await _toolRepository.GetWithCategoryAsync(id);
            if (tool == null)
            {
                return null;
            }
            
            var toolDto = MapToToolDto(tool);
            await EnrichWithGitHubDataAsync(toolDto);
            
            return toolDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fel vid hämtning av verktyg med ID {Id}", id);
            return null;
        }
    }
    
    public async Task<IEnumerable<ToolDto>> GetToolsByCategoryAsync(int categoryId)
    {
        try
        {
            var tools = await _toolRepository.GetByCategoryAsync(categoryId);
            var toolDtos = tools.Select(t => MapToToolDto(t)).ToList();
            
            // Berika med GitHub-data asynkront
            var enrichmentTasks = toolDtos.Select(EnrichWithGitHubDataAsync);
            await Task.WhenAll(enrichmentTasks);
            
            return toolDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fel vid hämtning av verktyg för kategori {CategoryId}", categoryId);
            return Enumerable.Empty<ToolDto>();
        }
    }
    
    public async Task<ToolDto> CreateToolAsync(ToolDto toolDto)
    {
        try
        {
            var tool = new Tool
            {
                Name = toolDto.Name,
                Description = toolDto.Description,
                CategoryId = toolDto.CategoryId,
                LogoUrl = toolDto.LogoUrl,
                LogoAltText = toolDto.LogoAltText,
                GitHubUrl = toolDto.GitHubUrl,
                OfficialUrl = toolDto.OfficialUrl,
                Features = JoinFeatures(toolDto.Features)
            };
            
            await _toolRepository.AddAsync(tool);
            await _toolRepository.SaveChangesAsync();
            
            toolDto.Id = tool.Id;
            return toolDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fel vid skapande av verktyg: {Name}", toolDto.Name);
            throw;
        }
    }
    
    public async Task<ToolDto?> UpdateToolAsync(ToolDto toolDto)
    {
        try
        {
            var existingTool = await _toolRepository.GetByIdAsync(toolDto.Id);
            if (existingTool == null)
            {
                return null;
            }
            
            existingTool.Name = toolDto.Name;
            existingTool.Description = toolDto.Description;
            existingTool.CategoryId = toolDto.CategoryId;
            existingTool.LogoUrl = toolDto.LogoUrl;
            existingTool.LogoAltText = toolDto.LogoAltText;
            existingTool.GitHubUrl = toolDto.GitHubUrl;
            existingTool.OfficialUrl = toolDto.OfficialUrl;
            existingTool.Features = JoinFeatures(toolDto.Features);
            
            await _toolRepository.UpdateAsync(existingTool);
            await _toolRepository.SaveChangesAsync();
            
            var updatedDto = MapToToolDto(existingTool);
            await EnrichWithGitHubDataAsync(updatedDto);
            
            return updatedDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fel vid uppdatering av verktyg med ID {Id}", toolDto.Id);
            throw;
        }
    }
    
    public async Task<bool> DeleteToolAsync(int id)
    {
        try
        {
            var tool = await _toolRepository.GetByIdAsync(id);
            if (tool == null)
            {
                return false;
            }
            
            await _toolRepository.DeleteAsync(tool);
            await _toolRepository.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fel vid borttagning av verktyg med ID {Id}", id);
            return false;
        }
    }
    
    public List<string>? ParseFeatures(string? features)
    {
        if (string.IsNullOrEmpty(features))
        {
            return null;
        }
        
        return features.Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(f => f.Trim())
            .Where(f => !string.IsNullOrEmpty(f))
            .ToList();
    }
    
    public string? JoinFeatures(List<string>? features)
    {
        if (features == null || !features.Any())
        {
            return null;
        }
        
        return string.Join(";", features);
    }
    
    // Hjälpmetod för att mappa från domänmodell till DTO
    private ToolDto MapToToolDto(Tool tool)
    {
        return new ToolDto
        {
            Id = tool.Id,
            Name = tool.Name,
            Description = tool.Description,
            CategoryId = tool.CategoryId,
            CategoryName = tool.Category?.Name,
            LogoUrl = tool.LogoUrl,
            LogoAltText = tool.LogoAltText,
            GitHubUrl = tool.GitHubUrl,
            OfficialUrl = tool.OfficialUrl,
            Features = ParseFeatures(tool.Features)
        };
    }
    
    // Hjälpmetod för att berika DTO med GitHub-data
    private async Task EnrichWithGitHubDataAsync(ToolDto toolDto)
    {
        if (!string.IsNullOrEmpty(toolDto.GitHubUrl))
        {
            try
            {
                toolDto.GitHubStars = await _gitHubService.GetRepositoryStarsAsync(toolDto.GitHubUrl);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Kunde inte hämta GitHub-stjärnor för {ToolName}", toolDto.Name);
            }
        }
    }
}