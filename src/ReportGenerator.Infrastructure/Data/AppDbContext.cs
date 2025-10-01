using Microsoft.EntityFrameworkCore;
using ReportGenerator.Domain.Entities;

namespace ReportGenerator.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Template> Templates => Set<Template>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Template>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.Description).HasMaxLength(1000);
            b.Property(x => x.CreatedAtUtc).IsRequired();
        });
    }
}

