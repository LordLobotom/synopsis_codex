# Report Generator - Technical Architecture Document

**Version:** 1.0.0
**Date:** October 1, 2025
**Status:** Design Phase

---

## 1. Architecture Overview

### 1.1 Architecture Style

The system follows **Clean Architecture** principles combined with **MVVM** pattern for WPF presentation:

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer (WPF)                  │
│                        MVVM Pattern                          │
│  ┌────────────┐  ┌─────────────┐  ┌───────────────────┐   │
│  │   Views    │◄─│  ViewModels │◄─│  View Services    │   │
│  │  (XAML)    │  │  (C#/F#)    │  │  (Converters,     │   │
│  │            │  │             │  │   Behaviors)      │   │
│  └────────────┘  └─────────────┘  └───────────────────┘   │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│                    Application Layer                         │
│              (Use Cases / Business Logic)                    │
│  ┌────────────────────┐  ┌────────────────────────────┐    │
│  │  Report Commands   │  │  Query Handlers            │    │
│  │  - CreateReport    │  │  - GetTemplates            │    │
│  │  - UpdateTemplate  │  │  - GetReportData           │    │
│  │  - GenerateReport  │  │  - SearchTemplates         │    │
│  └────────────────────┘  └────────────────────────────┘    │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│                      Domain Layer                            │
│                  (Core Business Logic)                       │
│  ┌─────────────┐  ┌─────────────┐  ┌──────────────────┐   │
│  │  Entities   │  │  Value      │  │  Domain          │   │
│  │  - Report   │  │  Objects    │  │  Services        │   │
│  │  - Template │  │  - Money    │  │  - Expression    │   │
│  │  - Element  │  │  - Position │  │    Evaluator     │   │
│  │  - Section  │  │  - Size     │  │  - Barcode       │   │
│  └─────────────┘  └─────────────┘  │    Generator     │   │
│                                     └──────────────────┘   │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│                  Infrastructure Layer                        │
│              (External Concerns & I/O)                       │
│  ┌────────────┐  ┌─────────────┐  ┌────────────────────┐  │
│  │ Data       │  │ File System │  │ External Services  │  │
│  │ Access     │  │ - Template  │  │ - Printing         │  │
│  │ - EF Core  │  │   Storage   │  │ - Image Export     │  │
│  │ - Dapper   │  │ - Export    │  │ - Barcode (ZXing)  │  │
│  └────────────┘  └─────────────┘  └────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

### 1.2 Design Principles

- **Separation of Concerns:** Each layer has distinct responsibility
- **Dependency Inversion:** Dependencies point inward (toward domain)
- **Single Responsibility:** Each class has one reason to change
- **Open/Closed:** Open for extension, closed for modification
- **Interface Segregation:** Small, focused interfaces
- **Dependency Injection:** Constructor injection throughout

---

## 2. System Architecture

### 2.1 High-Level Component Diagram

```
┌───────────────────────────────────────────────────────────────┐
│                      Report Generator Application             │
│                                                               │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │              WPF Application Host                        │ │
│  │  ┌───────────────────┐    ┌──────────────────────────┐ │ │
│  │  │  Main Window      │    │  Dependency Injection    │ │ │
│  │  │  - Menu           │    │  Container               │ │ │
│  │  │  - Toolbar        │◄───┤  (Microsoft.Extensions   │ │ │
│  │  │  - Status Bar     │    │   .DependencyInjection)  │ │ │
│  │  └───────────────────┘    └──────────────────────────┘ │ │
│  └─────────────────────────────────────────────────────────┘ │
│                                                               │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │                     Presentation Layer                   │ │
│  │                                                          │ │
│  │  ┌──────────────────┐  ┌──────────────────────────┐   │ │
│  │  │ Designer         │  │ Preview & Output         │   │ │
│  │  │                  │  │                          │   │ │
│  │  │ ┌──────────────┐ │  │ ┌──────────────────────┐ │   │ │
│  │  │ │DesignerView  │ │  │ │PreviewView           │ │   │ │
│  │  │ │DesignerVM    │ │  │ │PreviewVM             │ │   │ │
│  │  │ └──────────────┘ │  │ └──────────────────────┘ │   │ │
│  │  │                  │  │                          │   │ │
│  │  │ ┌──────────────┐ │  │ ┌──────────────────────┐ │   │ │
│  │  │ │ElementEditor │ │  │ │PrintDialog           │ │   │ │
│  │  │ │ElementVM     │ │  │ │PrintVM               │ │   │ │
│  │  │ └──────────────┘ │  │ └──────────────────────┘ │   │ │
│  │  │                  │  │                          │   │ │
│  │  │ ┌──────────────┐ │  │ ┌──────────────────────┐ │   │ │
│  │  │ │Toolbox       │ │  │ │ExportDialog          │ │   │ │
│  │  │ │ToolboxVM     │ │  │ │ExportVM              │ │   │ │
│  │  │ └──────────────┘ │  │ └──────────────────────┘ │   │ │
│  │  └──────────────────┘  └──────────────────────────┘   │ │
│  │                                                          │ │
│  │  ┌──────────────────┐  ┌──────────────────────────┐   │ │
│  │  │ Data Source      │  │ Template Management      │   │ │
│  │  │                  │  │                          │   │ │
│  │  │ ┌──────────────┐ │  │ ┌──────────────────────┐ │   │ │
│  │  │ │DataSourceView│ │  │ │TemplateListView      │ │   │ │
│  │  │ │DataSourceVM  │ │  │ │TemplateListVM        │ │   │ │
│  │  │ └──────────────┘ │  │ └──────────────────────┘ │   │ │
│  │  │                  │  │                          │   │ │
│  │  │ ┌──────────────┐ │  │ ┌──────────────────────┐ │   │ │
│  │  │ │QueryBuilder  │ │  │ │TemplateProperties    │ │   │ │
│  │  │ │QueryVM       │ │  │ │TemplatePropsVM       │ │   │ │
│  │  │ └──────────────┘ │  │ └──────────────────────┘ │   │ │
│  │  └──────────────────┘  └──────────────────────────┘   │ │
│  └─────────────────────────────────────────────────────────┘ │
│                               ▲                               │
│                               │                               │
│  ┌────────────────────────────┴────────────────────────────┐ │
│  │                   Application Services                    │ │
│  │                                                          │ │
│  │  ┌──────────────────┐  ┌──────────────────────────┐   │ │
│  │  │ Command Handlers │  │ Query Handlers           │   │ │
│  │  │                  │  │                          │   │ │
│  │  │ - CreateTemplate │  │ - GetTemplateById        │   │ │
│  │  │ - UpdateTemplate │  │ - GetAllTemplates        │   │ │
│  │  │ - DeleteTemplate │  │ - SearchTemplates        │   │ │
│  │  │ - GenerateReport │  │ - GetReportData          │   │ │
│  │  │ - ExportReport   │  │ - PreviewReport          │   │ │
│  │  └──────────────────┘  └──────────────────────────┘   │ │
│  │                                                          │ │
│  │  ┌──────────────────────────────────────────────────┐  │ │
│  │  │            Application Interfaces                 │  │ │
│  │  │  ITemplateService, IReportService,               │  │ │
│  │  │  IDataSourceService, IExpressionService          │  │ │
│  │  └──────────────────────────────────────────────────┘  │ │
│  └─────────────────────────────────────────────────────────┘ │
│                               ▲                               │
│                               │                               │
│  ┌────────────────────────────┴────────────────────────────┐ │
│  │                      Domain Layer                         │ │
│  │                                                          │ │
│  │  ┌──────────────────┐  ┌──────────────────────────┐   │ │
│  │  │ Entities         │  │ Value Objects            │   │ │
│  │  │                  │  │                          │   │ │
│  │  │ - Template       │  │ - Position (X, Y)        │   │ │
│  │  │ - Section        │  │ - Size (Width, Height)   │   │ │
│  │  │ - ReportElement  │  │ - Color                  │   │ │
│  │  │ - DataSource     │  │ - Font                   │   │ │
│  │  │ - Expression     │  │ - Alignment              │   │ │
│  │  │ - Group          │  │ - Margin                 │   │ │
│  │  └──────────────────┘  └──────────────────────────┘   │ │
│  │                                                          │ │
│  │  ┌──────────────────────────────────────────────────┐  │ │
│  │  │            Domain Services                        │  │ │
│  │  │                                                   │  │ │
│  │  │  ┌────────────────────┐  ┌──────────────────┐   │  │ │
│  │  │  │ ExpressionEvaluator│  │ BarcodeGenerator │   │  │ │
│  │  │  │ - Parse()          │  │ - Generate()     │   │  │ │
│  │  │  │ - Evaluate()       │  │ - Validate()     │   │  │ │
│  │  │  └────────────────────┘  └──────────────────┘   │  │ │
│  │  │                                                   │  │ │
│  │  │  ┌────────────────────┐  ┌──────────────────┐   │  │ │
│  │  │  │ ReportRenderer     │  │ LayoutEngine     │   │  │ │
│  │  │  │ - RenderPage()     │  │ - Calculate()    │   │  │ │
│  │  │  │ - ApplyStyle()     │  │ - Position()     │   │  │ │
│  │  │  └────────────────────┘  └──────────────────┘   │  │ │
│  │  └──────────────────────────────────────────────────┘  │ │
│  │                                                          │ │
│  │  ┌──────────────────────────────────────────────────┐  │ │
│  │  │            Repository Interfaces                  │  │ │
│  │  │  ITemplateRepository, IReportRepository,         │  │ │
│  │  │  IDataSourceRepository                           │  │ │
│  │  └──────────────────────────────────────────────────┘  │ │
│  └─────────────────────────────────────────────────────────┘ │
│                               ▲                               │
│                               │                               │
│  ┌────────────────────────────┴────────────────────────────┐ │
│  │                  Infrastructure Layer                     │ │
│  │                                                          │ │
│  │  ┌──────────────────┐  ┌──────────────────────────┐   │ │
│  │  │ Data Access      │  │ External Services        │   │ │
│  │  │                  │  │                          │   │ │
│  │  │ ┌──────────────┐ │  │ ┌──────────────────────┐ │   │ │
│  │  │ │ EF Core      │ │  │ │ PrintService         │ │   │ │
│  │  │ │ - AppDbCtx   │ │  │ │ (System.Printing)    │ │   │ │
│  │  │ │ - Migrations │ │  │ └──────────────────────┘ │   │ │
│  │  │ └──────────────┘ │  │                          │   │ │
│  │  │                  │  │ ┌──────────────────────┐ │   │ │
│  │  │ ┌──────────────┐ │  │ │ ImageExportService   │ │   │ │
│  │  │ │ Dapper       │ │  │ │ (System.Drawing)     │ │   │ │
│  │  │ │ - Queries    │ │  │ └──────────────────────┘ │   │ │
│  │  │ └──────────────┘ │  │                          │   │ │
│  │  │                  │  │ ┌──────────────────────┐ │   │ │
│  │  │ ┌──────────────┐ │  │ │ BarcodeService       │ │   │ │
│  │  │ │ Repositories │ │  │ │ (ZXing.Net)          │ │   │ │
│  │  │ │ - Template   │ │  │ └──────────────────────┘ │   │ │
│  │  │ │ - Report     │ │  │                          │   │ │
│  │  │ └──────────────┘ │  │ ┌──────────────────────┐ │   │ │
│  │  └──────────────────┘  │ │ ExpressionService    │ │   │ │
│  │                         │ │ (NCalc)              │ │   │ │
│  │                         │ └──────────────────────┘ │   │ │
│  │                         └──────────────────────────┘   │ │
│  └─────────────────────────────────────────────────────────┘ │
└───────────────────────────────────────────────────────────────┘
```

---

## 3. Layer Details

### 3.1 Presentation Layer

**Technology:** WPF with MVVM pattern

#### 3.1.1 Views (XAML)
- Pure UI markup
- Data binding to ViewModels
- No business logic
- Material Design styling

#### 3.1.2 ViewModels (C#)
- INotifyPropertyChanged implementation
- Commands (ICommand/RelayCommand)
- Data transformation for display
- Input validation
- Navigation logic

#### 3.1.3 View Services
- Value Converters (e.g., BoolToVisibilityConverter)
- Attached Behaviors (e.g., DragDropBehavior)
- Custom Controls (e.g., ReportCanvas)
- Markup Extensions

**Key ViewModels:**

```csharp
// Designer
DesignerViewModel
ElementEditorViewModel
ToolboxViewModel
PropertiesViewModel

// Data
DataSourceViewModel
QueryBuilderViewModel
FieldMappingViewModel

// Template
TemplateListViewModel
TemplatePropertiesViewModel

// Output
PreviewViewModel
PrintViewModel
ExportViewModel
```

### 3.2 Application Layer

**Technology:** C# application services

#### 3.2.1 Command Handlers (CQRS Pattern)

```csharp
// Commands (write operations)
CreateTemplateCommand / CreateTemplateCommandHandler
UpdateTemplateCommand / UpdateTemplateCommandHandler
DeleteTemplateCommand / DeleteTemplateCommandHandler
GenerateReportCommand / GenerateReportCommandHandler
ExportReportCommand / ExportReportCommandHandler

// Queries (read operations)
GetTemplateByIdQuery / GetTemplateByIdQueryHandler
GetAllTemplatesQuery / GetAllTemplatesQueryHandler
SearchTemplatesQuery / SearchTemplatesQueryHandler
GetReportDataQuery / GetReportDataQueryHandler
PreviewReportQuery / PreviewReportQueryHandler
```

#### 3.2.2 Application Services

```csharp
public interface ITemplateService
{
    Task<Guid> CreateTemplateAsync(Template template);
    Task UpdateTemplateAsync(Template template);
    Task DeleteTemplateAsync(Guid templateId);
    Task<Template> GetTemplateByIdAsync(Guid templateId);
    Task<IEnumerable<Template>> GetAllTemplatesAsync();
    Task<IEnumerable<Template>> SearchTemplatesAsync(string searchTerm);
}

public interface IReportService
{
    Task<ReportResult> GenerateReportAsync(Guid templateId, ReportParameters parameters);
    Task<byte[]> ExportReportAsync(ReportResult report, ExportFormat format);
    Task<ReportPreview> PreviewReportAsync(Guid templateId, ReportParameters parameters);
}

public interface IDataSourceService
{
    Task<DataSourceConnection> TestConnectionAsync(string connectionString);
    Task<IEnumerable<string>> GetTablesAsync(Guid dataSourceId);
    Task<IEnumerable<Column>> GetColumnsAsync(Guid dataSourceId, string tableName);
    Task<DataTable> ExecuteQueryAsync(Guid dataSourceId, string query, Dictionary<string, object> parameters);
}

public interface IExpressionService
{
    ExpressionResult Evaluate(string expression, Dictionary<string, object> variables);
    IEnumerable<string> GetAvailableFunctions();
    FunctionInfo GetFunctionInfo(string functionName);
}
```

### 3.3 Domain Layer

**Technology:** C# entities and domain services

#### 3.3.1 Entities

```csharp
public class Template : Entity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public TemplateMetadata Metadata { get; private set; }
    public PageSetup PageSetup { get; private set; }
    public List<Section> Sections { get; private set; }
    public List<DataSource> DataSources { get; private set; }
    public List<Expression> Expressions { get; private set; }

    // Business methods
    public void AddSection(Section section);
    public void RemoveSection(Guid sectionId);
    public void UpdateMetadata(TemplateMetadata metadata);
    public ValidationResult Validate();
}

public class Section : Entity
{
    public Guid Id { get; private set; }
    public SectionType Type { get; private set; }
    public string Name { get; private set; }
    public Size Height { get; private set; }
    public List<ReportElement> Elements { get; private set; }
    public bool IsVisible { get; private set; }

    public void AddElement(ReportElement element);
    public void RemoveElement(Guid elementId);
    public void Reorder(Guid elementId, int newIndex);
}

public abstract class ReportElement : Entity
{
    public Guid Id { get; private set; }
    public Position Position { get; private set; }
    public Size Size { get; private set; }
    public ElementStyle Style { get; private set; }
    public string Name { get; private set; }
    public bool IsVisible { get; private set; }

    public abstract ElementType GetElementType();
    public abstract ValidationResult Validate();
}

// Concrete element types
public class LabelElement : ReportElement
public class DatabaseFieldElement : ReportElement
public class ExpressionElement : ReportElement
public class LineElement : ReportElement
public class RectangleElement : ReportElement
public class ImageElement : ReportElement
public class BarcodeElement : ReportElement
public class SubReportElement : ReportElement
```

#### 3.3.2 Value Objects

```csharp
public record Position(double X, double Y);
public record Size(double Width, double Height);
public record Color(byte R, byte G, byte B, byte A);
public record Font(string Family, double Size, FontWeight Weight, FontStyle Style);
public record Margin(double Left, double Top, double Right, double Bottom);
```

#### 3.3.3 Domain Services

```csharp
public interface IExpressionEvaluator
{
    object Evaluate(string expression, IDictionary<string, object> context);
    ExpressionResult Parse(string expression);
    bool Validate(string expression);
}

public interface IBarcodeGenerator
{
    byte[] Generate(string content, BarcodeType type, BarcodeOptions options);
    bool Validate(string content, BarcodeType type);
}

public interface IReportRenderer
{
    RenderResult RenderPage(Template template, DataContext dataContext, int pageNumber);
    IEnumerable<RenderResult> RenderAllPages(Template template, DataContext dataContext);
}

public interface ILayoutEngine
{
    LayoutResult CalculateLayout(Section section, Size availableSize);
    Position CalculatePosition(ReportElement element, LayoutContext context);
}
```

### 3.4 Infrastructure Layer

**Technology:** EF Core, Dapper, external libraries

#### 3.4.1 Data Access

**Entity Framework Core (Complex Queries, Writes):**

```csharp
public class AppDbContext : DbContext
{
    public DbSet<TemplateEntity> Templates { get; set; }
    public DbSet<SectionEntity> Sections { get; set; }
    public DbSet<ElementEntity> Elements { get; set; }
    public DbSet<DataSourceEntity> DataSources { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Entity configurations
        modelBuilder.ApplyConfiguration(new TemplateConfiguration());
        modelBuilder.ApplyConfiguration(new SectionConfiguration());
        // ...
    }
}

public class TemplateRepository : ITemplateRepository
{
    private readonly AppDbContext _context;

    public async Task<Template> GetByIdAsync(Guid id)
    {
        var entity = await _context.Templates
            .Include(t => t.Sections)
                .ThenInclude(s => s.Elements)
            .Include(t => t.DataSources)
            .FirstOrDefaultAsync(t => t.Id == id);

        return entity?.ToDomainModel();
    }

    public async Task SaveAsync(Template template)
    {
        var entity = TemplateEntity.FromDomainModel(template);
        _context.Templates.Update(entity);
        await _context.SaveChangesAsync();
    }
}
```

**Dapper (High-Performance Reads):**

```csharp
public class ReportDataRepository : IReportDataRepository
{
    private readonly IDbConnection _connection;

    public async Task<IEnumerable<dynamic>> ExecuteQueryAsync(string query, object parameters)
    {
        return await _connection.QueryAsync(query, parameters);
    }

    public async Task<DataTable> GetReportDataAsync(string query)
    {
        using var reader = await _connection.ExecuteReaderAsync(query);
        var table = new DataTable();
        table.Load(reader);
        return table;
    }
}
```

#### 3.4.2 External Services

**Print Service:**

```csharp
public class PrintService : IPrintService
{
    public async Task PrintAsync(RenderResult renderResult, PrintSettings settings)
    {
        var printDialog = new PrintDialog();
        if (printDialog.ShowDialog() == true)
        {
            var document = CreateDocument(renderResult);
            printDialog.PrintDocument(document.DocumentPaginator, "Report");
        }
    }
}
```

**Image Export Service:**

```csharp
public class ImageExportService : IImageExportService
{
    public async Task<byte[]> ExportToPngAsync(RenderResult renderResult, int dpi)
    {
        using var bitmap = CreateBitmap(renderResult, dpi);
        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        return stream.ToArray();
    }
}
```

**Barcode Service (ZXing.Net):**

```csharp
public class BarcodeService : IBarcodeService
{
    private readonly BarcodeWriter _writer = new();

    public byte[] GenerateBarcode(string content, BarcodeType type, int width, int height)
    {
        _writer.Format = MapToBarcodeFormat(type);
        _writer.Options = new EncodingOptions
        {
            Width = width,
            Height = height,
            Margin = 0
        };

        using var bitmap = _writer.Write(content);
        return BitmapToByteArray(bitmap);
    }
}
```

**Expression Service (NCalc):**

```csharp
public class ExpressionService : IExpressionService
{
    public object Evaluate(string expression, IDictionary<string, object> parameters)
    {
        var ncalcExpression = new Expression(expression);

        foreach (var param in parameters)
        {
            ncalcExpression.Parameters[param.Key] = param.Value;
        }

        // Register custom functions
        ncalcExpression.EvaluateFunction += EvaluateCustomFunction;

        return ncalcExpression.Evaluate();
    }

    private void EvaluateCustomFunction(string name, FunctionArgs args)
    {
        switch (name.ToUpper())
        {
            case "LEFT":
                args.Result = EvaluateLeft(args.Parameters);
                break;
            case "RIGHT":
                args.Result = EvaluateRight(args.Parameters);
                break;
            // ... 68 functions
        }
    }
}
```

---

## 4. Data Flow Architecture

### 4.1 Report Generation Flow

```
┌──────────────────┐
│ User clicks      │
│ "Generate Report"│
└────────┬─────────┘
         │
         ▼
┌──────────────────────────────┐
│ PreviewViewModel             │
│ - ExecuteGenerateCommand()   │
└────────┬─────────────────────┘
         │
         ▼
┌──────────────────────────────┐
│ GenerateReportCommandHandler │
│ (Application Layer)          │
└────────┬─────────────────────┘
         │
         ▼
┌──────────────────────────────┐
│ IReportService               │
│ - GenerateReportAsync()      │
└────────┬─────────────────────┘
         │
         ├─► Load Template (ITemplateRepository)
         │
         ├─► Load Data (IDataSourceService)
         │
         ├─► Evaluate Expressions (IExpressionEvaluator)
         │
         ├─► Render Report (IReportRenderer)
         │
         └─► Return ReportResult
                  │
                  ▼
         ┌─────────────────────┐
         │ PreviewViewModel    │
         │ - Display result    │
         └─────────────────────┘
```

### 4.2 Template Save Flow

```
┌──────────────────┐
│ User clicks      │
│ "Save Template"  │
└────────┬─────────┘
         │
         ▼
┌──────────────────────────────┐
│ DesignerViewModel            │
│ - ExecuteSaveCommand()       │
└────────┬─────────────────────┘
         │
         ▼
┌──────────────────────────────┐
│ UpdateTemplateCommandHandler │
│ (Application Layer)          │
└────────┬─────────────────────┘
         │
         ▼
┌──────────────────────────────┐
│ ITemplateService             │
│ - UpdateTemplateAsync()      │
└────────┬─────────────────────┘
         │
         ├─► Validate (Domain)
         │
         ├─► Map to Entity (Infrastructure)
         │
         ├─► Save (EF Core)
         │
         └─► Update Cache (if implemented)
                  │
                  ▼
         ┌─────────────────────┐
         │ Show success message│
         └─────────────────────┘
```

---

## 5. Technology Stack Detail

### 5.1 Core Framework
- **.NET 8.0** (LTS, released November 2023)
- **C# 12** (primary language)
- **F# 8** (optional for functional modules like expression parser)

### 5.2 UI Framework
- **WPF** (.NET 8 version)
- **MaterialDesignInXAML** (v5.0+) - Material Design theming
- **MaterialDesignThemes.Wpf** - Controls and icons
- **Extended.Wpf.Toolkit** - Additional controls

### 5.3 MVVM Framework
- **CommunityToolkit.Mvvm** (formerly MVVM Toolkit)
  - ObservableObject
  - RelayCommand
  - ObservableProperty
  - Messenger (for loose coupling)

### 5.4 Data Access
- **Entity Framework Core 8.0** (ORM for SQLite)
- **Dapper 2.1+** (micro-ORM for high-performance queries)
- **Microsoft.Data.Sqlite** (SQLite provider)
- **Microsoft.EntityFrameworkCore.SqlServer** (MSSQL provider)

### 5.5 Expression Engine
- **NCalc** (v3.0+) - Expression evaluator
  - Custom function support
  - Type-safe evaluation
  - Extensive operator support

### 5.6 Barcode/QR Code
- **ZXing.Net** (v0.16+)
  - 22+ barcode formats
  - QR code generation
  - High-quality output

### 5.7 PDF Export (Future)
- **QuestPDF** (v2024+)
  - Fluent API
  - Modern, fast
  - AI-friendly documentation

### 5.8 Dependency Injection
- **Microsoft.Extensions.DependencyInjection**
- **Microsoft.Extensions.Hosting** (for application lifetime)

### 5.9 Logging
- **Serilog** (v3.0+)
- **Serilog.Sinks.File** (rolling file logging)
- **Serilog.Sinks.Debug** (debug output)
- **Serilog.Sinks.Console** (console logging)

### 5.10 Testing
- **xUnit** (v2.6+) - Test framework
- **FluentAssertions** (v6.12+) - Assertion library
- **Moq** (v4.20+) - Mocking framework
- **Bogus** (v35+) - Test data generation
- **Verify** (snapshot testing)

### 5.11 Code Quality
- **StyleCop.Analyzers** - Code style
- **Roslynator** - Code analysis
- **SonarAnalyzer.CSharp** - Code quality

---

## 6. Deployment Architecture

### 6.1 Application Packaging

```
ReportGenerator.App.exe (WPF application)
│
├── ReportGenerator.Domain.dll (domain entities)
├── ReportGenerator.Application.dll (use cases)
├── ReportGenerator.Infrastructure.dll (data access, external services)
├── ReportGenerator.Presentation.dll (views, viewmodels)
│
├── Dependencies/
│   ├── MaterialDesignThemes.Wpf.dll
│   ├── MaterialDesignColors.dll
│   ├── NCalc.dll
│   ├── ZXing.Net.dll
│   ├── EntityFrameworkCore.*.dll
│   ├── Dapper.dll
│   ├── Serilog.*.dll
│   └── ... (other NuGet packages)
│
├── Templates/
│   └── default.db (SQLite database for templates)
│
├── Logs/
│   └── (log files written here)
│
├── appsettings.json (configuration)
└── appsettings.Production.json (production overrides)
```

### 6.2 Configuration Management

**appsettings.json:**

```json
{
  "Application": {
    "Name": "Report Generator",
    "Version": "1.0.0",
    "Theme": "Light"
  },
  "Database": {
    "Templates": {
      "Provider": "SQLite",
      "ConnectionString": "Data Source=Templates/templates.db"
    },
    "Production": {
      "Provider": "MSSQL",
      "ConnectionString": "Server=(local);Database=Production;Integrated Security=true"
    }
  },
  "Logging": {
    "MinimumLevel": "Information",
    "RollingInterval": "Day",
    "RetainedFileCountLimit": 7
  },
  "Features": {
    "EnablePdfExport": false,
    "EnableCloudSync": false,
    "EnablePlugins": false
  }
}
```

---

## 7. Security Architecture

### 7.1 Connection String Encryption

```csharp
public class SecureConfigurationService
{
    public string EncryptConnectionString(string connectionString)
    {
        // Use DPAPI (Data Protection API) for Windows
        var bytes = Encoding.UTF8.GetBytes(connectionString);
        var encrypted = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(encrypted);
    }

    public string DecryptConnectionString(string encryptedConnectionString)
    {
        var encrypted = Convert.FromBase64String(encryptedConnectionString);
        var bytes = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
        return Encoding.UTF8.GetString(bytes);
    }
}
```

### 7.2 SQL Injection Prevention

- **Parameterized queries** exclusively
- **No string concatenation** for SQL
- **Input validation** for all user inputs
- **ORM usage** (EF Core) for complex queries

### 7.3 Expression Evaluation Security

- **Sandboxed execution** (NCalc does not allow code execution)
- **Timeout limits** for expression evaluation
- **No file system or network access** from expressions
- **Whitelist** of allowed functions only

---

## 8. Performance Architecture

### 8.1 Caching Strategy

```csharp
public interface ICacheService
{
    T Get<T>(string key);
    void Set<T>(string key, T value, TimeSpan expiration);
    void Remove(string key);
}

// Cache layers:
// - Template metadata (in-memory, 1 hour)
// - Data source schemas (in-memory, 24 hours)
// - Expression parse trees (in-memory, no expiration)
// - Report previews (disk cache, 1 hour)
```

### 8.2 Async/Await Pattern

All I/O operations use async/await:
- Database queries
- File I/O
- Network calls
- Long-running computations

```csharp
public async Task<ReportResult> GenerateReportAsync(Guid templateId)
{
    var template = await _templateRepo.GetByIdAsync(templateId);
    var data = await _dataService.FetchDataAsync(template.DataSource);
    var result = await _renderer.RenderAsync(template, data);
    return result;
}
```

### 8.3 Lazy Loading

- Templates loaded on demand
- Report data streamed (not loaded entirely into memory)
- Large images loaded only when visible
- Sections rendered incrementally

---

## 9. Extensibility Architecture

### 9.1 Plugin System (Future Phase)

```csharp
public interface IReportPlugin
{
    string Name { get; }
    Version Version { get; }
    void Initialize(IServiceProvider services);
}

public interface ICustomElementPlugin : IReportPlugin
{
    ReportElement CreateElement();
    Control CreateDesignerControl();
    void Render(RenderContext context);
}

public interface IDataSourcePlugin : IReportPlugin
{
    bool CanHandle(string connectionString);
    IDataReader GetData(string query);
}
```

### 9.2 Custom Function Registration

```csharp
public interface ICustomFunctionRegistry
{
    void Register(string name, Delegate function, string description);
    bool Unregister(string name);
    IEnumerable<FunctionInfo> GetAll();
}
```

---

## 10. Testing Architecture

### 10.1 Test Pyramid

```
        ┌───────────┐
        │    E2E    │  (5% - UI automation)
        └─────┬─────┘
       ┌──────┴──────┐
       │ Integration │  (20% - component tests)
       └──────┬──────┘
     ┌────────┴────────┐
     │   Unit Tests    │  (75% - isolated tests)
     └─────────────────┘
```

### 10.2 Test Organization

```
ReportGenerator.Tests/
├── Unit/
│   ├── Domain/
│   │   ├── TemplateTests.cs
│   │   ├── ExpressionEvaluatorTests.cs
│   │   └── LayoutEngineTests.cs
│   ├── Application/
│   │   ├── TemplateServiceTests.cs
│   │   └── ReportServiceTests.cs
│   └── Infrastructure/
│       ├── TemplateRepositoryTests.cs
│       └── ExpressionServiceTests.cs
│
├── Integration/
│   ├── DatabaseTests.cs
│   ├── ReportGenerationTests.cs
│   └── BarcodeGenerationTests.cs
│
└── E2E/
    ├── DesignerWorkflowTests.cs
    └── ReportGenerationWorkflowTests.cs
```

---

This architecture provides a solid foundation for building a maintainable, testable, and extensible report generation system that can evolve with future requirements.
