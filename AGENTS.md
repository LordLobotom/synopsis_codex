# Repository Guidelines

## Project Structure & Module Organization
- `docs/` — Canonical specs: requirements, architecture, stack, APIs, roadmap. Keep synchronized with code changes.
- `docs_old/` — Legacy reference only. Do not modify; migrate content into `docs/` if still relevant.
- `src/` — Clean Architecture layout:
  - `ReportGenerator.App` (WPF, MVVM, composition root)
  - `ReportGenerator.Application` (use cases/CQRS, DTOs)
  - `ReportGenerator.Domain` (entities, value objects, interfaces)
  - `ReportGenerator.Infrastructure` (EF Core, Dapper, external services)
- `tests/` — xUnit test projects mirroring `src/` (e.g., `ReportGenerator.Domain.Tests`).

## Build, Test, and Development Commands
- Environment: .NET 8 SDK. WPF requires Windows (Windows Desktop SDK). Use VS 2022 or Rider.
- Cross‑platform build (no WPF): `dotnet build ReportGenerator.CrossPlatform.slnf`
- Cross‑platform tests: `dotnet test ReportGenerator.CrossPlatform.slnf`
- Full build on Windows: `dotnet build`
- Run app (Windows): `dotnet run --project src/ReportGenerator.App`
- EF Core tools (infra): `dotnet ef migrations add <Name>`; `dotnet ef database update`.

## Coding Style & Naming Conventions
- C# 12, nullable enabled, 4-space indentation.
- Naming: `PascalCase` (types, methods), `_camelCase` (private fields), `camelCase` (locals/params).
- One public type per file; filename matches type (e.g., `TemplateService.cs`).
- Use MVVM with CommunityToolkit.Mvvm source generators; DI via `Microsoft.Extensions.Hosting`.
- Analyzers: `StyleCop.Analyzers`, `Roslynator` (dev-only).

## Testing Guidelines
- Frameworks: xUnit + FluentAssertions + Moq.
- Structure: one test class per SUT; fast, isolated unit tests (no network/IO).
- Naming: `MethodName_ShouldExpectedBehavior_WhenCondition`.
- Coverage: prioritize Application/Domain; add integration tests for Infrastructure.

## Commit & Pull Request Guidelines
- Conventional Commits (e.g., `feat(ui): add template designer toolbar`, `fix(db): correct migration ordering`).
- PRs: clear description, linked issues, screenshots for UI, steps to validate, and references to updated docs in `docs/`.
- Require at least one review; ensure tests pass and logging is in place for new features.

## Security & Configuration Tips
- Do not commit secrets. Use environment variables or `dotnet user-secrets` for local dev. Follow MIT/Apache dependencies per stack.

## Agent-Specific Instructions
- Treat `docs/` as the single source of truth. Keep changes minimal and targeted; prefer `dotnet` CLI and NuGet packages specified in `03_Technology_Stack.md`. Avoid editing `docs_old/`.
