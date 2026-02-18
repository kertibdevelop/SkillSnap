using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SkillSnap.Shared.Models;

public class PortfolioUser
{

    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string Bio { get; set; } = "";

    public string ProfileImageUrl { get; set; }
    
    [JsonIgnore]
    public List<Project>? Projects { get; set; }

    [JsonIgnore]
    public List<Skill>? Skills { get; set; }

}