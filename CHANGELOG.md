# Changelog

All notable changes to this project are documented here.

## [0.1.1] - 2025-10-01
- Added NCalc-based expression evaluator (Infrastructure).
- Added unit tests for evaluator and SQLite repository integration.
- Added cross-platform solution filter for WSL/Linux builds.
- Improved AGENTS.md with cross-platform commands.

## [0.2.0] - 2025-10-01
- Migrated to NCalc.Core 4.3.0 (AST + visitor). Removed usage of deprecated `NCalc.Expression` API.
- Extended evaluator with common functions (math/string/date/logic/conversion) and updated docs.
- WPF App: single-window drag-and-drop designer (Text/Expr/Rect/Barcode), parameters JSON, Save layout, Print canvas.
- Added ZXing barcode rendering (CODE_128, CODE_39, EAN_13, QR_CODE) in designer.
- Added Serilog.Settings.Configuration to support `.ReadFrom.Configuration`.

## [0.3.0] - 2025-10-01
- Added `ReportGenerator.Web` (Blazor Server) with a simple, stable template editor and live preview using NCalc.
- Export endpoint: `GET /api/templates/{id}/export` returns JSON payload for downstream apps.
- Updated docs: Technology Stack, PROGRESS.md, and docs README for Web UI.

## [0.1.0] - 2025-10-01
- Initial MVP scaffold: Clean Architecture solution, WPF app shell, DI, EF Core (SQLite), Serilog logging.
- Domain entities and interfaces; Application services; Infrastructure DbContext and repository.
- xUnit test project with initial TemplateService tests.
