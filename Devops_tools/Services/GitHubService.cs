using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Devops_tools.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
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
            return null;
        }
        
        try
        {
            // Försök extrahera owner och repo från URL
            var (owner, repo) = ExtractOwnerAndRepo(repositoryUrl);
            if (owner == null || repo == null)
            {
                _logger.LogWarning("Ogiltig GitHub URL: {Url}", repositoryUrl);
                return null;
            }
            
            // Skapa cache-nyckel
            string cacheKey = $"github-stars-{owner}-{repo}";
            
            // Försök hämta från cache först
            if (_cache.TryGetValue(cacheKey, out int cachedStars))
            {
                return cachedStars;
            }
            
            // Anropa GitHub API
            var apiUrl = $"https://api.github.com/repos/{owner}/{repo}";
            var repoInfo = await _httpClient.GetFromJsonAsync<GitHubRepoInfo>(apiUrl);
            
            if (repoInfo != null)
            {
                // Spara i cache
                _cache.Set(cacheKey, repoInfo.StargazersCount, _cacheDuration);
                return repoInfo.StargazersCount;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fel vid hämtning av GitHub-stjärnor för {Url}", repositoryUrl);
            return null;
        }
    }
    
    public (string? owner, string? repo) ExtractOwnerAndRepo(string? repositoryUrl)
    {
        if (string.IsNullOrEmpty(repositoryUrl))
        {
            return (null, null);
        }
        
        // Matcha URL-mönster för GitHub repositories
        // Stödjer format som:
        // - https://github.com/owner/repo
        // - http://github.com/owner/repo
        // - github.com/owner/repo
        var regex = new Regex(@"github\.com[/:](?<owner>[^/]+)/(?<repo>[^/]+)");
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
            
            return (owner, repo);
        }
        
        return (null, null);
    }
    
    // Privat klass för deserialisering av GitHub API-svar
    private class GitHubRepoInfo
    {
        public int StargazersCount { get; set; }
    }
}