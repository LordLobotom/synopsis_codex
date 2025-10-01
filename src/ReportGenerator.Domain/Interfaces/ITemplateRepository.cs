using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ReportGenerator.Domain.Entities;

namespace ReportGenerator.Domain.Interfaces;

public interface ITemplateRepository
{
    Task<Template?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Template>> ListAsync(CancellationToken ct = default);
    Task<Template> AddAsync(Template template, CancellationToken ct = default);
    Task UpdateDescriptionAsync(Guid id, string? description, CancellationToken ct = default);
    Task UpdateNameAsync(Guid id, string name, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
