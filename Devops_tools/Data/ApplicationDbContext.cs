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
    
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Tool> Tools { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Grundläggande konfiguration - låt EF hantera ID-generering
        modelBuilder.Entity<Category>(entity => 
        {
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();
        });
            
        modelBuilder.Entity<Tool>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();
        });
        
        // Definiera relationen mellan Tool och Category
        modelBuilder.Entity<Tool>()
            .HasOne(t => t.Category)
            .WithMany(c => c.Tools)
            .HasForeignKey(t => t.CategoryId);
        
        // Identity-konfiguration
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