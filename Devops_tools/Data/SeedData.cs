using Devops_tools.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Devops_tools.Data;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Uppdatera eller skapa kategorier
        UpsertCategories(context);
        
        // Uppdatera eller skapa verktyg
        UpsertTools(context);
    }

    private static void UpsertCategories(ApplicationDbContext context)
    {
        var categoriesToUpsert = new[]
        {
            new Category { Name = "Programming Languages", Description = "Programmeringsspråk som är särskilt användbara inom DevOps för att bygga och automatisera moderna system med bättre skalbarhet och säkerhet." },
            new Category { Name = "Cloud Platforms", Description = "Molnbaserade plattformar som erbjuder skalbar infrastruktur och tjänster för att utveckla, driftsätta och hantera moderna applikationer." },
            new Category { Name = "CI/CD", Description = "Verktyg som automatiserar bygg-, test- och driftsättningsprocesser för att leverera kod snabbare och mer tillförlitligt." },
            new Category { Name = "Containers", Description = "Verktyg för att paketera och köra applikationer i isolerade miljöer, vilket gör det möjligt att köra samma kod konsekvent oavsett infrastruktur." },
            new Category { Name = "Orchestration", Description = "Verktyg för att hantera, skala och övervaka containerbaserade applikationer i produktionsmiljö på ett automatiserat sätt." },
            new Category { Name = "Infrastructure as Code", Description = "Verktyg som låter dig hantera infrastruktur genom kod istället för manuell konfiguration, vilket ger repeterbarhet och bättre kontroll." }
        };
        
        foreach (var category in categoriesToUpsert)
        {
            var existingCategory = context.Categories.FirstOrDefault(c => c.Name == category.Name);
            
            if (existingCategory == null)
            {
                // Skapa ny kategori
                context.Categories.Add(category);
            }
            else
            {
                // Uppdatera befintlig kategori
                existingCategory.Description = category.Description;
            }
        }
        
        context.SaveChanges();
    }
    
    private static void UpsertTools(ApplicationDbContext context)
    {
        // Ladda alla kategorier för att minska databasanrop
        var allCategories = context.Categories.ToDictionary(c => c.Name, c => c);
        
        // === Programming Languages ===
        
        // C#
        UpsertTool(context, allCategories, 
            "C#", 
            "Programming Languages",
            "C# är ett mångsidigt objektorienterat språk från Microsoft som passar perfekt för DevOps i Windows-miljöer och molninfrastruktur baserad på Azure, med sömlös integration mot .NET-ekosystemet.",
            "https://api.iconify.design/skill-icons/cs.svg",
            "C# logotyp",
            "https://github.com/dotnet/csharplang",
            "https://dotnet.microsoft.com/languages/csharp",
            "Tight integration med Azure och Windows-miljöer;Smidig övergång från utveckling till drift med .NET;Utmärkt stöd för asynkron programmering;Perfekt för att bygga API:er och microservices;Kan köras cross-platform genom .NET Core/5+");
        
        // Python
        UpsertTool(context, allCategories, 
            "Python", 
            "Programming Languages",
            "Python är ett lättläst språk med minimalistisk syntax som blivit något av en standard för automation och skriptning inom DevOps, tack vare sitt rika ekosystem av bibliotek och verktyg.",
            "https://api.iconify.design/skill-icons/python-light.svg",
            "Python logotyp",
            "https://github.com/python/cpython",
            "https://www.python.org/",
            "Enkel syntax som gör automation begriplig;Bred användning i CI/CD-pipelines och deployment-skript;Kraftfulla bibliotek som Ansible och Fabric;Utmärkt för att bearbeta och analysera driftdata;Väl integrerat med de flesta molnleverantörers verktyg");
        
        // Go
        UpsertTool(context, allCategories, 
            "Go", 
            "Programming Languages",
            "Go är ett rakt-på-sak språk utvecklat av Google som kombinerar enkelhet med hög prestanda, vilket gjort det till ett förstahandsval för många containerbaserade DevOps-verktyg.",
            "https://api.iconify.design/skill-icons/golang.svg",
            "Go logotyp",
            "https://github.com/golang/go",
            "https://golang.org/",
            "Enkelhet som gör koden lättunderhållen;Kompilerar direkt till fristående binärfiler utan beroenden;Inbyggt stöd för parallell exekvering;Grunden för Docker, Kubernetes och andra viktiga DevOps-verktyg;Bra prestanda för tjänster med hög belastning");
        
        // === Cloud Platforms ===
        
        // AWS
        UpsertTool(context, allCategories,
            "AWS", 
            "Cloud Platforms",
            "Amazon Web Services (AWS) är en molnplattform som erbjuder över 200 tjänster från datacenter globalt, med marknadsledande bredd och djup av funktionalitet.",
            "https://api.iconify.design/logos/aws.svg",
            "AWS logotyp",
            null,
            "https://aws.amazon.com/",
            "Omfattande tjänsteutbud för alla behov;Global infrastruktur med hög tillgänglighet;Branschledande lagrings- och databastjänster;Avancerade säkerhets- och identitetsverktyg;Robust serverless-funktionalitet");
        
        // Azure
        UpsertTool(context, allCategories,
            "Microsoft Azure", 
            "Cloud Platforms",
            "Microsoft Azure är en molnplattform med stark integration mot Microsofts ekosystem, vilket förenklar övergången till molnet för Windows-baserade organisationer.",
            "https://api.iconify.design/logos/microsoft-azure.svg",
            "Microsoft Azure logotyp",
            null,
            "https://azure.microsoft.com/",
            "Sömlös integration med Microsoft-produkter;Kraftfulla hybridmolnlösningar;Överlägsen identitetshantering via Azure AD;Omfattande DevOps-verktyg;Stark .NET och Windows-support");
        
        // GCP
        UpsertTool(context, allCategories,
            "Google Cloud Platform", 
            "Cloud Platforms",
            "Google Cloud Platform (GCP) erbjuder molntjänster byggda på Googles infrastruktur, med särskilda styrkor inom dataanalys, container-teknologi och AI.",
            "https://api.iconify.design/logos/google-cloud.svg",
            "Google Cloud Platform logotyp",
            null,
            "https://cloud.google.com/",
            "Världsledande container-orkestrering med GKE;Högpresterande databehandling med BigQuery;Avancerade AI/ML-verktyg;Globalt höghastighetsnätverk;Innovativa serverless-lösningar");
        
        // === CI/CD ===
        
        // GitHub Actions
        UpsertTool(context, allCategories,
            "GitHub Actions", 
            "CI/CD",
            "GitHub Actions är ett CI/CD-verktyg som låter dig automatisera bygg-, test- och driftsättningsprocesser direkt från GitHub, utan behov av externa verktyg.",
            "https://api.iconify.design/skill-icons/github-light.svg",
            "GitHub Actions logotyp",
            "https://github.com/features/actions",
            "https://github.com/features/actions",
            "Sömlös GitHub-integration;Omfattande marketplace med färdiga actions;Enkel YAML-baserad konfiguration;Triggerfunktioner för olika GitHub-events;Inbyggd hemlighantering och caching");
        
        // GitLab CI/CD
        UpsertTool(context, allCategories,
            "GitLab CI/CD", 
            "CI/CD",
            "GitLab CI/CD är en inbyggd funktion i GitLab-plattformen som erbjuder en komplett DevOps-lösning från kodhantering till driftsättning.",
            "https://about.gitlab.com/images/press/logo/svg/gitlab-logo-500.svg",
            "GitLab logotyp",
            "https://github.com/gitlabhq/gitlabhq",
            "https://about.gitlab.com/",
            "Komplett DevOps-plattform i en produkt;Auto DevOps för automatisk pipeline-konfiguration;Inbyggd container registry och artefakthantering;Integrerad kodgranskning och testrapportering;Avancerad visualisering av CI/CD-flöden");
        
        // === Containers ===
        
        // Docker
        UpsertTool(context, allCategories, 
            "Docker", 
            "Containers",
            "Docker är en plattform för att utveckla, leverera och köra applikationer i containrar. Det förenklar arbetet med att paketera applikationer och deras beroenden för konsekvent körning i olika miljöer.",
            "https://api.iconify.design/skill-icons/docker.svg",
            "Docker logotyp",
            "https://github.com/docker/docker-ce",
            "https://www.docker.com/",
            "Containerisering av applikationer;Effektiv image-hantering;Omfattande ekosystem med Docker Hub;Enkel delning av utvecklingsmiljöer;Sömlös integration med CI/CD-verktyg");
        
        // Podman
        UpsertTool(context, allCategories,
            "Podman", 
            "Containers",
            "Podman är ett daemonless container engine för att utveckla, hantera och köra OCI-containrar med förbättrad säkerhet genom rootless execution.",
            "https://simpleicons.org/icons/podman.svg",
            "Podman logotyp",
            "https://github.com/containers/podman",
            "https://podman.io/",
            "Rootless containers för förbättrad säkerhet;Fungerar utan central daemon-process;Full kompatibilitet med Docker-kommandon;Native pod-support liknande Kubernetes;Integrering med systemd för bättre systemhantering");
        
        // === Orchestration ===
        
        // Kubernetes
        UpsertTool(context, allCategories,
            "Kubernetes", 
            "Orchestration",
            "Kubernetes är ett open-source system för automatisering av driftsättning, skalning och hantering av containeriserade applikationer, ursprungligen utvecklat av Google.",
            "https://api.iconify.design/skill-icons/kubernetes.svg",
            "Kubernetes logotyp",
            "https://github.com/kubernetes/kubernetes",
            "https://kubernetes.io/",
            "Automatisk skalning och lastbalansering;Self-healing med automatisk återstart;Deklarativ konfigurationshantering;Rollbaserad åtkomstkontroll (RBAC);Omfattande ekosystem av tillägg");
        
        // Docker Swarm
        UpsertTool(context, allCategories,
            "Docker Swarm", 
            "Orchestration",
            "Docker Swarm är Docker's inbyggda orkestreringsverktyg som erbjuder en enklare lösning för att hantera containrar över flera värdar.",
            "https://api.iconify.design/skill-icons/docker.svg",
            "Docker Swarm logotyp",
            "https://github.com/docker/docker-ce",
            "https://www.docker.com/",
            "Enkel implementation med Docker-kommandon;Inbyggt overlay-nätverk mellan noder;Integrerad service discovery;Rolling updates och health checks;Låg inlärningströskel för Docker-användare");
        
        // === Infrastructure as Code ===
        
        // Terraform
        UpsertTool(context, allCategories,
            "Terraform", 
            "Infrastructure as Code",
            "Terraform är ett verktyg för infrastruktur som kod som låter dig definiera och hantera resurser från olika molnleverantörer genom deklarativ konfiguration.",
            "https://api.iconify.design/logos/terraform-icon.svg",
            "Terraform logotyp",
            "https://github.com/hashicorp/terraform",
            "https://www.terraform.io/",
            "Leverantörsoberoende med stöd för hundratals providers;Tillståndshantering för säkra uppdateringar;Modulär design för återanvändbar kod;Planerings- och förhandsgranskningsfunktionalitet;Stort community och omfattande dokumentation");
        
        // Pulumi
        UpsertTool(context, allCategories,
            "Pulumi", 
            "Infrastructure as Code",
            "Pulumi är en modern IaC-plattform som låter dig använda fullständiga programmeringsspråk istället för domänspecifika språk för att hantera infrastruktur.",
            "https://www.pulumi.com/logos/brand/avatar-on-white.svg",
            "Pulumi logotyp",
            "https://github.com/pulumi/pulumi",
            "https://www.pulumi.com/",
            "Stöd för flera språk (Python, TypeScript, Go, C#);Kraftfulla abstraktioner och komponenter;Typkontroll och felhantering i utvecklingsmiljön;Inbyggd testramverk för infrastrukturkod;Kompatibilitet med befintliga molnresurser");
        
        context.SaveChanges();
    }
    
    private static void UpsertTool(
        ApplicationDbContext context, 
        Dictionary<string, Category> categories,
        string name, 
        string categoryName,
        string description, 
        string? logoUrl, 
        string logoAltText, 
        string? gitHubUrl, 
        string? officialUrl, 
        string? features)
    {
        // Hämta kategori
        if (!categories.TryGetValue(categoryName, out var category))
        {
            // Kategorin hittades inte, logga ett fel eller hantera på annat sätt
            return;
        }
        
        // Kolla om verktyget finns
        var existingTool = context.Tools.FirstOrDefault(t => t.Name == name);
        
        if (existingTool == null)
        {
            // Skapa nytt verktyg
            context.Tools.Add(new Tool 
            {
                Name = name,
                Description = description,
                Category = category,
                LogoUrl = logoUrl,
                LogoAltText = logoAltText,
                GitHubUrl = gitHubUrl,
                OfficialUrl = officialUrl,
                Features = features
            });
        }
        else
        {
            // Uppdatera befintligt verktyg
            existingTool.Description = description;
            existingTool.CategoryId = category.Id;
            existingTool.LogoUrl = logoUrl;
            existingTool.LogoAltText = logoAltText;
            existingTool.GitHubUrl = gitHubUrl;
            existingTool.OfficialUrl = officialUrl;
            existingTool.Features = features;
        }
    }
}