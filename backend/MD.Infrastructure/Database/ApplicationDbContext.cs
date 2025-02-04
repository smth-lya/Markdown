using MD.Domain;
using Microsoft.EntityFrameworkCore;

namespace MD.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }
    public DbSet<User> Users { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentPermission> DocumentPermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.FileName).IsRequired();
            entity.Property(d => d.OriginalName).IsRequired();
            entity.Property(d => d.CreatedAt).IsRequired();

            entity.HasOne(d => d.Owner)
                  .WithMany(u => u.OwnedDocuments)
                  .HasForeignKey(d => d.OwnerId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(d => d.Permissions)
                  .WithOne(dp => dp.Document)
                  .HasForeignKey(dp => dp.DocumentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DocumentPermission>(entity =>
        {
            entity.HasKey(dp => dp.Id);

            entity.HasOne(dp => dp.Document)
                  .WithMany(d => d.Permissions)
                  .HasForeignKey(dp => dp.DocumentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(dp => dp.User)
                  .WithMany(u => u.Permissions)
                  .HasForeignKey(dp => dp.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();

            entity.HasMany(u => u.OwnedDocuments)
                  .WithOne(d => d.Owner)
                  .HasForeignKey(d => d.OwnerId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(u => u.Permissions)
                  .WithOne(dp => dp.User)
                  .HasForeignKey(dp => dp.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}