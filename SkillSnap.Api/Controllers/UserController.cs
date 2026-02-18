
using Microsoft.AspNetCore.Mvc;
using SkillSnap.Shared.Models;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly SkillSnapContext _context;

    public UserController(SkillSnapContext context)
    {
        _context = context;
    }

    // GET: api/user
    [HttpGet]
    public ActionResult<IEnumerable<PortfolioUser>> GetUsers()
    {
        return _context.PortfolioUsers.ToList();
    }

    // GET: api/user/5
    [HttpGet("{id}")]
    public ActionResult<PortfolioUser> GetUser(int id)
    {
        var user = _context.PortfolioUsers.Find(id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }
}