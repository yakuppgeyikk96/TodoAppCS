
using FirstWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstWebApi.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options) // Dependency Injection için DbContextOptions<ApplicationDbContext> options parametresi gerekiyor
{
  public DbSet<Todo> Todos { get; set; }
  public DbSet<User> Users { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Todo>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
      entity.Property(e => e.Description).HasMaxLength(1000);
      entity.Property(e => e.CreatedAt).IsRequired();
    });

    modelBuilder.Entity<User>(entity =>
    {
      entity.HasIndex(u => u.Username).IsUnique();
      entity.HasIndex(u => u.Email).IsUnique();
    });
  }
}
