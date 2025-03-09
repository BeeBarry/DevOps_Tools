using System.Threading.Tasks;

namespace Devops_tools.Services.Interfaces;

public interface IGitHubService
{
    /// <summary>
    /// Hämtar antalet stjärnor för ett GitHub-repository baserat på URL
    /// </summary>
    /// <param name="repositoryUrl">GitHub repository URL (t.ex. https://github.com/docker/docker-ce)</param>
    /// <returns>Antal stjärnor eller null om inte tillgängligt</returns>
    Task<int?> GetRepositoryStarsAsync(string? repositoryUrl);
    
    /// <summary>
    /// Extraherar owner och repo-namn från en GitHub URL
    /// </summary>
    /// <param name="repositoryUrl">GitHub repository URL</param>
    /// <returns>Tuple med (owner, repo) eller (null, null) om URL är ogiltig</returns>
    (string? owner, string? repo) ExtractOwnerAndRepo(string? repositoryUrl);
}