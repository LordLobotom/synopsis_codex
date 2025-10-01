# Project Progress

Date: 2025-10-01

## Completed
- Clean Architecture solution scaffold (Domain, Application, Infrastructure, WPF App).
- DI + Serilog logging, logs to `Logs/app-<date>.log`.
- EF Core (SQLite) `AppDbContext` and `TemplateRepository` (Add/List/Get) with integration test.
- `TemplateService` (Create/List) with unit tests.
- Expression engine: migrated to `NCalc.Core` 4.3.0 (AST + visitor), evaluator implemented in Infrastructure.
- Added common NCalc functions: IF/IIF, COALESCE, LEFT/RIGHT/SUBSTRING/MID, LEN, CONCAT, CONTAINS, STARTSWITH, ENDSWITH, ABS, MIN/MAX, ROUND/FLOOR/CEILING, NOW, DATE, type casts.
- WPF single‑window designer (Phase 1): drag‑and‑drop canvas (A4), toolbox (Text/Expr/Rectangle/Barcode), parameters JSON, Save layout, Print canvas.
- Barcode support (ZXing): CODE_128, CODE_39, EAN_13, QR_CODE; content can be expression‑driven.
- Cross‑platform build filter `ReportGenerator.CrossPlatform.slnf` validated under WSL.

## Web UI Pivot (Blazor Server)
- Added `ReportGenerator.Web` (Blazor Server) with a simple, stable template editor and live preview.
- Reused EF Core (SQLite), `TemplateService`, and NCalc evaluator on server for preview.
- Export endpoint: `GET /api/templates/{id}/export` returns JSON template.

## In Progress / Next
- Web designer UX: drag/resize canvas, properties panel, snap‑to‑grid, alignment guides.
- Properties panel: bold/italic/align, colors, fonts; inline text editing.
- Persistence: move layout JSON to a dedicated table/column; add migrations (InitialCreate).
- Export: XPS/PDF export (consider QuestPDF), multi‑page pagination.
- Optional Avalonia UI variant to enable cross‑platform designer.
- Seed a few sample templates (invoice, label with barcode) for demo.

## Notes
- Build full solution on Windows; use `ReportGenerator.CrossPlatform.slnf` on WSL/Linux.
- NCalc 4.x removes `NCalc.Expression`; we use AST + visitor approach.
