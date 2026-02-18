using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillSnap.Shared.Models;

namespace SkillSnap.Api.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var context = scope.ServiceProvider.GetRequiredService<SkillSnapContext>();

        string[] roles = { "Admin", "User" };
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        string adminEmail = "admin@skillsnap.com";
        var adminPassword = Environment.GetEnvironmentVariable("SKILLSNAP_DEFAULT_ADMIN_PASSWORD")
                            ?? throw new InvalidOperationException("Admin password not set. Please set SKILLSNAP_DEFAULT_ADMIN_PASSWORD environment variable.");  

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true,
                Bio = "",
                ProfileImageUrl = "https://via.placeholder.com/150?text=Admin"
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);

            if (!createResult.Succeeded)
            {
                throw new Exception($"Admin user creation failed: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }

            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        if(!(await userManager.GetUsersInRoleAsync("User")).Any())
        {
            var user = new ApplicationUser
            {
                UserName="sample.user@skillsnap.com",
                Email = "sample.user@skillsnap.com",
                FirstName = "Jordan ",
                LastName = "Developer",
                Bio = "Full-stack developer passionate about learning new tech.",
                ProfileImageUrl = "https://example.com/images/jordan.png",
                Projects = new List<Project>
                {
                    new Project { Title = "Task Tracker", Description = "Manage tasks effectively", ImageUrl = "https://example.com/images/task.png" },
                    new Project { Title = "Weather App", Description = "Forecast weather using APIs", ImageUrl = "https://example.com/images/weather.png" }
                },
                Skills = new List<Skill>
                {
                    new Skill { Name = "C#", Level = "Advanced" },
                    new Skill { Name = "Blazor", Level = "Intermediate" }
                }
            };
            
            var createResult = await userManager.CreateAsync(user, "Cic@1234");
            if (!createResult.Succeeded)
            {
                throw new Exception($"Test user creation failed: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }
            await userManager.AddToRoleAsync(user, "User");
        }
        

        await context.SaveChangesAsync();


    }
}