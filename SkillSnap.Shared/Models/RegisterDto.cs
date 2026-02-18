using System.ComponentModel.DataAnnotations;

namespace SkillSnap.Shared.Models;

public class RegisterDto
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
}