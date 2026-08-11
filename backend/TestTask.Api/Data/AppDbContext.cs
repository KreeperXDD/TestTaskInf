using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

    public DbSet<Value> Value => Set<Value>();

    public DbSet<Result> Results => Set<Result>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Value>(entity =>
        {
            entity.ToTable("Values");

            entity.HasKey(x =>x.Id);

            entity.Property(x => x.FileName)
            .HasMaxLength(255)
            .IsRequired();

            entity.Property(x => x.Date)
            .IsRequired();

            entity.Property(x => x.ExecutionTime)
            .IsRequired();

            entity.Property(x => x.MetricValue)
            .IsRequired();

            entity.HasIndex(x => new {x.FileName, x.Date});
        });

        modelBuilder.Entity<Result>(entity =>
        {
            entity.ToTable("Results");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.FileName)
            .HasMaxLength(255)
            .IsRequired();

            entity.HasIndex(x => x.FileName)
            .IsUnique();

            entity.Property(x => x.FirstOperationDate)
            .IsRequired();
        });
    }
}
