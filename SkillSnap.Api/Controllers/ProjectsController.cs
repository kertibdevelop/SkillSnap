using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SkillSnap.Api;
using SkillSnap.Shared.Models;

namespace SkillSnap.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly SkillSnapContext _context;
    private readonly IMemoryCache _cache;
    private const int CacheAbsoluteExpirationMinutes = 10;
    private const int CacheSlidingExpirationMinutes = 2;
    private const string CacheKey = "projects";

    public ProjectsController(SkillSnapContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    // GET: api/projects
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
    {
        return (await GetCache()).Values.ToList();
    }

    // GET: api/projects/5  
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Project>> GetProject(int id)
    {

        
        var project = (await GetCache(id)).Values.FirstOrDefault();
        if (project == null)
        {
            return NotFound();
        }

        return project;
    }

    // POST: api/projects
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Project>> CreateProject(Project project)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if(User.IsInRole("Admin") && project.ApplicationUserId == null)
        {
            return BadRequest("Admin users must specify an ApplicationUserId when creating a project.");
        }
        else if(!User.IsInRole("Admin"))
        {
            string userId=User.FindFirstValue(ClaimTypes.NameIdentifier);
            project.ApplicationUserId = userId;
        }

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        await SetCache(project);

        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
    }

    

    // DELETE: api/projects/5  
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        string userId=User.FindFirstValue(ClaimTypes.NameIdentifier);
        bool isAdmin = User.IsInRole("Admin");

        if(!isAdmin && project.ApplicationUserId != userId)
        {
            return Forbid("You are not authorized to delete this project.");
        }

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        await RemoveCache(id);
        return NoContent();
    }

    private async Task<Dictionary<int, Project>> GetCache(params int[] Ids)
    {

        if (_cache.TryGetValue(CacheKey, out Dictionary<int, Project> cachedDictionary) && cachedDictionary != null)
        {
            
            if(Ids != null && Ids.Length > 0){
                var missingIds=Ids.ToList();
                missingIds.RemoveAll(pId => cachedDictionary.ContainsKey(pId));
                if(missingIds.Count > 0){
                    await _context.Projects.Where(p => missingIds.Contains(p.Id)).ForEachAsync(p => cachedDictionary[p.Id] = p);
                    _cache.Set(CacheKey, cachedDictionary, new MemoryCacheEntryOptions{
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheAbsoluteExpirationMinutes),
                                SlidingExpiration = TimeSpan.FromMinutes(CacheSlidingExpirationMinutes)
                    });
                }
                
                return cachedDictionary.Where(p => Ids.Contains(p.Key)).ToDictionary(p => p.Key, p => p.Value);
            }
        }
        else
        {
            if(Ids != null && Ids.Length > 0)
            {
                cachedDictionary = await _context.Projects
                    .Where(p => Ids.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id);
            }
            else{
                cachedDictionary = await _context.Projects.ToDictionaryAsync(p => p.Id);
            }

            _cache.Set(CacheKey, cachedDictionary, new MemoryCacheEntryOptions{
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheAbsoluteExpirationMinutes),
                        SlidingExpiration = TimeSpan.FromMinutes(CacheSlidingExpirationMinutes)
            });
            
        }

        return cachedDictionary;
    }

    private async Task SetCache(params Project[] project)
    {
        if (!_cache.TryGetValue(CacheKey, out Dictionary<int, Project> cachedDictionary))
        {
            cachedDictionary=new Dictionary<int, Project>();
        }
        foreach(var p in project)
        {
            cachedDictionary[p.Id] = p;
        }
        _cache.Set(CacheKey, cachedDictionary, new MemoryCacheEntryOptions{
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheAbsoluteExpirationMinutes),
                SlidingExpiration = TimeSpan.FromMinutes(CacheSlidingExpirationMinutes)
        });
    }

    private async Task RemoveCache(params int[] Ids)
    {
        if(Ids == null || Ids.Length == 0)
        {
            _cache.Remove(CacheKey);
            return;
        }

        if (_cache.TryGetValue(CacheKey, out Dictionary<int, Project> cachedDictionary))
        {
            foreach(var pId in Ids)
            {
                cachedDictionary.Remove(pId);
            }
            _cache.Set(CacheKey, cachedDictionary, new MemoryCacheEntryOptions{
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheAbsoluteExpirationMinutes),
                SlidingExpiration = TimeSpan.FromMinutes(CacheSlidingExpirationMinutes)
            });
        }
        
    }
}