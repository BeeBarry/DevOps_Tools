using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Devops_tools.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Devops_tools.Services;

public class GitHubService : IGitHubService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<GitHubService> _logger;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(1);

    public GitHubService(HttpClient httpClient, IMemoryCache cache, ILogger<GitHubService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
        
        // Konfigurera GitHub API headers
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "DevOps-Tools-App");
    }
    
    public async Task<int?> GetRepositoryStarsAsync(string? repositoryUrl)
    {
        if (string.IsNullOrEmpty(repositoryUrl))
        {
            _logger.LogWarning("Anrop till GetRepositoryStarsAsync med tom URL");
            return null;
        }
        
        try
        {
            // Försök extrahera owner och repo från URL
            var (owner, repo) = ExtractOwnerAndRepo(repositoryUrl);
            if (owner == null || repo == null)
            {
                _logger.LogWarning("Ogiltig GitHub URL kunde inte extraheras: {Url}", repositoryUrl);
                return null;
            }
            
            _logger.LogInformation("Försöker hämta stjärnor för {Owner}/{Repo} från {Url}", owner, repo, repositoryUrl);
            
            // Skapa cache-nyckel
            string cacheKey = $"github-stars-{owner}-{repo}";
            
            // Försök hämta från cache först
            if (_cache.TryGetValue(cacheKey, out int cachedStars))
            {
                _logger.LogInformation("Hittade cachad data för {Owner}/{Repo}: {Stars} stjärnor", owner, repo, cachedStars);
                return cachedStars;
            }
            
            // Anropa GitHub API med bättre felhantering
            var apiUrl = $"https://api.github.com/repos/{owner}/{repo}";
            _logger.LogInformation("Gör API-anrop till: {ApiUrl}", apiUrl);
            
            var response = await _httpClient.GetAsync(apiUrl);
            _logger.LogInformation("API-svar status: {Status} för {Owner}/{Repo}", response.StatusCode, owner, repo);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("API-svar innehåll: {Content}", responseContent);
                
                try 
                {
                    var repoInfo = JsonSerializer.Deserialize<GitHubRepoInfo>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    if (repoInfo != null)
                    {
                        _logger.LogInformation("Hämtade {Stars} stjärnor för {Owner}/{Repo}", repoInfo.StargazersCount, owner, repo);
                        // Spara i cache
                        _cache.Set(cacheKey, repoInfo.StargazersCount, _cacheDuration);
                        return repoInfo.StargazersCount;
                    }
                    else
                    {
                        _logger.LogWarning("Kunde inte deserialisera GitHub API-svar för {Owner}/{Repo}", owner, repo);
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "JSON-deserialiseringsfel för {Owner}/{Repo}: {Message}", owner, repo, ex.Message);
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("GitHub API rate limit nådd för {Owner}/{Repo}: {Content}", owner, repo, responseContent);
                
                // Returnera eventuellt tidigare cachat värde om det finns
                if (_cache.TryGetValue(cacheKey, out int oldCachedValue))
                {
                    _logger.LogInformation("Återanvänder gammal cachad data: {Stars} stjärnor", oldCachedValue);
                    return oldCachedValue;
                }
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("GitHub API-fel: {StatusCode} för {Owner}/{Repo}: {Content}", 
                    response.StatusCode, owner, repo, responseContent);
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Oväntat fel vid hämtning av GitHub-stjärnor för {Url}: {Message}", repositoryUrl, ex.Message);
            return null;
        }
    }
    
    public (string? owner, string? repo) ExtractOwnerAndRepo(string? repositoryUrl)
    {
        if (string.IsNullOrEmpty(repositoryUrl))
        {
            return (null, null);
        }
        
        _logger.LogDebug("Försöker extrahera owner/repo från URL: {Url}", repositoryUrl);
        
        // Speciell hantering för GitHub Actions och andra feature-URLs
        if (repositoryUrl.Contains("github.com/features/"))
        {
            _logger.LogWarning("URL är en GitHub feature, inte ett repository: {Url}", repositoryUrl);
            return (null, null);
        }
        
        // Matcha URL-mönster för GitHub repositories
        // Stödjer format som:
        // - https://github.com/owner/repo
        // - http://github.com/owner/repo
        // - github.com/owner/repo
        var regex = new Regex(@"github\.com[/:](?<owner>[^/]+)/(?<repo>[^/\.]+)");
        var match = regex.Match(repositoryUrl);
        
        if (match.Success)
        {
            var owner = match.Groups["owner"].Value;
            var repo = match.Groups["repo"].Value;
            
            // Rensa eventuellt .git suffix
            if (repo.EndsWith(".git"))
            {
                repo = repo.Substring(0, repo.Length - 4);
            }
            
            _logger.LogDebug("Extraherade owner: {Owner}, repo: {Repo}", owner, repo);
            return (owner, repo);
        }
        
        _logger.LogWarning("Kunde inte extrahera owner/repo från URL: {Url}", repositoryUrl);
        return (null, null);
    }
    
    // Privat klass för deserialisering av GitHub API-svar
    private class GitHubRepoInfo
    {
        // GitHub API använder snake_case, inte PascalCase
        public int stargazers_count { get; set; }
        
        // Property för enklare tillgång i koden
        public int StargazersCount => stargazers_count;
    }
}