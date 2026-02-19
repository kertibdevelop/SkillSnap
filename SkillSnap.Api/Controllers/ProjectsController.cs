using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSnap.Api;
using SkillSnap.Shared.Models;

namespace SkillSnap.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly SkillSnapContext _context;

    public ProjectsController(SkillSnapContext context)
    {
        _context = context;
    }

    // GET: api/projects
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
    {
        

        return await _context.Projects
            .ToListAsync();
    }

    // GET: api/projects/5  
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Project>> GetProject(int id)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id);

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

        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        // Megjegyzés: ha később GetProject(int id) endpoint is lesz, akkor használd azt
    }

    

    // DELETE: api/projects/5   (ajánlott kiegészítés)
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

        return NoContent();
    }
}