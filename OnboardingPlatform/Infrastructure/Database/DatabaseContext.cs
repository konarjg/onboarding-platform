namespace Infrastructure.Database;

using Domain;
using Microsoft.EntityFrameworkCore;

public class DatabaseContext : DbContext {
  public DbSet<User> Users { get; set; }
  public DbSet<Path> Paths { get; set; }
  public DbSet<Module> Modules { get; set; }
  public DbSet<ContentSection> ContentSections { get; set; }
  public DbSet<UserPathProgress> UserPathProgresses { get; set; }
  public DbSet<UserModuleProgress> UserModuleProgresses { get; set; }
  public DbSet<RefreshToken> RefreshTokens { get; set; }

  public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<User>(entity => {
      entity.HasIndex(e => e.Email).IsUnique();
      entity.Property(e => e.IsActive).HasDefaultValue(true);
      entity.HasQueryFilter(e => e.IsActive);
    });

    modelBuilder.Entity<Path>(entity => {
      entity.HasMany(p => p.Modules)
        .WithOne(m => m.Path)
        .HasForeignKey(m => m.PathId)
        .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<Module>(entity => {
      entity.HasMany(m => m.ContentSections)
        .WithOne(cs => cs.Module)
        .HasForeignKey(cs => cs.ModuleId)
        .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<ContentSection>(entity => {
      entity.HasDiscriminator<string>("SectionType")
        .HasValue<MarkdownSection>("Markdown")
        .HasValue<ImageSection>("Image");
    });
    
    modelBuilder.Entity<UserPathProgress>(entity => {
      entity.HasKey(e => new { e.UserId, e.PathId });

      entity.HasOne(e => e.User)
        .WithMany(u => u.AssignedPaths)
        .HasForeignKey(e => e.UserId);

      entity.HasOne(e => e.Path)
        .WithMany(p => p.AssignedUsers)
        .HasForeignKey(e => e.PathId);
    });
    
    modelBuilder.Entity<UserModuleProgress>(entity => {
      entity.HasKey(e => new { e.UserId, e.ModuleId });

      entity.HasOne(e => e.User)
        .WithMany(u => u.AssignedModules)
        .HasForeignKey(e => e.UserId);

      entity.HasOne(e => e.Module)
        .WithMany(m => m.AssignedUsers)
        .HasForeignKey(e => e.ModuleId);
    });

    modelBuilder.Entity<RefreshToken>(entity => {
      entity.HasOne(e => e.User)
        .WithMany(u => u.Sessions)
        .HasForeignKey(e => e.UserId);
        
      entity.HasIndex(e => e.Token).IsUnique();
    });
  }
}