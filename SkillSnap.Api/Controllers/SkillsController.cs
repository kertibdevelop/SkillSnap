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
public class SkillsController : ControllerBase
{
    private readonly SkillSnapContext _context;
    private readonly IMemoryCache _cache;
    private const int CacheAbsoluteExpirationMinutes = 10;
    private const int CacheSlidingExpirationMinutes = 2;
    private const string CacheKey = "projects";

    public SkillsController(SkillSnapContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    // GET: api/skills
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Skill>>> GetSkills()
    {
        return (await GetCache()).Values.ToList();
    }

    // GET: api/skills/5 
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Skill>> GetSkill(int id)
    {
        var skill = (await GetCache(id)).Values.FirstOrDefault();

        if (skill == null)
        {
            return NotFound();
        }

        return skill;
    }

    // POST: api/skills
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Skill>> CreateSkill(Skill skill)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if(User.IsInRole("Admin") && skill.ApplicationUserId == null)
        {
            return BadRequest("Admin users must specify an ApplicationUserId when creating a skill.");
        }
        else if(!User.IsInRole("Admin"))
        {
            string userId=User.FindFirstValue(ClaimTypes.NameIdentifier);
            skill.ApplicationUserId = userId;
        }

        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();
        await SetCache(skill);
        return CreatedAtAction(nameof(GetSkill), new { id = skill.Id }, skill);
    }

    // DELETE: api/skills/5 
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteSkill(int id)
    {
        var skill = await _context.Skills.FindAsync(id);
        if (skill == null)
        {
            return NotFound();
        }

        string userId=User.FindFirstValue(ClaimTypes.NameIdentifier);
        bool isAdmin = User.IsInRole("Admin");

        if(!isAdmin && skill.ApplicationUserId != userId)
        {
            return Forbid("You are not authorized to delete this skill.");
        }

        _context.Skills.Remove(skill);
        await _context.SaveChangesAsync();
        await RemoveCache(id);
        return NoContent();
    }

    private async Task<Dictionary<int, Skill>> GetCache(params int[] Ids)
    {

        if (_cache.TryGetValue(CacheKey, out Dictionary<int, Skill> cachedDictionary) && cachedDictionary != null)
        {
            
            if(Ids != null && Ids.Length > 0){
                var missingIds=Ids.ToList();
                missingIds.RemoveAll(pId => cachedDictionary.ContainsKey(pId));
                if(missingIds.Count > 0){
                    await _context.Skills.Where(s => missingIds.Contains(s.Id)).ForEachAsync(s => cachedDictionary[s.Id] = s);
                    _cache.Set(CacheKey, cachedDictionary, new MemoryCacheEntryOptions{
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheAbsoluteExpirationMinutes),
                                SlidingExpiration = TimeSpan.FromMinutes(CacheSlidingExpirationMinutes)
                    });
                }
                
                return cachedDictionary.Where(s => Ids.Contains(s.Key)).ToDictionary(s => s.Key, s => s.Value);
            }
        }
        else
        {
            if(Ids != null && Ids.Length > 0)
            {
                cachedDictionary = await _context.Skills
                    .Where(s => Ids.Contains(s.Id))
                    .ToDictionaryAsync(s => s.Id);
            }
            else{
                cachedDictionary = await _context.Skills.ToDictionaryAsync(s => s.Id);
            }

            _cache.Set(CacheKey, cachedDictionary, new MemoryCacheEntryOptions{
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheAbsoluteExpirationMinutes),
                        SlidingExpiration = TimeSpan.FromMinutes(CacheSlidingExpirationMinutes)
            });
            
        }

        return cachedDictionary;
    }

    private async Task SetCache(params Skill[] skills)
    {
        if (!_cache.TryGetValue(CacheKey, out Dictionary<int, Skill> cachedDictionary))
        {
            cachedDictionary=new Dictionary<int, Skill>();
        }
        foreach(var s in skills)
        {
            cachedDictionary[s.Id] = s;
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

        if (_cache.TryGetValue(CacheKey, out Dictionary<int, Skill> cachedDictionary))
        {
            foreach(var sId in Ids)
            {
                cachedDictionary.Remove(sId);
            }
            _cache.Set(CacheKey, cachedDictionary, new MemoryCacheEntryOptions{
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheAbsoluteExpirationMinutes),
                SlidingExpiration = TimeSpan.FromMinutes(CacheSlidingExpirationMinutes)
            });
        }
        
    }
}