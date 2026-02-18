using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SkillSnap.Shared.Models;

public class Skill
{
    [Key]
    public int Id { get; set;}
    
    [Required]
    public string Name { get; set;} 

    public string Level { get; set;}

    [ForeignKey("PortfolioUser")]
    public int PortfolioUserId { get; set; }

    [JsonIgnore]
    public PortfolioUser? PortfolioUser { get; set; }
}