using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SkillSnap.Shared.Models;

public class Project
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public string ImageUrl { get; set; }
    
    [ForeignKey("PortfolioUser")]
    public int PortfolioUserId { get; set; }

    [JsonIgnore]
    public PortfolioUser? PortfolioUser { get; set; }

}