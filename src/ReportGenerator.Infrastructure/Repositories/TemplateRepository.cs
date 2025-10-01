using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReportGenerator.Domain.Entities;
using ReportGenerator.Domain.Interfaces;
using ReportGenerator.Infrastructure.Data;

namespace ReportGenerator.Infrastructure.Repositories;

public class TemplateRepository : ITemplateRepository
{
    private readonly AppDbContext _db;

    public TemplateRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<Template?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Templates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Template>> ListAsync(CancellationToken ct = default)
        => await _db.Templates.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);

    public async Task<Template> AddAsync(Template template, CancellationToken ct = default)
    {
        _db.Templates.Add(template);
        await _db.SaveChangesAsync(ct);
        return template;
    }

    public async Task UpdateDescriptionAsync(Guid id, string? description, CancellationToken ct = default)
    {
        var entity = await _db.Templates.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (entity is null) return;
        entity.Description = description;
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateNameAsync(Guid id, string name, CancellationToken ct = default)
    {
        var entity = await _db.Templates.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (entity is null) return;
        entity.Name = name;
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Templates.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (entity is null) return;
        _db.Templates.Remove(entity);
        await _db.SaveChangesAsync(ct);
    }
}
