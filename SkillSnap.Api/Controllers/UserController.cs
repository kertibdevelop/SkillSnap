
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSnap.Shared.Models;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly SkillSnapContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(SkillSnapContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: api/user
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PortfolioUser>>> GetUsers()
    {
        var users = await _userManager.Users
            .Include(u => u.Projects)
            .Include(u => u.Skills)
            .Select(u => new PortfolioUser
            {
                Id = u.Id,
                FirstName = u.FirstName,
                MiddleName = u.MiddleName,
                LastName = u.LastName,
                Bio = u.Bio,
                ProfileImageUrl = u.ProfileImageUrl,
                Projects = u.Projects,
                Skills = u.Skills
            })
            .ToListAsync();

        return Ok(users);
    }

    // GET: api/user/5
    [HttpGet("{id}")]
    public async Task<ActionResult<PortfolioUser>> GetUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var portfolio = new PortfolioUser
        {
            Id = user.Id,
            FirstName = user.FirstName,
            MiddleName = user.MiddleName,
            LastName = user.LastName,
            Bio = user.Bio,
            ProfileImageUrl = user.ProfileImageUrl,
            Projects = user.Projects,
            Skills = user.Skills
        };

        return Ok(portfolio);
    }
}