using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SkillSnap.Shared.Models;

public class PortfolioUser : IPortfolioUser
{

    [Key]
    public string Id { get; set; } = string.Empty;
    
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string ProfileImageUrl { get; set; } = string.Empty;
    public List<Project> Projects { get; set; } = new List<Project>();
    public List<Skill> Skills { get; set; } = new List<Skill>();
}