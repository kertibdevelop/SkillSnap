using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SkillSnap.Api;
using SkillSnap.Shared.Models;

namespace SkillSnap.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SkillsController : ControllerBase
{
    private readonly SkillSnapContext _context;

    public SkillsController(SkillSnapContext context)
    {
        _context = context;
    }

    // GET: api/skills
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Skill>>> GetSkills()
    {
        return await _context.Skills
            .ToListAsync();
    }

    // GET: api/skills/5 
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Skill>> GetSkill(int id)
    {
        var skill = await _context.Skills
            .FirstOrDefaultAsync(s => s.Id == id);

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

        return CreatedAtAction(nameof(GetSkill), new { id = skill.Id }, skill);
    }

    // DELETE: api/skills/5   (ajánlott kiegészítés)
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

        return NoContent();
    }
}