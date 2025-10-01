# Report Generator - System Requirements Specification (SRS)

**Project Name:** Modern Report Generator (Successor to SINEA Report Generator)
**Version:** 1.0.0
**Date:** October 1, 2025
**Status:** Initial Design Phase

---

## 1. Executive Summary

This document specifies requirements for a modern report generation system that replaces the legacy SINEA Report Generator (Delphi/VCL) with a contemporary .NET 8/WPF application. The system will maintain core functionality while adopting modern architecture patterns, improved user experience, and extensible design.

---

## 2. Project Overview

### 2.1 Project Goals
- **Modernize** legacy Delphi report generator to .NET 8/WPF
- **Maintain** all critical features from original system
- **Improve** user experience with modern UI/UX
- **Enable** future extensibility and cloud integration
- **Provide** better performance and maintainability

### 2.2 Success Criteria
- ✅ Feature parity with legacy system (core functions)
- ✅ 50%+ reduction in report design time
- ✅ Support for 100K+ records without performance degradation
- ✅ Modern, intuitive UI matching contemporary design standards
- ✅ Comprehensive automated test coverage (80%+)

### 2.3 Scope

#### In Scope (Phase 1 - MVP)
- Visual drag-and-drop report designer
- Database connectivity (MSSQL via EF Core)
- Template storage (SQLite)
- Expression engine with 43+ functions
- Barcode/QR code generation
- Master-detail reports
- Grouping and aggregates
- Print preview and Windows printer output
- Image export (PNG/JPEG)
- Basic XAML view for advanced users

#### Future Phases (Not MVP)
- PDF export (Phase 2)
- Charts and graphs (Phase 2)
- Email integration (Phase 3)
- Cloud storage for templates (Phase 3)
- User permissions/roles (Phase 3)
- Web-based report viewer (Phase 4)
- Multi-database support (PostgreSQL, MySQL, Oracle) (Phase 4)
- Plugin system (Phase 5)

#### Out of Scope
- Mobile application (native iOS/Android)
- Real-time collaborative editing
- AI-powered report generation (future consideration)

---

## 3. Stakeholders

### 3.1 Primary Users
- **Report Designers** - Create and maintain report templates
- **Business Analysts** - Configure data sources and expressions
- **End Users** - Generate reports from templates
- **System Administrators** - Deploy and maintain application

### 3.2 Development Team
- **Developers** - C#/.NET developers
- **AI Coding Assistants** - Claude, GitHub Copilot
- **QA Engineers** - Automated and manual testing

---

## 4. Functional Requirements

### 4.1 Report Designer (FR-DESIGNER)

#### FR-DESIGNER-001: Visual Designer Interface
**Priority:** Critical
**Description:** Provide drag-and-drop visual report designer
**Acceptance Criteria:**
- User can add elements via toolbox
- Elements can be dragged, resized, aligned
- Properties panel shows element configuration
- Grid and ruler for precise positioning
- Undo/redo support (20+ actions)
- Multi-select and bulk operations
- Copy/paste elements

#### FR-DESIGNER-002: Report Sections
**Priority:** Critical
**Description:** Support standard report sections
**Acceptance Criteria:**
- Report Header (once at start)
- Page Header (top of each page)
- Detail/Body (per record)
- Page Footer (bottom of each page)
- Report Footer (once at end)
- Group Headers/Footers (unlimited groups)
- Section height adjustable
- Section visibility conditional

#### FR-DESIGNER-003: Report Elements
**Priority:** Critical
**Description:** Support various report element types
**Element Types:**
- Static Label (text)
- Database Field (bound to data)
- Calculated Field (expressions)
- Line (horizontal/vertical)
- Rectangle
- Rounded Rectangle
- Ellipse/Circle
- Image (static or dynamic)
- Barcode (Code39, Code128, EAN, UPC, etc.)
- QR Code
- PaintBox (custom drawing area)
- Sub-report (master-detail)

**Acceptance Criteria:**
- All element types render correctly
- Properties: position, size, font, color, border, alignment
- Conditional visibility
- Expression binding for dynamic content

#### FR-DESIGNER-004: XAML View
**Priority:** Medium
**Description:** Provide code view for advanced users
**Acceptance Criteria:**
- View template as XAML or custom DSL
- Syntax highlighting
- Real-time validation
- Switch between visual and code views
- Direct editing with immediate visual update

#### FR-DESIGNER-005: Template Management
**Priority:** Critical
**Description:** Save, load, and manage report templates
**Acceptance Criteria:**
- Save template to database (SQLite)
- Load existing template
- Template versioning (track changes)
- Template metadata (name, description, author, date)
- Template search and filtering
- Template export/import (file-based backup)

### 4.2 Data Binding (FR-DATA)

#### FR-DATA-001: MSSQL Connectivity
**Priority:** Critical
**Description:** Connect to Microsoft SQL Server databases
**Acceptance Criteria:**
- Connection string configuration
- Test connection capability
- Support for Windows and SQL authentication
- Connection pooling
- Timeout configuration
- Error handling and retry logic

#### FR-DATA-002: Data Source Configuration
**Priority:** Critical
**Description:** Configure data sources for reports
**Acceptance Criteria:**
- Direct SQL query support
- Stored procedure support
- Table/view selection
- Query builder (visual)
- Parameter support (@param syntax)
- Preview data (first 100 rows)

#### FR-DATA-003: Field Mapping
**Priority:** Critical
**Description:** Map database fields to report elements
**Acceptance Criteria:**
- Drag-and-drop field to element
- Auto-detect field type (string, int, date, decimal, etc.)
- Format string configuration
- Null value handling
- Field list refreshable

#### FR-DATA-004: Master-Detail Relationships
**Priority:** High
**Description:** Support hierarchical data relationships
**Acceptance Criteria:**
- Define master-detail link (foreign key)
- Nested sub-reports
- Detail data filtered by master record
- Multiple detail levels (3+ deep)

### 4.3 Expression Engine (FR-EXPR)

#### FR-EXPR-001: Expression Evaluator
**Priority:** Critical
**Description:** Evaluate expressions for calculated fields
**Acceptance Criteria:**
- Arithmetic operations (+, -, *, /, %, ^)
- Comparison operators (=, <>, <, >, <=, >=)
- Logical operators (AND, OR, NOT)
- Field references ([FieldName])
- String concatenation
- Parentheses for precedence
- Type coercion (string to number, etc.)

#### FR-EXPR-002: Built-in Functions
**Priority:** Critical
**Description:** Provide comprehensive function library
**Function Categories:**

**Mathematical (15 functions):**
- ABS(x), CEILING(x), FLOOR(x), ROUND(x, decimals)
- SQRT(x), POWER(x, y), EXP(x), LOG(x), LOG10(x)
- SIN(x), COS(x), TAN(x), ASIN(x), ACOS(x), ATAN(x)

**String (20 functions):**
- LEFT(s, n), RIGHT(s, n), MID(s, start, len)
- UPPER(s), LOWER(s), PROPER(s)
- TRIM(s), LTRIM(s), RTRIM(s)
- LEN(s), REPLACE(s, old, new)
- SUBSTRING(s, start, len), CONTAINS(s, substr)
- STARTSWITH(s, prefix), ENDSWITH(s, suffix)
- CONCAT(s1, s2, ...), FORMAT(fmt, val)
- PADLEFT(s, len, char), PADRIGHT(s, len, char)
- REVERSE(s), INDEXOF(s, substr)

**Date/Time (15 functions):**
- NOW(), TODAY(), YEAR(d), MONTH(d), DAY(d)
- HOUR(t), MINUTE(t), SECOND(t)
- DATEADD(d, n, unit), DATEDIFF(d1, d2, unit)
- DATEFORMAT(d, fmt), TIMEFORMAT(t, fmt)
- WEEKDAY(d), WEEKNUM(d), QUARTER(d)

**Aggregate (8 functions):**
- SUM([Field]), AVG([Field]), COUNT([Field])
- MIN([Field]), MAX([Field])
- STDEV([Field]), VAR([Field])
- COUNTDISTINCT([Field])

**Conditional (5 functions):**
- IF(cond, true_val, false_val)
- ISNULL(val, default), ISEMPTY(val)
- COALESCE(val1, val2, ...), NULLIF(val1, val2)

**Conversion (5 functions):**
- TOSTRING(val), TONUMBER(val), TODATE(val)
- TOBOOLEAN(val), CAST(val, type)

**Acceptance Criteria:**
- All 68 functions implemented
- Function help/documentation in UI
- Error messages for invalid syntax
- IntelliSense/auto-complete in expression editor

#### FR-EXPR-003: Custom Functions
**Priority:** Medium
**Description:** Allow user-defined functions
**Acceptance Criteria:**
- C# code snippets (Roslyn compilation)
- Function library management
- Reusable across templates
- Parameter passing
- Return type specification

### 4.4 Barcode/QR Code (FR-BARCODE)

#### FR-BARCODE-001: Barcode Generation
**Priority:** High
**Description:** Generate standard barcodes
**Supported Types:**
- Code 39, Code 39 Extended
- Code 128 (A, B, C)
- Code 93, Code 93 Extended
- EAN-8, EAN-13, EAN-128
- UPC-A, UPC-E
- Interleaved 2 of 5
- Codabar, MSI, PostNet

**Acceptance Criteria:**
- Render to image
- Configurable size (module width)
- Checksum calculation (optional)
- Human-readable text (optional)
- Rotation support (0°, 90°, 180°, 270°)

#### FR-BARCODE-002: QR Code Generation
**Priority:** High
**Description:** Generate QR codes
**Acceptance Criteria:**
- Error correction levels (L, M, Q, H)
- Size scaling
- Encoding support (UTF-8)
- Dynamic content from fields
- Color customization (foreground/background)

### 4.5 Grouping & Aggregates (FR-GROUP)

#### FR-GROUP-001: Data Grouping
**Priority:** High
**Description:** Group report data by fields
**Acceptance Criteria:**
- Multiple group levels (unlimited)
- Group header/footer sections
- Break on field value change
- Sort within groups
- Keep together (page break handling)

#### FR-GROUP-002: Aggregate Calculations
**Priority:** High
**Description:** Calculate aggregates per group/report
**Acceptance Criteria:**
- Sum, Average, Count, Min, Max
- Running totals
- Percentage of total
- Group-level and report-level aggregates
- Reset on group change

### 4.6 Output & Rendering (FR-OUTPUT)

#### FR-OUTPUT-001: Print Preview
**Priority:** Critical
**Description:** Preview report before printing
**Acceptance Criteria:**
- Page-by-page navigation
- Zoom controls (50%-400%)
- Fit to width/height
- Thumbnail view
- Page count display
- Search within preview

#### FR-OUTPUT-002: Windows Printer Output
**Priority:** Critical
**Description:** Print to Windows printer
**Acceptance Criteria:**
- Printer selection dialog
- Page range (all, current, range)
- Number of copies
- Collation support
- Orientation (portrait/landscape)
- Paper size selection
- Margin configuration

#### FR-OUTPUT-003: Image Export
**Priority:** High
**Description:** Export report pages as images
**Acceptance Criteria:**
- PNG format (lossless)
- JPEG format (lossy, configurable quality)
- Resolution configuration (DPI)
- Single page or all pages
- Filename template (e.g., Report_Page{n}.png)

### 4.7 User Interface (FR-UI)

#### FR-UI-001: Modern UI Design
**Priority:** High
**Description:** Provide contemporary user interface
**Acceptance Criteria:**
- Material Design theme (MaterialDesignInXAML)
- Responsive layout
- Consistent color scheme
- Icon set (Material Design Icons)
- Smooth animations and transitions
- Loading indicators for long operations

#### FR-UI-002: Keyboard Shortcuts
**Priority:** Medium
**Description:** Support power user shortcuts
**Common Shortcuts:**
- Ctrl+N: New template
- Ctrl+O: Open template
- Ctrl+S: Save template
- Ctrl+Z: Undo
- Ctrl+Y: Redo
- Ctrl+C/V/X: Copy/Paste/Cut
- Ctrl+A: Select all
- Delete: Delete selected
- Ctrl+P: Print preview
- F5: Refresh data

---

## 5. Non-Functional Requirements

### 5.1 Performance (NFR-PERF)

#### NFR-PERF-001: Data Handling
- Support 100,000 records without performance degradation
- Report generation: < 5 seconds for 1,000 records
- Report generation: < 30 seconds for 10,000 records
- Report generation: < 5 minutes for 100,000 records

#### NFR-PERF-002: UI Responsiveness
- Designer operations: < 100ms response time
- Template load: < 2 seconds
- Data preview: < 3 seconds
- No UI freezing during long operations (async/await)

#### NFR-PERF-003: Memory Usage
- Idle memory: < 200 MB
- Active designer: < 500 MB
- Large report (100K records): < 2 GB
- Memory cleanup after report generation

### 5.2 Reliability (NFR-REL)

#### NFR-REL-001: Application Stability
- Zero unhandled exceptions
- Graceful error handling with user-friendly messages
- Auto-save every 5 minutes
- Crash recovery (restore last session)

#### NFR-REL-002: Data Integrity
- Database transactions for template saves
- Concurrent access handling
- Backup and restore capabilities
- Data validation on all inputs

### 5.3 Maintainability (NFR-MAINT)

#### NFR-MAINT-001: Code Quality
- Clean Architecture principles
- SOLID principles
- 80%+ code coverage (unit tests)
- XML documentation for public APIs
- Consistent naming conventions (Microsoft C# standards)

#### NFR-MAINT-002: Logging & Diagnostics
- Structured logging (Serilog)
- Log levels: Trace, Debug, Info, Warning, Error, Fatal
- Log to file (rolling, 7-day retention)
- Performance metrics logging
- Crash dump generation

### 5.4 Usability (NFR-USABILITY)

#### NFR-USABILITY-001: Learning Curve
- New user can create basic report in < 15 minutes
- Comprehensive in-app help system
- Tooltips for all UI elements
- Tutorial/wizard for first-time users

#### NFR-USABILITY-002: Accessibility
- Keyboard navigation for all features
- Screen reader compatible (UI Automation)
- High contrast mode support
- Font size adjustable

### 5.5 Compatibility (NFR-COMPAT)

#### NFR-COMPAT-001: Platform Support
- Windows 10 version 1809 or higher
- Windows 11
- Windows Server 2019 or higher

#### NFR-COMPAT-002: .NET Runtime
- .NET 8.0 Runtime required
- Self-contained deployment option available

#### NFR-COMPAT-003: Database Support
- SQL Server 2016 or higher (production data)
- SQLite 3.x (template storage)

### 5.6 Security (NFR-SEC)

#### NFR-SEC-001: Data Security
- Connection strings encrypted (DPAPI)
- SQLite database encrypted at rest (optional)
- No sensitive data in logs
- SQL injection prevention (parameterized queries)

#### NFR-SEC-002: Application Security
- Code signing certificate
- Administrator elevation when needed
- Safe expression evaluation (no code execution exploits)

---

## 6. System Constraints

### 6.1 Technical Constraints
- **Platform:** Windows only (WPF limitation)
- **Database:** MSSQL primary, SQLite for templates
- **Language:** C# (primary), F# (optional for specific modules)
- **.NET Version:** .NET 8 LTS
- **UI Framework:** WPF with Material Design

### 6.2 Resource Constraints
- **Development Time:** MVP in 6 months (estimated)
- **Team Size:** 1-2 developers + AI assistants
- **Budget:** Open-source libraries only (no commercial licenses initially)

### 6.3 Regulatory Constraints
- None for MVP (future GDPR compliance for cloud features)

---

## 7. Assumptions and Dependencies

### 7.1 Assumptions
- Target users have Windows 10+ with .NET 8 runtime
- Users have basic SQL knowledge for data source configuration
- MSSQL servers are accessible via network (production)
- Legacy SINEA templates will not be auto-migrated (manual recreation)

### 7.2 Dependencies
- **External Libraries:** NuGet packages (EF Core, NCalc, ZXing.Net, etc.)
- **Database:** MSSQL server availability
- **Development Tools:** Visual Studio 2022, Rider, or VS Code
- **AI Assistants:** Claude, GitHub Copilot for accelerated development

---

## 8. Success Metrics

### 8.1 Quantitative Metrics
- **Feature Completeness:** 100% of MVP requirements implemented
- **Test Coverage:** ≥ 80% code coverage
- **Performance:** All NFR-PERF targets met
- **Bug Density:** < 1 critical bug per 1,000 LOC
- **User Adoption:** 10+ active users within 3 months of launch

### 8.2 Qualitative Metrics
- **User Satisfaction:** ≥ 4.0/5.0 rating (user surveys)
- **Code Maintainability:** A-grade SonarQube analysis
- **Documentation Quality:** Complete and clear for all features
- **AI Assistant Effectiveness:** 60%+ of code generated with AI assistance

---

## 9. Risks and Mitigations

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| Performance issues with large datasets | Medium | High | Early load testing, pagination, streaming |
| UI complexity overwhelming users | Medium | Medium | Iterative UX testing, wizard-based flows |
| Expression engine security vulnerabilities | Low | High | Sandboxed evaluation, input validation |
| Third-party library breaking changes | Low | Medium | Pin library versions, test updates |
| Database schema evolution issues | Medium | Medium | EF Core migrations, versioning |

---

## 10. Approval

| Role | Name | Signature | Date |
|------|------|-----------|------|
| Product Owner | [TBD] | | |
| Lead Developer | [TBD] | | |
| QA Lead | [TBD] | | |

---

## 11. Document History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0.0 | 2025-10-01 | Claude AI | Initial SRS document |

---

## 12. Appendices

### Appendix A: Glossary
- **MVP:** Minimum Viable Product
- **MSSQL:** Microsoft SQL Server
- **EF Core:** Entity Framework Core
- **MVVM:** Model-View-ViewModel
- **WPF:** Windows Presentation Foundation
- **ORM:** Object-Relational Mapping

### Appendix B: References
- Legacy SINEA Documentation (Synopsis/docs old/)
- .NET 8 Documentation (https://learn.microsoft.com/dotnet/)
- Material Design Guidelines (https://material.io/)
- Clean Architecture Principles (Robert C. Martin)
