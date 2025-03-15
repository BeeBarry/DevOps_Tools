using Devops_tools.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Devops_tools.Data;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Docker
        UpsertTool(context, new Tool 
        { 
            Id = 1, 
            Name = "Docker", 
            Description = "Docker är en plattform för att utveckla, leverera och köra applikationer i containrar.",
            CategoryId = 1,
            LogoUrl = "https://api.iconify.design/skill-icons/docker.svg",
            LogoAltText = "Docker logotyp",
            GitHubUrl = "https://github.com/docker/docker-ce",
            OfficialUrl = "https://www.docker.com/",
            Features = "Containerisering;Image byggnad;Volume hantering;Nätverk"
        });
        
        // Podman
        UpsertTool(context, new Tool 
        { 
            Id = 2, 
            Name = "Podman", 
            Description = "Podman är ett daemonless container engine för att utveckla, hantera och köra OCI-containrar.",
            CategoryId = 1,
            LogoUrl = "https://api.iconify.design/logos/podman.svg",
            LogoAltText = "Podman logotyp",
            GitHubUrl = "https://github.com/containers/podman",
            OfficialUrl = "https://podman.io/",
            Features = "Rootless containers;Pod support;Systemd integration"
        });
        
        // Kubernetes
        UpsertTool(context, new Tool 
        { 
            Id = 3, 
            Name = "Kubernetes", 
            Description = "Kubernetes (K8s) är ett open-source system för automatisering av driftsättning, skalning och hantering av containeriserade applikationer.",
            CategoryId = 3, 
            LogoUrl = "https://api.iconify.design/skill-icons/kubernetes.svg",
            LogoAltText = "Kubernetes logotyp",
            GitHubUrl = "https://github.com/kubernetes/kubernetes",
            OfficialUrl = "https://kubernetes.io/",
            Features = "Container orkestrering;Auto-skalning;Service discovery;Load balancing"
        });
        
        // GitHub Actions
        UpsertTool(context, new Tool 
        { 
            Id = 4, 
            Name = "GitHub Actions", 
            Description = "GitHub Actions är ett CI/CD-verktyg som låter dig automatisera dina bygg-, test- och driftsättningsprocesser direkt från GitHub.",
            CategoryId = 2,
            LogoUrl = "https://api.iconify.design/skill-icons/github-light.svg",
            LogoAltText = "GitHub Actions logotyp",
            GitHubUrl = "https://github.com/features/actions",
            OfficialUrl = "https://github.com/features/actions",
            Features = "Workflow automation;CI/CD;Event-baserad exekvering;GitHub integration"
        });
        
        // GitLab CI/CD
        UpsertTool(context, new Tool 
        { 
            Id = 5, 
            Name = "GitLab CI/CD", 
            Description = "GitLab CI/CD är en inbyggd funktion i GitLab-plattformen som tillhandahåller kontinuerlig integration och driftsättning.",
            CategoryId = 2,
            LogoUrl = "https://about.gitlab.com/images/press/logo/svg/gitlab-logo-500.svg",
            LogoAltText = "GitLab logotyp",
            GitHubUrl = "https://github.com/gitlabhq/gitlabhq",
            OfficialUrl = "https://about.gitlab.com/",
            Features = "Pipelinehantering;Container Registry;CI/CD som kod;Auto DevOps"
        });
        
        context.SaveChanges();
    }
    
    private static void UpsertTool(ApplicationDbContext context, Tool toolData)
    {
        var existingTool = context.Tools.Find(toolData.Id);
        
        if (existingTool == null)
        {
            // Verktyget finns inte - skapa nytt
            context.Tools.Add(toolData);
        }
        else
        {
            // Verktyget finns - uppdatera alla fält
            existingTool.Name = toolData.Name;
            existingTool.Description = toolData.Description;
            existingTool.CategoryId = toolData.CategoryId;
            existingTool.LogoUrl = toolData.LogoUrl;
            existingTool.LogoAltText = toolData.LogoAltText;
            existingTool.GitHubUrl = toolData.GitHubUrl;
            existingTool.OfficialUrl = toolData.OfficialUrl;
            existingTool.Features = toolData.Features;
        }
    }
}