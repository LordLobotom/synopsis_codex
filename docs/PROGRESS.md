# Project Progress

Date: 2025-10-01

## Completed
- Solution scaffold with Clean Architecture.
- WPF app shell with DI and Serilog.
- EF Core SQLite context and `TemplateRepository`.
- `TemplateService` with create/list.
- xUnit tests (TemplateService).
- Expression evaluator (NCalc) + unit tests.
- SQLite repository integration test.
- Cross-platform solution filter for WSL builds.

## In Progress / Next
- Material Design theme and MVVM shell (Windows).
- Seed sample templates for demo.
- Switch to EF Core migrations and add `InitialCreate`.
- Designer surface prototype (Phase 1 MVP).

## Notes
- Build full solution on Windows. Use `ReportGenerator.CrossPlatform.slnf` on Linux/WSL.
- Logs are written to `Logs/app-<date>.log`.
