using Microsoft.AspNetCore.Mvc;
using ReportGenerator.Domain.Interfaces;

namespace ReportGenerator.Web.Controllers;

[ApiController]
[Route("api/templates")] 
public class TemplatesController : ControllerBase
{
    private readonly ITemplateRepository _repo;
    public TemplatesController(ITemplateRepository repo) { _repo = repo; }

    [HttpGet("{id:guid}/export")]
    public async Task<IActionResult> Export(Guid id, CancellationToken ct)
    {
        var tpl = await _repo.GetByIdAsync(id, ct);
        if (tpl == null) return NotFound();
        var payload = new
        {
            tpl.Id,
            tpl.Name,
            Body = tpl.Description,
            ExportedAtUtc = DateTime.UtcNow
        };
        return File(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(payload, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }), "application/json", $"template-{tpl.Id}.json");
    }
}

