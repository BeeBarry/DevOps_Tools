using Devops_tools.Services;
using Devops_tools.Services.Interfaces;
using Devops_tools.Components;
using Devops_tools.Data;
using Devops_tools.Repositories;
using Devops_tools.Repositories.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Lägg till tjänster för Razor Pages och server-side Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Konfiguration för databaskoppling
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                      throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Lägg till DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Lägg till Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => 
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Lägg till Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => 
        policy.RequireRole("Admin"));
});

// Lägg till HttpClient för GitHub API
builder.Services.AddHttpClient();

// Lägg till Memory Cache
builder.Services.AddMemoryCache();

// Repository registreringar
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IToolRepository, ToolRepository>();

// Service registreringar
builder.Services.AddScoped<IGitHubService, GitHubService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IToolService, ToolService>();

var app = builder.Build();

// Konfigurera HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Middleware för anti-forgery (för vi har endpoints med dessa metadatas)
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Initialisera databasen och skapa admin-användare
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
        
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
        // Skapa Admin-roll om den inte finns
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        
        // Skapa admin-användare om den inte finns
        var adminEmail = builder.Configuration["AdminCredentials:Email"];
        if (!string.IsNullOrEmpty(adminEmail))
        {
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                
                var adminPassword = builder.Configuration["AdminCredentials:Password"];
                if (!string.IsNullOrEmpty(adminPassword))
                {
                    var result = await userManager.CreateAsync(adminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

app.Run();