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
        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasIndex(u => u.Email)
                .IsUnique();
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.HasIndex(d => d.CreatedAt);
            entity.HasIndex(d => d.OriginalName);
            
            entity.Property(d => d.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(d => d.FileName)
                .IsRequired()
                .HasMaxLength(36); // Guid.ToString() length

            entity.Property(d => d.OriginalName)
                   .IsRequired()
                   .HasMaxLength(255);

            entity.Property(d => d.Content)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(d => d.CreatedAt)
                .HasDefaultValueSql("NOW()");

            entity.HasOne(d => d.Owner)
                .WithMany(u => u.OwnedDocuments)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DocumentPermission>(entity =>
        {
            entity.HasKey(dp => dp.Id);

            entity.Property(dp => dp.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(dp => dp.Document)
                .WithMany(d => d.Permissions)
                .HasForeignKey(dp => dp.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(dp => dp.User)
                .WithMany(u => u.Permissions)
                .HasForeignKey(dp => dp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(dp => new { dp.DocumentId, dp.UserId })
                .IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
    {
        foreach (var entry in ChangeTracker.Entries<Document>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
            }
        }

        return base.SaveChanges();
    }
}