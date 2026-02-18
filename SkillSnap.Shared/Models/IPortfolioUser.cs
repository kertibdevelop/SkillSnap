namespace SkillSnap.Shared.Models;

public interface IPortfolioUser
{

    string FirstName { get; set; }
    string MiddleName { get; set; }
    string LastName { get; set; } 
    string Bio { get; set; }
    string ProfileImageUrl { get; set; }

    List<Project> Projects { get; set; }
    List<Skill> Skills { get; set; }
}