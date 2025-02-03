using MD.Domain;
using Microsoft.EntityFrameworkCore;

namespace MD.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Document> Documents => Set<Document>();

    public DbSet<DocumentParticipant> Participants => Set<DocumentParticipant>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasIndex(u => u.Email)
                .IsUnique();
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.Property(d => d.FileName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(d => d.Content)
                .IsRequired();

            entity.Property(d => d.CreatedAt)
               .HasColumnType("timestamp with time zone")
               .HasDefaultValueSql("NOW()");

            entity.Property(d => d.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("NOW()");

            entity.HasIndex(d => d.OriginalName);
        });

        modelBuilder.Entity<DocumentParticipant>(entity =>
        {
            entity.HasKey(dp => dp.Guid);

            entity.HasOne(dp => dp.User)
                .WithMany(u => u.DocumentParticipants)
                .HasForeignKey(dp => dp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(dp => dp.Document)
                .WithMany(d => d.DocumentParticipants)
                .HasForeignKey(dp => dp.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(dp => new { dp.UserId, dp.DocumentId })
                .IsUnique();

            entity.Property(dp => dp.Role)
                .IsRequired()
                .HasConversion<string>();  
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