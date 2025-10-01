using System;

namespace ReportGenerator.Domain.Entities;

public class Template
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
}

