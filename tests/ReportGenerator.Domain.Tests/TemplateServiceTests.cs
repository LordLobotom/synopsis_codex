using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using ReportGenerator.Application.Services;
using ReportGenerator.Domain.Entities;
using ReportGenerator.Domain.Interfaces;
using Xunit;

namespace ReportGenerator.Domain.Tests;

public class TemplateServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCreateTemplate_WithTrimmedName()
    {
        var repo = new Mock<ITemplateRepository>();
        repo.Setup(r => r.AddAsync(It.IsAny<Template>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Template t, CancellationToken _) => t);

        var sut = new TemplateService(repo.Object);

        var created = await sut.CreateAsync("  Sales Report  ");

        created.Name.Should().Be("Sales Report");
    }

    [Fact]
    public async Task ListAsync_ShouldReturnItems_FromRepository()
    {
        var expected = new List<Template> { new() { Name = "A" }, new() { Name = "B" } } as IReadOnlyList<Template>;
        var repo = new Mock<ITemplateRepository>();
        repo.Setup(r => r.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var sut = new TemplateService(repo.Object);
        var list = await sut.ListAsync();

        list.Should().BeEquivalentTo(expected);
    }
}

