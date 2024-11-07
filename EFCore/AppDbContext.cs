using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Models;
using Task = TaskManagementSystem.Models.Task;

namespace TaskManagementSystem.EFCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Task> Tasks { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>(b =>
        {
            b.ToTable("Projects");
            b.Property(x => x.Title).HasMaxLength(50).IsRequired();
            b.Property(x => x.Description).HasMaxLength(500).IsRequired();
            b.Property(x => x.Id).IsRequired();
        });
        modelBuilder.Entity<Role>(b =>
        {
            b.ToTable("Roles");
            b.Property(x => x.Name).HasMaxLength(50).IsRequired();
            b.Property(x => x.Id).IsRequired();
        });
        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("Users");
            b.Property(x => x.Name).HasMaxLength(50).IsRequired();
            b.Property(x => x.Email).HasMaxLength(500).IsRequired();
            b.Property(x => x.Password).HasMaxLength(500).IsRequired();
            b.Property(x => x.Id).IsRequired();
            b.HasOne<Role>().WithMany().HasForeignKey(x => x.RoleId).IsRequired();
        });
      
    }
}