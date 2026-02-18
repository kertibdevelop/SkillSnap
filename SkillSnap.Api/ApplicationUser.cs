

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    [Required]
    public string Email { get; set;}

    [Required]
    public string Password { get; set;}
    public string Role { get; set;} = "";

    public string Title { get; set;} = "";
    public string FirstName { get; set;} = "";
    public string MiddleName { get; set;} = "";
    public string LastName { get; set;} = "";

}