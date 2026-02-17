using SkillSnap.Shared.Models;
using System.Net.Http.Json;

namespace SkillSnap.Client.Services;

public class ProjectService
{
    private readonly HttpClient _httpClient;

    public ProjectService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // GET: api/projects
    public async Task<List<Project>?> GetProjectsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Project>>("api/projects");
    }

    // POST: api/projects
    public async Task<Project?> AddProjectAsync(Project newProject)
    {
        var response = await _httpClient.PostAsJsonAsync("api/projects", newProject);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Project>();
        }
        
        return null;
    }
}