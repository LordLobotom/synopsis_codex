using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ReportGenerator.Domain.Entities;
using ReportGenerator.Infrastructure.Data;
using ReportGenerator.Infrastructure.Repositories;
using Xunit;

namespace ReportGenerator.Domain.Tests;

public class TemplateRepositoryIntegrationTests
{
    [Fact]
    public async Task AddAndList_ShouldPersist_UsingSqliteInMemory()
    {
        using var conn = new SqliteConnection("DataSource=:memory:");
        conn.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(conn)
            .Options;

        using var db = new AppDbContext(options);
        db.Database.EnsureCreated();

        var repo = new TemplateRepository(db);
        await repo.AddAsync(new Template { Name = "Test" });

        var list = await repo.ListAsync();
        list.Should().ContainSingle(t => t.Name == "Test");
    }
}

