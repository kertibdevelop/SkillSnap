using SkillSnap.Shared.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace SkillSnap.Client.Services;

public class ProjectService
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;

    public ProjectService(IHttpClientFactory httpClientFactory, NavigationManager navigationManager)
    {
        _httpClient = httpClientFactory.CreateClient("api");
        _navigationManager = navigationManager;
    }

    // GET: api/projects
    public async Task<List<Project>?> GetProjectsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<Project>>("api/projects");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _navigationManager.NavigateTo("/login");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching projects: {ex.Message}");
            return null;
        }
        return await _httpClient.GetFromJsonAsync<List<Project>>("api/projects");
    }

    // POST: api/projects
    public async Task<Project?> AddProjectAsync(Project newProject)
    {
        try {
            var response = await _httpClient.PostAsJsonAsync("api/projects", newProject);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Project>();
            }
        } catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _navigationManager.NavigateTo("/login");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding project: {ex.Message}");
            return null;
        }

        
        
        return null;
    }
}