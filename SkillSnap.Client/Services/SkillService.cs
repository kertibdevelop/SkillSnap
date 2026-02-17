using SkillSnap.Shared.Models;
using System.Net.Http.Json;

namespace SkillSnap.Client.Services;

public class SkillService
{
    private readonly HttpClient _httpClient;

    public SkillService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // GET: api/skills
    public async Task<List<Skill>?> GetSkillsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Skill>>("api/skills");
    }

    // POST: api/skills
    public async Task<Skill?> AddSkillAsync(Skill newSkill)
    {
        var response = await _httpClient.PostAsJsonAsync("api/skills", newSkill);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Skill>();
        }

        return null;
    }
}