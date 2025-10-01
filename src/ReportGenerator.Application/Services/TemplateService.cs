using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ReportGenerator.Domain.Entities;
using ReportGenerator.Domain.Interfaces;

namespace ReportGenerator.Application.Services;

public class TemplateService
{
    private readonly ITemplateRepository _repository;

    public TemplateService(ITemplateRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<Template>> ListAsync(CancellationToken ct = default)
        => _repository.ListAsync(ct);

    public async Task<Template> CreateAsync(string name, string? description = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required", nameof(name));

        var template = new Template
        {
            Name = name.Trim(),
            Description = description?.Trim()
        };

        return await _repository.AddAsync(template, ct);
    }

    public Task UpdateDescriptionAsync(Guid id, string? description, CancellationToken ct = default)
        => _repository.UpdateDescriptionAsync(id, description, ct);

    public Task UpdateNameAsync(Guid id, string name, CancellationToken ct = default)
        => _repository.UpdateNameAsync(id, name, ct);

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
        => _repository.DeleteAsync(id, ct);
}
