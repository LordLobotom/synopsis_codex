# Report Generator - API & Interface Specifications

**Version:** 1.0.0
**Date:** October 1, 2025
**Status:** Design Phase

---

## 1. Overview

This document defines all public interfaces, contracts, and APIs for the Report Generator system. Following **Clean Architecture** and **SOLID principles**, all dependencies point inward through well-defined interfaces.

---

## 2. Domain Layer Interfaces

### 2.1 Repository Interfaces

#### **ITemplateRepository**

Template persistence operations.

```csharp
namespace ReportGenerator.Domain.Repositories;

/// <summary>
/// Repository for managing report templates
/// </summary>
public interface ITemplateRepository
{
    /// <summary>
    /// Gets a template by its unique identifier
    /// </summary>
    /// <param name="id">Template ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Template if found, null otherwise</returns>
    Task<Template?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active templates
    /// </summary>
    /// <param name="includeInactive">Include inactive templates</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of templates</returns>
    Task<IEnumerable<Template>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches templates by name, description, or tags
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Matching templates</returns>
    Task<IEnumerable<Template>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets templates modified after specified date
    /// </summary>
    /// <param name="since">Date to filter from</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Modified templates</returns>
    Task<IEnumerable<Template>> GetModifiedSinceAsync(DateTime since, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new template
    /// </summary>
    /// <param name="template">Template to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Generated template ID</returns>
    Task<Guid> AddAsync(Template template, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing template
    /// </summary>
    /// <param name="template">Template to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateAsync(Template template, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a template (soft delete - marks as inactive)
    /// </summary>
    /// <param name="id">Template ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Permanently deletes a template
    /// </summary>
    /// <param name="id">Template ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task HardDeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if template name already exists
    /// </summary>
    /// <param name="name">Template name</param>
    /// <param name="excludeId">Template ID to exclude (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if name exists</returns>
    Task<bool> ExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
```

---

#### **IDataSourceRepository**

Data source configuration persistence.

```csharp
namespace ReportGenerator.Domain.Repositories;

public interface IDataSourceRepository
{
    Task<DataSource?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<DataSource>> GetByTemplateIdAsync(Guid templateId, CancellationToken cancellationToken = default);
    Task<Guid> AddAsync(DataSource dataSource, CancellationToken cancellationToken = default);
    Task UpdateAsync(DataSource dataSource, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> TestConnectionAsync(Guid id, CancellationToken cancellationToken = default);
}
```

---

#### **IConnectionStringRepository**

Connection string management.

```csharp
namespace ReportGenerator.Domain.Repositories;

public interface IConnectionStringRepository
{
    Task<ConnectionString?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ConnectionString?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<ConnectionString>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<Guid> AddAsync(ConnectionString connectionString, CancellationToken cancellationToken = default);
    Task UpdateAsync(ConnectionString connectionString, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
```

---

### 2.2 Domain Service Interfaces

#### **IExpressionEvaluator**

Expression parsing and evaluation.

```csharp
namespace ReportGenerator.Domain.Services;

/// <summary>
/// Service for evaluating report expressions
/// </summary>
public interface IExpressionEvaluator
{
    /// <summary>
    /// Evaluates an expression with given context
    /// </summary>
    /// <param name="expression">Expression string</param>
    /// <param name="context">Variable context</param>
    /// <returns>Evaluation result</returns>
    /// <exception cref="ExpressionEvaluationException">If evaluation fails</exception>
    ExpressionResult Evaluate(string expression, ExpressionContext context);

    /// <summary>
    /// Parses expression without evaluating
    /// </summary>
    /// <param name="expression">Expression string</param>
    /// <returns>Parse result with syntax tree</returns>
    ParseResult Parse(string expression);

    /// <summary>
    /// Validates expression syntax
    /// </summary>
    /// <param name="expression">Expression string</param>
    /// <returns>Validation result</returns>
    ValidationResult Validate(string expression);

    /// <summary>
    /// Gets list of available functions
    /// </summary>
    /// <returns>Function metadata</returns>
    IEnumerable<FunctionInfo> GetAvailableFunctions();

    /// <summary>
    /// Registers a custom function
    /// </summary>
    /// <param name="name">Function name</param>
    /// <param name="function">Function delegate</param>
    /// <param name="description">Function description</param>
    void RegisterFunction(string name, Delegate function, string description);

    /// <summary>
    /// Unregisters a custom function
    /// </summary>
    /// <param name="name">Function name</param>
    /// <returns>True if function was removed</returns>
    bool UnregisterFunction(string name);
}

/// <summary>
/// Context for expression evaluation
/// </summary>
public class ExpressionContext
{
    /// <summary>
    /// Variable values
    /// </summary>
    public Dictionary<string, object> Variables { get; set; } = new();

    /// <summary>
    /// Current data row (for field references)
    /// </summary>
    public object? CurrentRow { get; set; }

    /// <summary>
    /// Aggregate values (for summary calculations)
    /// </summary>
    public Dictionary<string, object> Aggregates { get; set; } = new();

    /// <summary>
    /// Report parameters
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = new();
}

/// <summary>
/// Result of expression evaluation
/// </summary>
public class ExpressionResult
{
    public object? Value { get; set; }
    public Type ResultType { get; set; }
    public bool IsNull { get; set; }
    public TimeSpan EvaluationTime { get; set; }
}

/// <summary>
/// Function metadata
/// </summary>
public class FunctionInfo
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Syntax { get; set; } = string.Empty;
    public string[] Examples { get; set; } = Array.Empty<string>();
    public Type ReturnType { get; set; }
    public ParameterInfo[] Parameters { get; set; } = Array.Empty<ParameterInfo>();
}
```

---

#### **IBarcodeGenerator**

Barcode and QR code generation.

```csharp
namespace ReportGenerator.Domain.Services;

/// <summary>
/// Service for generating barcodes and QR codes
/// </summary>
public interface IBarcodeGenerator
{
    /// <summary>
    /// Generates a barcode image
    /// </summary>
    /// <param name="content">Barcode content</param>
    /// <param name="type">Barcode type</param>
    /// <param name="options">Generation options</param>
    /// <returns>Barcode image as byte array</returns>
    /// <exception cref="BarcodeGenerationException">If generation fails</exception>
    byte[] Generate(string content, BarcodeType type, BarcodeOptions options);

    /// <summary>
    /// Generates a QR code image
    /// </summary>
    /// <param name="content">QR code content</param>
    /// <param name="options">Generation options</param>
    /// <returns>QR code image as byte array</returns>
    byte[] GenerateQRCode(string content, QRCodeOptions options);

    /// <summary>
    /// Validates barcode content for specified type
    /// </summary>
    /// <param name="content">Content to validate</param>
    /// <param name="type">Barcode type</param>
    /// <returns>Validation result</returns>
    ValidationResult Validate(string content, BarcodeType type);

    /// <summary>
    /// Gets supported barcode types
    /// </summary>
    /// <returns>List of supported types</returns>
    IEnumerable<BarcodeTypeInfo> GetSupportedTypes();

    /// <summary>
    /// Calculates checksum for barcode
    /// </summary>
    /// <param name="content">Barcode content</param>
    /// <param name="type">Barcode type</param>
    /// <returns>Checksum character</returns>
    char CalculateChecksum(string content, BarcodeType type);
}

/// <summary>
/// Barcode generation options
/// </summary>
public class BarcodeOptions
{
    public int Width { get; set; } = 200;
    public int Height { get; set; } = 100;
    public int Margin { get; set; } = 10;
    public bool ShowText { get; set; } = true;
    public bool CalculateChecksum { get; set; } = true;
    public int ModuleWidth { get; set; } = 1;
    public int Rotation { get; set; } = 0; // 0, 90, 180, 270
    public string ForegroundColor { get; set; } = "#000000";
    public string BackgroundColor { get; set; } = "#FFFFFF";
}

/// <summary>
/// QR code generation options
/// </summary>
public class QRCodeOptions
{
    public int Size { get; set; } = 200;
    public ErrorCorrectionLevel ErrorCorrection { get; set; } = ErrorCorrectionLevel.Medium;
    public string ForegroundColor { get; set; } = "#000000";
    public string BackgroundColor { get; set; } = "#FFFFFF";
}

public enum BarcodeType
{
    Code39,
    Code39Extended,
    Code93,
    Code93Extended,
    Code128A,
    Code128B,
    Code128C,
    EAN8,
    EAN13,
    EAN128,
    UPCA,
    UPCE,
    Interleaved2Of5,
    Codabar,
    MSI,
    PostNet
}

public enum ErrorCorrectionLevel
{
    Low,    // 7% recovery
    Medium, // 15% recovery
    Quartile, // 25% recovery
    High    // 30% recovery
}
```

---

#### **IReportRenderer**

Report rendering engine.

```csharp
namespace ReportGenerator.Domain.Services;

/// <summary>
/// Service for rendering reports
/// </summary>
public interface IReportRenderer
{
    /// <summary>
    /// Renders a complete report
    /// </summary>
    /// <param name="template">Report template</param>
    /// <param name="dataContext">Data context</param>
    /// <param name="options">Render options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Rendered report</returns>
    Task<RenderResult> RenderAsync(Template template, DataContext dataContext, RenderOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Renders a single page
    /// </summary>
    /// <param name="template">Report template</param>
    /// <param name="dataContext">Data context</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Rendered page</returns>
    Task<PageResult> RenderPageAsync(Template template, DataContext dataContext, int pageNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates total page count without full rendering
    /// </summary>
    /// <param name="template">Report template</param>
    /// <param name="dataContext">Data context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Page count</returns>
    Task<int> CalculatePageCountAsync(Template template, DataContext dataContext, CancellationToken cancellationToken = default);
}

/// <summary>
/// Render options
/// </summary>
public class RenderOptions
{
    public int? StartPage { get; set; }
    public int? EndPage { get; set; }
    public bool IncludeMetadata { get; set; } = true;
    public bool CalculateAggregates { get; set; } = true;
    public int DPI { get; set; } = 96;
}

/// <summary>
/// Render result
/// </summary>
public class RenderResult
{
    public List<PageResult> Pages { get; set; } = new();
    public int TotalPages => Pages.Count;
    public TimeSpan RenderTime { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Single page render result
/// </summary>
public class PageResult
{
    public int PageNumber { get; set; }
    public byte[] ImageData { get; set; } = Array.Empty<byte>();
    public double Width { get; set; }
    public double Height { get; set; }
}
```

---

#### **ILayoutEngine**

Layout calculation engine.

```csharp
namespace ReportGenerator.Domain.Services;

/// <summary>
/// Service for calculating element layouts
/// </summary>
public interface ILayoutEngine
{
    /// <summary>
    /// Calculates layout for a section
    /// </summary>
    /// <param name="section">Report section</param>
    /// <param name="availableSize">Available size</param>
    /// <returns>Layout result</returns>
    LayoutResult CalculateLayout(Section section, Size availableSize);

    /// <summary>
    /// Calculates position for an element
    /// </summary>
    /// <param name="element">Report element</param>
    /// <param name="context">Layout context</param>
    /// <returns>Calculated position</returns>
    Position CalculatePosition(ReportElement element, LayoutContext context);

    /// <summary>
    /// Checks if element fits in remaining space
    /// </summary>
    /// <param name="element">Report element</param>
    /// <param name="availableSpace">Available space</param>
    /// <returns>True if element fits</returns>
    bool FitsInSpace(ReportElement element, Size availableSpace);

    /// <summary>
    /// Aligns elements within container
    /// </summary>
    /// <param name="elements">Elements to align</param>
    /// <param name="containerSize">Container size</param>
    /// <param name="alignment">Alignment type</param>
    void AlignElements(IEnumerable<ReportElement> elements, Size containerSize, Alignment alignment);
}

/// <summary>
/// Layout calculation result
/// </summary>
public class LayoutResult
{
    public Size CalculatedSize { get; set; }
    public Dictionary<Guid, Position> ElementPositions { get; set; } = new();
    public bool RequiresPageBreak { get; set; }
}

/// <summary>
/// Layout context
/// </summary>
public class LayoutContext
{
    public Size AvailableSize { get; set; }
    public Position CurrentPosition { get; set; }
    public int CurrentPage { get; set; }
    public Dictionary<string, object> Variables { get; set; } = new();
}
```

---

## 3. Application Layer Interfaces

### 3.1 Service Interfaces

#### **ITemplateService**

High-level template management.

```csharp
namespace ReportGenerator.Application.Services;

/// <summary>
/// Application service for template management
/// </summary>
public interface ITemplateService
{
    /// <summary>
    /// Creates a new template
    /// </summary>
    /// <param name="request">Create request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created template ID</returns>
    Task<Guid> CreateTemplateAsync(CreateTemplateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing template
    /// </summary>
    /// <param name="request">Update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateTemplateAsync(UpdateTemplateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a template
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteTemplateAsync(Guid templateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets template by ID
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Template</returns>
    /// <exception cref="TemplateNotFoundException">If template not found</exception>
    Task<TemplateDto> GetTemplateAsync(Guid templateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all templates
    /// </summary>
    /// <param name="filter">Filter criteria</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of templates</returns>
    Task<IEnumerable<TemplateListItemDto>> GetAllTemplatesAsync(TemplateFilter? filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches templates
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Matching templates</returns>
    Task<IEnumerable<TemplateListItemDto>> SearchTemplatesAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Duplicates a template
    /// </summary>
    /// <param name="templateId">Template ID to duplicate</param>
    /// <param name="newName">New template name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New template ID</returns>
    Task<Guid> DuplicateTemplateAsync(Guid templateId, string newName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a template
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result</returns>
    Task<ValidationResult> ValidateTemplateAsync(Guid templateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports template to file
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <param name="filePath">Export file path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ExportTemplateAsync(Guid templateId, string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Imports template from file
    /// </summary>
    /// <param name="filePath">Import file path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Imported template ID</returns>
    Task<Guid> ImportTemplateAsync(string filePath, CancellationToken cancellationToken = default);
}

/// <summary>
/// Template data transfer object
/// </summary>
public class TemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public int Version { get; set; }
    public PageSetupDto PageSetup { get; set; } = new();
    public List<SectionDto> Sections { get; set; } = new();
    public List<DataSourceDto> DataSources { get; set; } = new();
    public List<ExpressionDto> Expressions { get; set; } = new();
    public List<GroupDto> Groups { get; set; } = new();
}

/// <summary>
/// Template list item (lightweight)
/// </summary>
public class TemplateListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ModifiedDate { get; set; }
    public int SectionCount { get; set; }
    public int ElementCount { get; set; }
    public string[] Tags { get; set; } = Array.Empty<string>();
}

/// <summary>
/// Create template request
/// </summary>
public class CreateTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public PageSetupDto PageSetup { get; set; } = new();
}

/// <summary>
/// Update template request
/// </summary>
public class UpdateTemplateRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public PageSetupDto PageSetup { get; set; } = new();
    public List<SectionDto> Sections { get; set; } = new();
}

/// <summary>
/// Template filter
/// </summary>
public class TemplateFilter
{
    public bool? IsActive { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? ModifiedAfter { get; set; }
    public string[]? Tags { get; set; }
}
```

---

#### **IReportService**

Report generation and execution.

```csharp
namespace ReportGenerator.Application.Services;

/// <summary>
/// Application service for report generation
/// </summary>
public interface IReportService
{
    /// <summary>
    /// Generates a report
    /// </summary>
    /// <param name="request">Generation request</param>
    /// <param name="progress">Progress reporting</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Generated report</returns>
    Task<ReportResult> GenerateReportAsync(
        GenerateReportRequest request,
        IProgress<ReportProgress>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates report preview (limited pages)
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <param name="parameters">Report parameters</param>
    /// <param name="maxPages">Maximum pages to generate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Preview result</returns>
    Task<ReportPreviewResult> PreviewReportAsync(
        Guid templateId,
        Dictionary<string, object>? parameters = null,
        int maxPages = 3,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports report to specified format
    /// </summary>
    /// <param name="reportResult">Report to export</param>
    /// <param name="format">Export format</param>
    /// <param name="filePath">Output file path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ExportReportAsync(
        ReportResult reportResult,
        ExportFormat format,
        string filePath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Prints report to specified printer
    /// </summary>
    /// <param name="reportResult">Report to print</param>
    /// <param name="settings">Print settings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task PrintReportAsync(
        ReportResult reportResult,
        PrintSettings settings,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates report statistics without full generation
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <param name="parameters">Report parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Report statistics</returns>
    Task<ReportStatistics> GetReportStatisticsAsync(
        Guid templateId,
        Dictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Generate report request
/// </summary>
public class GenerateReportRequest
{
    public Guid TemplateId { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
    public int? StartPage { get; set; }
    public int? EndPage { get; set; }
    public bool CalculateAggregates { get; set; } = true;
}

/// <summary>
/// Report generation result
/// </summary>
public class ReportResult
{
    public Guid ReportId { get; set; }
    public Guid TemplateId { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public DateTime GeneratedDate { get; set; }
    public TimeSpan GenerationTime { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public List<PageResult> Pages { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Report preview result
/// </summary>
public class ReportPreviewResult
{
    public List<PageResult> Pages { get; set; } = new();
    public int EstimatedTotalPages { get; set; }
    public bool IsPartial { get; set; }
}

/// <summary>
/// Report progress information
/// </summary>
public class ReportProgress
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int ProcessedRecords { get; set; }
    public string CurrentSection { get; set; } = string.Empty;
    public double PercentComplete => TotalPages > 0 ? (double)CurrentPage / TotalPages * 100 : 0;
}

/// <summary>
/// Report statistics
/// </summary>
public class ReportStatistics
{
    public int EstimatedPages { get; set; }
    public int TotalRecords { get; set; }
    public TimeSpan EstimatedGenerationTime { get; set; }
    public Dictionary<string, object> Aggregates { get; set; } = new();
}

/// <summary>
/// Export format enumeration
/// </summary>
public enum ExportFormat
{
    PNG,
    JPEG,
    PDF,
    TIFF,
    BMP
}

/// <summary>
/// Print settings
/// </summary>
public class PrintSettings
{
    public string? PrinterName { get; set; }
    public int Copies { get; set; } = 1;
    public bool Collate { get; set; } = true;
    public int? FromPage { get; set; }
    public int? ToPage { get; set; }
    public bool PrintToFile { get; set; }
    public string? OutputFilePath { get; set; }
}
```

---

#### **IDataSourceService**

Data source management and querying.

```csharp
namespace ReportGenerator.Application.Services;

/// <summary>
/// Application service for data source operations
/// </summary>
public interface IDataSourceService
{
    /// <summary>
    /// Tests database connection
    /// </summary>
    /// <param name="connectionStringId">Connection string ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Test result</returns>
    Task<ConnectionTestResult> TestConnectionAsync(Guid connectionStringId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets list of tables from database
    /// </summary>
    /// <param name="connectionStringId">Connection string ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Table names</returns>
    Task<IEnumerable<string>> GetTablesAsync(Guid connectionStringId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets columns for a table
    /// </summary>
    /// <param name="connectionStringId">Connection string ID</param>
    /// <param name="tableName">Table name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Column information</returns>
    Task<IEnumerable<ColumnInfo>> GetColumnsAsync(Guid connectionStringId, string tableName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes query and returns preview data
    /// </summary>
    /// <param name="dataSourceId">Data source ID</param>
    /// <param name="maxRows">Maximum rows to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Preview data</returns>
    Task<DataPreviewResult> PreviewDataAsync(Guid dataSourceId, int maxRows = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes query with parameters
    /// </summary>
    /// <param name="dataSourceId">Data source ID</param>
    /// <param name="parameters">Query parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Query result</returns>
    Task<DataQueryResult> ExecuteQueryAsync(
        Guid dataSourceId,
        Dictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets stored procedures from database
    /// </summary>
    /// <param name="connectionStringId">Connection string ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Stored procedure names</returns>
    Task<IEnumerable<string>> GetStoredProceduresAsync(Guid connectionStringId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Connection test result
/// </summary>
public class ConnectionTestResult
{
    public bool IsSuccessful { get; set; }
    public string? ErrorMessage { get; set; }
    public TimeSpan ResponseTime { get; set; }
    public string ServerVersion { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}

/// <summary>
/// Column information
/// </summary>
public class ColumnInfo
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public int? MaxLength { get; set; }
    public bool IsPrimaryKey { get; set; }
}

/// <summary>
/// Data preview result
/// </summary>
public class DataPreviewResult
{
    public List<ColumnInfo> Columns { get; set; } = new();
    public List<Dictionary<string, object?>> Rows { get; set; } = new();
    public int TotalRows { get; set; }
    public bool IsLimited { get; set; }
}

/// <summary>
/// Data query result
/// </summary>
public class DataQueryResult
{
    public List<ColumnInfo> Columns { get; set; } = new();
    public List<Dictionary<string, object?>> Rows { get; set; } = new();
    public int TotalRows { get; set; }
    public TimeSpan ExecutionTime { get; set; }
}
```

---

## 4. Infrastructure Layer Interfaces

### 4.1 External Service Interfaces

#### **IPrintService**

Printing service.

```csharp
namespace ReportGenerator.Infrastructure.Services;

/// <summary>
/// Service for printing reports
/// </summary>
public interface IPrintService
{
    /// <summary>
    /// Gets available printers
    /// </summary>
    /// <returns>Printer names</returns>
    IEnumerable<string> GetAvailablePrinters();

    /// <summary>
    /// Gets default printer name
    /// </summary>
    /// <returns>Default printer name</returns>
    string GetDefaultPrinter();

    /// <summary>
    /// Prints pages to specified printer
    /// </summary>
    /// <param name="pages">Pages to print</param>
    /// <param name="settings">Print settings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task PrintAsync(IEnumerable<PageResult> pages, PrintSettings settings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Shows print dialog
    /// </summary>
    /// <param name="settings">Initial settings</param>
    /// <returns>Updated settings if user clicked OK, null if cancelled</returns>
    PrintSettings? ShowPrintDialog(PrintSettings settings);
}
```

---

#### **IImageExportService**

Image export service.

```csharp
namespace ReportGenerator.Infrastructure.Services;

/// <summary>
/// Service for exporting reports as images
/// </summary>
public interface IImageExportService
{
    /// <summary>
    /// Exports page to PNG
    /// </summary>
    /// <param name="page">Page to export</param>
    /// <param name="filePath">Output file path</param>
    /// <param name="dpi">Resolution in DPI</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ExportToPngAsync(PageResult page, string filePath, int dpi = 300, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports page to JPEG
    /// </summary>
    /// <param name="page">Page to export</param>
    /// <param name="filePath">Output file path</param>
    /// <param name="quality">JPEG quality (1-100)</param>
    /// <param name="dpi">Resolution in DPI</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ExportToJpegAsync(PageResult page, string filePath, int quality = 90, int dpi = 300, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports multiple pages to single TIFF
    /// </summary>
    /// <param name="pages">Pages to export</param>
    /// <param name="filePath">Output file path</param>
    /// <param name="dpi">Resolution in DPI</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ExportToTiffAsync(IEnumerable<PageResult> pages, string filePath, int dpi = 300, CancellationToken cancellationToken = default);
}
```

---

## 5. Validation Interfaces

```csharp
namespace ReportGenerator.Domain.Validation;

/// <summary>
/// Validation result
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<ValidationError> Errors { get; set; } = new();

    public static ValidationResult Success() => new() { IsValid = true };
    public static ValidationResult Failure(params ValidationError[] errors) =>
        new() { IsValid = false, Errors = errors.ToList() };
}

/// <summary>
/// Validation error
/// </summary>
public class ValidationError
{
    public string PropertyName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public object? AttemptedValue { get; set; }
}
```

---

## 6. Event Interfaces

```csharp
namespace ReportGenerator.Domain.Events;

/// <summary>
/// Domain event interface
/// </summary>
public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
}

/// <summary>
/// Domain event handler
/// </summary>
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}

// Example events
public class TemplateCreatedEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid TemplateId { get; set; }
    public string TemplateName { get; set; } = string.Empty;
}

public class ReportGeneratedEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid ReportId { get; set; }
    public Guid TemplateId { get; set; }
    public int PageCount { get; set; }
    public TimeSpan GenerationTime { get; set; }
}
```

---

This comprehensive interface specification ensures loose coupling, testability, and adherence to SOLID principles throughout the application.
