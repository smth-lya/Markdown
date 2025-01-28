using MD.Domain;
using Microsoft.EntityFrameworkCore;

namespace MD.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Document> Documents => Set<Document>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(x =>
        {
            x.Property(p => p.Email)
                .IsRequired();

            x.Property(p => p.PasswordHash)
                .IsRequired();

            x.HasIndex(p => p.Email)
                .IsUnique();
        });

        modelBuilder.Entity<Document>(x =>
        {
            x.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            x.Property(p => p.CreatedAt)
                .HasDefaultValueSql("NOW()");
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