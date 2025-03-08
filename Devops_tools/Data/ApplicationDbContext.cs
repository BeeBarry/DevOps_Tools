using Devops_tools.Models.Domain;
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
            
            // Konfigurationer för Category
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Tools)
                .WithOne(t => t.Category)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Seed-data för kategorier
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Containers", Description = "Verktyg för containerisering och container-hantering" },
                new Category { Id = 2, Name = "CI/CD", Description = "Verktyg för kontinuerlig integration och leverans" },
                new Category { Id = 3, Name = "Orkestrering", Description = "Verktyg för container-orkestrering och skalning" }
            );
            
            // Seed-data för verktyg (några exempel)
            modelBuilder.Entity<Tool>().HasData(
                new Tool 
                { 
                    Id = 1, 
                    Name = "Docker", 
                    Description = "Docker är en plattform för att utveckla, leverera och köra applikationer i containrar.",
                    CategoryId = 1,
                    LogoUrl = "/images/docker-logo.png",
                    LogoAltText = "Docker logotyp",
                    GitHubUrl = "https://github.com/docker/docker-ce",
                    OfficialUrl = "https://www.docker.com/",
                    Features = "Containerisering;Image byggnad;Volume hantering;Nätverk",
                    
                },
                new Tool 
                { 
                    Id = 2, 
                    Name = "Podman", 
                    Description = "Podman är ett daemonless container engine för att utveckla, hantera och köra OCI-containrar.",
                    CategoryId = 1,
                    LogoUrl = "/images/podman-logo.png",
                    LogoAltText = "Podman logotyp",
                    GitHubUrl = "https://github.com/containers/podman",
                    OfficialUrl = "https://podman.io/",
                    Features = "Rootless containers;Pod support;Systemd integration",
                    
                }
            );
        }
    }