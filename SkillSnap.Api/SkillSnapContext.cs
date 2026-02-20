
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillSnap.Shared.Models;

public class SkillSnapContext : IdentityDbContext<ApplicationUser>
{
    public SkillSnapContext(DbContextOptions<SkillSnapContext> options) : base(options) { }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Skill> Skills { get; set; }

    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Project → ApplicationUser (1:N)
        builder.Entity<Project>()
            .HasOne<ApplicationUser>()
            .WithMany(u => u.Projects)
            .HasForeignKey(p => p.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Skill → ApplicationUser (1:N)
        builder.Entity<Skill>()
            .HasOne<ApplicationUser>()
            .WithMany(u => u.Skills)
            .HasForeignKey(s => s.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}