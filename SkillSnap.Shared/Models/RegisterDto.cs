using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SkillSnap.Shared.Models;

public class RegisterDto :IPortfolioUser
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty; 
    public string MiddleName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string ProfileImageUrl { get; set; } = string.Empty;

    [JsonIgnore]
    public List<Project> Projects { get; set; } = new List<Project>();
    
    [JsonIgnore]
    public List<Skill> Skills { get; set; } = new List<Skill>();
}