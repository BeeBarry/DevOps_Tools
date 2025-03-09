using Devops_tools.Models.Domain;

namespace Devops_tools.Repositories.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    // Specifika metoder för Category
    Task<Category?> GetWithToolsAsync(int id);
    Task<IEnumerable<Category>> GetAllWithToolsAsync();
}