# Report Generator (MVP)

Create and manage printable templates (labels, stickers, slips) with a drag‑and‑drop designer. Export templates to JSON so other apps can render/print them with real data.

## What’s Included

- Web Designer (Blazor Server): modern, responsive UI for building templates
- Persistence: templates stored in SQLite (created/seeding on first run)
- Expressions: NCalc 4.x for previewing dynamic values (e.g., `{{ CustomerName }}`)
- Export: JSON schema (dimensions + elements) ready for downstream apps
- WPF App (optional): simple editor/preview window for Windows

## Quick Start

Prereqs: .NET 8 SDK (Windows or WSL/Linux)

- Run the Web Designer (recommended)
  - `dotnet run --project src/ReportGenerator.Web`
  - Open http://localhost:5000
  - A sample template is seeded on first run

- Optional: Run the WPF app (Windows only)
  - `dotnet run --project src/ReportGenerator.App`

- Cross‑platform build + tests
  - `dotnet build ReportGenerator.CrossPlatform.slnf`
  - `dotnet test ReportGenerator.CrossPlatform.slnf`

## Using the Web Designer

- Canvas
  - Set Canvas W/H (px) in the toolbar (e.g., stickers/labels). Saved with the template.
- Elements
  - Add Text / Expr / Rect; drag to position
  - Click an element to select; edit properties (Name, X, Y, W, H; Text or Expression)
  - Elements list (right panel) lets you select, reorder, delete
- Parameters (preview only)
  - JSON parameters evaluate expressions during preview (e.g., `{ "CustomerName": "John", "Total": 123.45 }`)
  - These are not persisted as output data — they are only for previewing expressions in the designer
- Save & Export
  - Save persists the complete document (dimensions + elements) to SQLite
  - Export JSON: `GET /api/templates/{id}/export` returns a self‑contained TemplateDoc JSON

## Exported JSON Schema (TemplateDoc)

```json
{
  "w": 794,
  "h": 1123,
  "elements": [
    { "id": "GUID", "name": "Text 1", "kind": "Text", "x": 50, "y": 50, "w": 160, "h": 28, "text": "Label" },
    { "id": "GUID", "name": "Expr 1", "kind": "Expression", "x": 50, "y": 90, "w": 200, "h": 28, "expr": "ROUND(Total,2)" },
    { "id": "GUID", "name": "Rect 1", "kind": "Rectangle", "x": 40, "y": 140, "w": 200, "h": 60 }
  ],
  "dataSource": {
    "provider": "SqlServer",
    "connectionName": "Default",
    "query": "SELECT * FROM Orders WHERE Id=@OrderId",
    "parameters": { "OrderId": 123 }
  }
}
```

Notes:
- `w`, `h` are canvas dimensions in pixels
- `elements` define positions/sizes and content; `expr` is evaluated with NCalc
- `dataSource` is optional (placeholder). Resolve connection by name in your app and supply real parameters. Do not embed secrets in templates.

## Printing With SQL Data (downstream app)

Typical flow for your app that consumes the exported JSON:
1) Load the TemplateDoc JSON
2) Resolve a connection string by `connectionName` (e.g., from app settings)
3) Execute `dataSource.query` with parameters to fetch records
4) For each record:
   - Build a parameters dictionary from fields
   - For elements with `expr`, evaluate with NCalc to produce display text
   - For elements with `text`, use the literal
   - Render onto a canvas/PDF at (`x`,`y`,`w`,`h`) and send to printer

We can provide a .NET rendering library + PDF export (QuestPDF) as a next step.

## Developer Guide

- Solution structure
  - `src/ReportGenerator.Domain` — core entities, interfaces
  - `src/ReportGenerator.Application` — services/use cases
  - `src/ReportGenerator.Infrastructure` — EF Core (SQLite), repositories, NCalc evaluator
  - `src/ReportGenerator.Web` — Blazor Server web designer
  - `src/ReportGenerator.App` — WPF editor/preview (Windows)
  - `tests/ReportGenerator.Domain.Tests` — unit & integration tests

- VS Code
  - Use C# or C# Dev Kit extension
  - Run Web: `dotnet run --project src/ReportGenerator.Web`
  - Run WPF: `dotnet run --project src/ReportGenerator.App`

## Roadmap (MVP → Production)

- Designer UX: snap‑to‑grid, resize handles, alignment guides, keyboard nudging
- Elements: barcode, image, font/size/color, z‑order
- Data: SQL preview (sample record), connection management (no secrets), parameter prompts
- Export: server‑side PDF (QuestPDF), printer presets (labels), multi‑page
- Governance: versioning, import/export, asset library

## Notes

- Expression engine uses NCalc.Core 4.x (AST + visitor). No `NCalc.Expression` API.
- On first web run, the app creates `templates.db` and seeds a sample template.
- Do not commit secrets; use environment variables or `dotnet user-secrets`.
