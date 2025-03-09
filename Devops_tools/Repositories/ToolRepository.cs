using Devops_tools.Data;
using Devops_tools.Models.Domain;
using Devops_tools.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Devops_tools.Repositories;

public class ToolRepository : BaseRepository<Tool>, IToolRepository
{
    public ToolRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Tool?> GetWithCategoryAsync(int id)
    {
        return await _dbSet
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Tool>> GetByCategoryAsync(int categoryId)
    {
        return await _dbSet
            .Where(t => t.CategoryId == categoryId)
            .Include(t => t.Category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tool>> GetAllWithCategoryAsync()
    {
        return await _dbSet
            .Include(t => t.Category)
            .ToListAsync();
    }
}