using Microsoft.EntityFrameworkCore;
using TmsApi.Entities;
namespace TmsApi.Data;
public class TmsDbContext(DbContextOptions<TmsDbContext> options) : DbContext(options)
{
public DbSet<Student> Students => Set<Student>();
public DbSet<Course> Courses => Set<Course>();
public DbSet<Enrollment> Enrollments => Set<Enrollment>();
public DbSet<Assessment> Assessments => Set<Assessment>();
public DbSet<Certificate> Certificates => Set<Certificate>();

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfigurationsFromAssembly(
        typeof(TmsDbContext).Assembly
    );
    modelBuilder.Entity<Student>()
    .HasQueryFilter(s => !s.IsDeleted);
    
    modelBuilder.Entity<Enrollment>()
    .HasQueryFilter(e => !e.IsArchived);
}


public override async Task<int> SaveChangesAsync(
    CancellationToken cancellationToken = default)
{
    foreach(var entry in ChangeTracker.Entries<Student>())
    {
        if(entry.State == EntityState.Modified ||
           entry.State == EntityState.Added)
        {
            entry.Property("LastUpdated")
                 .CurrentValue = DateTime.UtcNow;
        }
    }


    return await base.SaveChangesAsync(cancellationToken);
}
}