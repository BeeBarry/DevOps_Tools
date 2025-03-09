using Devops_tools.Models.Domain;

namespace Devops_tools.Repositories.Interfaces;

public interface IToolRepository : IRepository<Tool>
{
    // Specifika metoder för Tool
    Task<Tool?> GetWithCategoryAsync(int id);
    Task<IEnumerable<Tool>> GetByCategoryAsync(int categoryId);
    Task<IEnumerable<Tool>> GetAllWithCategoryAsync();
}