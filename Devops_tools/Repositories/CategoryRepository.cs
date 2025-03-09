using Devops_tools.Data;
using Devops_tools.Models.Domain;
using Devops_tools.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Devops_tools.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetWithToolsAsync(int id)
    {
        return await _dbSet
            .Include(c => c.Tools)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Category>> GetAllWithToolsAsync()
    {
        return await _dbSet
            .Include(c => c.Tools)
            .ToListAsync();
    }
}