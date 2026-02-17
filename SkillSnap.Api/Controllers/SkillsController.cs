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
    public async Task<ActionResult<IEnumerable<Skill>>> GetSkills()
    {
        return await _context.Skills
            .Include(s => s.PortfolioUser)  // opcionális
            .ToListAsync();
    }

    // POST: api/skills
    [HttpPost]
    public async Task<ActionResult<Skill>> CreateSkill(Skill skill)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSkill), new { id = skill.Id }, skill);
    }

    // GET: api/skills/5   (opcionális segéd)
    [HttpGet("{id}")]
    public async Task<ActionResult<Skill>> GetSkill(int id)
    {
        var skill = await _context.Skills
            //.Include(s => s.PortfolioUser)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (skill == null)
        {
            return NotFound();
        }

        return skill;
    }

    // DELETE: api/skills/5   (ajánlott kiegészítés)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSkill(int id)
    {
        var skill = await _context.Skills.FindAsync(id);
        if (skill == null)
        {
            return NotFound();
        }

        _context.Skills.Remove(skill);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}