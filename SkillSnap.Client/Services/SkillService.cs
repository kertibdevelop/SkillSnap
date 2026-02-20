using Microsoft.AspNetCore.Components;
using SkillSnap.Shared.Models;
using System.Net.Http.Json;

namespace SkillSnap.Client.Services;

public class SkillService
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;

    public SkillService(IHttpClientFactory httpClientFactory, NavigationManager navigationManager)
    {
        _httpClient = httpClientFactory.CreateClient("api");
        _navigationManager = navigationManager;
    }

    // GET: api/skills
    public async Task<List<Skill>?> GetSkillsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<Skill>>("api/skills");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _navigationManager.NavigateTo("/login");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching skills: {ex.Message}");
            return null;
        }
        return await _httpClient.GetFromJsonAsync<List<Skill>>("api/skills");
    }

    // POST: api/skills
    public async Task<Skill?> AddSkillAsync(Skill newSkill)
    {
        try{
            var response = await _httpClient.PostAsJsonAsync("api/skills", newSkill);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Skill>();
            }
        } catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _navigationManager.NavigateTo("/login");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding skill: {ex.Message}");
            return null;
        }

        return null;
    }
}