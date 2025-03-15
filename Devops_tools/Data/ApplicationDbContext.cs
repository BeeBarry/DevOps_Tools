using Devops_tools.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Devops_tools.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Category> Categories { get; set; }
    public DbSet<Tool> Tools { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Minimal konfiguration för SQL Server Identity-kompatibilitet
        modelBuilder.Entity<IdentityUser>(entity => 
        {
            entity.Property(e => e.Id).HasMaxLength(450);
        });
        
        modelBuilder.Entity<IdentityRole>(entity => 
        {
            entity.Property(e => e.Id).HasMaxLength(450);
        });
        
        modelBuilder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.Property(e => e.UserId).HasMaxLength(450);
            entity.Property(e => e.RoleId).HasMaxLength(450);
        });
    }
}