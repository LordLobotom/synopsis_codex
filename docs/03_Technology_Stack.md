# Report Generator - Technology Stack Specification

**Version:** 1.0.0
**Date:** October 1, 2025
**Status:** Design Phase

---

## 1. Technology Selection Philosophy

All technology choices follow these principles:

1. **AI-Friendly:** Excellent documentation accessible to AI coding assistants
2. **Modern:** Active development with regular updates
3. **Community:** Large user base with extensive Stack Overflow coverage
4. **Type-Safe:** Strong typing to catch errors at compile time
5. **Testable:** Easy to unit test and mock
6. **Performance:** Efficient for target use cases

---

## 2. Core Technologies

### 2.1 .NET Platform

#### **

.NET 8.0 SDK** (LTS - Long Term Support)
- **Version:** 8.0.x
- **Release:** November 2023
- **Support Until:** November 2026
- **Download:** https://dotnet.microsoft.com/download/dotnet/8.0

**Why .NET 8:**
- Latest LTS version with 3-year support
- Performance improvements (25%+ faster than .NET 6)
- Native AOT compilation support
- Improved JSON serialization
- Enhanced LINQ performance
- Better memory management

**Key Features Used:**
- Generic math
- Required members
- Pattern matching enhancements
- File-scoped types
- UTF-8 string literals

---

### 2.2 Programming Languages

#### **C# 12**
- **Primary language** for entire application
- **Features Used:**
  - Primary constructors
  - Collection expressions
  - Ref readonly parameters
  - Default lambda parameters
  - Alias any type
  - Inline arrays

**Example:**
```csharp
// Primary constructor
public class Template(Guid id, string name)
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
}

// Collection expressions
List<string> sections = ["Header", "Body", "Footer"];

// Alias any type
using Point = (int X, int Y);
```

#### **F# 8** (Optional)
- **Use Cases:**
  - Expression parser implementation
  - Mathematical calculations
  - Data transformation pipelines

**Why F#:**
- Functional programming paradigm fits parsing
- Pattern matching for expression trees
- Immutable by default
- Excellent for domain modeling

**Example F# Module:**
```fsharp
module ExpressionParser

type Expression =
    | Number of float
    | Variable of string
    | BinaryOp of Expression * string * Expression
    | FunctionCall of string * Expression list

let rec evaluate (context: Map<string, obj>) = function
    | Number n -> n
    | Variable v -> context.[v] :?> float
    | BinaryOp (left, "+", right) -> evaluate context left + evaluate context right
    | FunctionCall (name, args) -> evaluateFunction name (List.map (evaluate context) args)
```

---

## 3. UI Framework Stack

### 3.1 **Windows Presentation Foundation (WPF)**

- **Version:** .NET 8 WPF
- **NuGet Package:** Built into .NET SDK
- **Documentation:** https://learn.microsoft.com/en-us/dotnet/desktop/wpf/

**Why WPF:**
- Mature, stable framework (since 2006)
- Rich control library
- Powerful data binding
- Hardware-accelerated rendering (DirectX)
- XAML declarative UI
- Extensive third-party ecosystem

**Key WPF Features Used:**
- Data binding (OneWay, TwoWay)
- Commands (ICommand pattern)
- Styles and templates
- Control templates
- Visual tree manipulation
- Routed events
- Dependency properties

---

### 3.2 **MaterialDesignInXAML Toolkit**

- **Version:** 5.0.0+
- **NuGet:** MaterialDesignThemes
- **GitHub:** https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit
- **License:** MIT

**Why Material Design:**
- Modern, professional appearance
- Consistent design language
- Extensive control library
- Dark/light theme support
- Material Design Icons (6,000+ icons)
- Active community

**Controls Used:**
- Card
- Button (raised, outlined, text)
- TextField (with floating labels)
- ComboBox
- DatePicker
- TimePicker
- DialogHost
- Snackbar
- ProgressBar (circular, linear)
- Chip
- ColorPicker

**Installation:**
```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <materialDesign:BundledTheme BaseTheme="Light"
                                        PrimaryColor="Blue"
                                        SecondaryColor="Lime" />
            <ResourceDictionary Source="pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

---

### 3.3 **Extended WPF Toolkit**

- **Version:** 4.6.0+
- **NuGet:** Extended.Wpf.Toolkit
- **License:** MS-PL

**Controls Used:**
- PropertyGrid (for element properties)
- ColorPicker (advanced)
- MaskedTextBox
- Calculator control
- WatermarkTextBox
- RichTextBox (enhanced)

---

### 3.4 **MVVM Framework - CommunityToolkit.Mvvm**

- **Version:** 8.2.0+
- **NuGet:** CommunityToolkit.Mvvm
- **Documentation:** https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/
- **License:** MIT

**Why CommunityToolkit.Mvvm:**
- Official Microsoft toolkit
- Source generators (compile-time code generation)
- Minimal boilerplate
- Performance optimized
- INotifyPropertyChanged automation
- Command generation
- Messenger for loose coupling

**Key Features:**

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class DesignerViewModel : ObservableObject
{
    // Auto-generates property with INotifyPropertyChanged
    [ObservableProperty]
    private string _templateName;

    // Auto-generates RelayCommand
    [RelayCommand]
    private async Task SaveTemplateAsync()
    {
        await _templateService.SaveAsync(Template);
    }

    // Conditional command (CanExecute)
    [RelayCommand(CanExecute = nameof(CanSave))]
    private void Save() { }

    private bool CanSave() => !string.IsNullOrEmpty(TemplateName);
}
```

---

## 4. Data Access Stack

### 4.1 **Entity Framework Core 8.0**

- **Version:** 8.0.0+
- **NuGet Packages:**
  - Microsoft.EntityFrameworkCore
  - Microsoft.EntityFrameworkCore.SqlServer
  - Microsoft.EntityFrameworkCore.Sqlite
  - Microsoft.EntityFrameworkCore.Design
  - Microsoft.EntityFrameworkCore.Tools

**Why EF Core:**
- Official Microsoft ORM
- Code-first and database-first support
- Migrations for schema changes
- LINQ query support
- Change tracking
- Excellent tooling

**Usage:**

```csharp
public class AppDbContext : DbContext
{
    public DbSet<TemplateEntity> Templates { get; set; }
    public DbSet<SectionEntity> Sections { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("Data Source=templates.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TemplateEntity>()
            .HasMany(t => t.Sections)
            .WithOne()
            .HasForeignKey(s => s.TemplateId);
    }
}

// Migrations
// dotnet ef migrations add InitialCreate
// dotnet ef database update
```

**Performance Optimizations:**
- Compiled queries for frequent operations
- AsNoTracking() for read-only queries
- Batch operations for bulk inserts/updates
- Connection pooling

---

### 4.2 **Dapper**

- **Version:** 2.1.0+
- **NuGet:** Dapper
- **GitHub:** https://github.com/DapperLib/Dapper
- **License:** Apache 2.0

**Why Dapper:**
- "King of Micro-ORMs"
- Near-native ADO.NET performance
- Simple, lightweight (single file)
- Perfect for read-heavy queries
- Direct SQL control

**Usage:**

```csharp
public class ReportDataRepository
{
    private readonly IDbConnection _connection;

    public async Task<IEnumerable<ReportData>> GetReportDataAsync(string query, object parameters)
    {
        return await _connection.QueryAsync<ReportData>(query, parameters);
    }

    public async Task<int> ExecuteCommandAsync(string sql, object parameters)
    {
        return await _connection.ExecuteAsync(sql, parameters);
    }
}
```

**When to Use:**
- Complex queries with joins
- Bulk data retrieval for reports
- Stored procedure calls
- Performance-critical read operations

---

### 4.3 **Database Providers**

#### **SQLite** (Template Storage)
- **Version:** 3.x
- **NuGet:** Microsoft.Data.Sqlite
- **Use Case:** Embedded database for report templates

**Advantages:**
- Zero configuration
- Single file database
- No server required
- Cross-platform
- ACID compliant
- Excellent for < 1GB data

**Configuration:**
```csharp
services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=templates.db")
           .EnableSensitiveDataLogging(isDevelopment)
           .LogTo(Console.WriteLine, LogLevel.Information));
```

#### **Microsoft SQL Server** (Production Data)
- **Version:** SQL Server 2016+
- **NuGet:** Microsoft.Data.SqlClient
- **Use Case:** Production data source for reports

**Connection String:**
```csharp
"Server=localhost;Database=ProductionDB;Integrated Security=true;TrustServerCertificate=true"
```

---

## 5. Expression Engine

### 5.1 **NCalc**

- **Version:** 4.3.0
- **NuGet:** NCalc.Core
- **GitHub:** https://github.com/ncalc/ncalc
- **License:** MIT

**Why NCalc:**
- Battle-tested (15+ years)
- No code execution (safe)
- Extensible with custom functions
- Supports all standard operators
- Type-safe evaluation
- Lambda expression support

**Features:**
- Arithmetic: `+, -, *, /, %, ^`
- Comparison: `=, <>, <, >, <=, >=`
- Logical: `and, or, not`
- Bitwise: `&, |, ^, <<, >>`
- Ternary: `condition ? true : false`
- Functions: `Abs(), Min(), Max(), if()`
- Parameters: Dynamic variable substitution

**Usage (3.x API):**

```csharp
public class ExpressionEvaluator
{
    public object Evaluate(string expression, Dictionary<string, object> parameters)
    {
        var ncalc = new Expression(expression);

        // Add parameters
        foreach (var param in parameters)
        {
            ncalc.Parameters[param.Key] = param.Value;
        }

        // Register custom functions
        ncalc.EvaluateFunction += (name, args) =>
        {
            if (name == "LEFT")
            {
                var str = args.Parameters[0].Evaluate().ToString();
                var len = Convert.ToInt32(args.Parameters[1].Evaluate());
                args.Result = str.Substring(0, Math.Min(len, str.Length));
            }
        };

        return ncalc.Evaluate();
    }
}
```

**Custom Function Library:**
```csharp
public static class CustomFunctions
{
    public static void Register(Expression expression)
    {
        expression.EvaluateFunction += (name, args) =>
        {
            switch (name.ToUpper())
            {
                case "LEFT":
                    args.Result = Left(args);
                    break;
                case "RIGHT":
                    args.Result = Right(args);
                    break;
                // ... 68 functions
            }
        };
    }
}
```

Note (NCalc 4.x):
- Starting with 4.x, `NCalc.Core` no longer exposes the `NCalc.Expression` wrapper. The public API provides the parser and AST; evaluation is internal. This project wraps NCalc.Core via `NCalcExpressionEvaluator` (Infrastructure) which parses the AST and evaluates it with a minimal visitor implementation to support arithmetic, logical, comparison, bitwise, and ternary operations with parameter substitution.

Example (current code):
```csharp
var evaluator = new NCalcExpressionEvaluator();
var result = evaluator.Evaluate("1 + 2 * x", new Dictionary<string, object?> { ["x"] = 3 });
// result == 7
```

---

## 6. Barcode/QR Code Generation

### 6.1 **ZXing.Net**

- **Version:** 0.16.9+
- **NuGet:** ZXing.Net
- **GitHub:** https://github.com/micjahn/ZXing.Net
- **License:** Apache 2.0

**Why ZXing.Net:**
- Port of popular Java library ZXing
- 22+ barcode formats
- QR code support with error correction
- High-quality output
- Active maintenance
- Well-documented

**Supported Formats:**
- 1D: Code 39, Code 93, Code 128, Codabar, EAN-8, EAN-13, UPC-A, UPC-E, ITF, MSI
- 2D: QR Code, Data Matrix, Aztec, PDF417

**Usage:**

```csharp
using ZXing;
using ZXing.Windows.Compatibility;

public class BarcodeGenerator
{
    public BitmapImage GenerateBarcode(string content, BarcodeFormat format, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = format,
            Options = new EncodingOptions
            {
                Width = width,
                Height = height,
                Margin = 10,
                PureBarcode = false // Include text
            }
        };

        var bitmap = writer.Write(content);
        return ConvertToBitmapImage(bitmap);
    }

    public BitmapImage GenerateQRCode(string content, int size, ErrorCorrectionLevel errorCorrection = ErrorCorrectionLevel.M)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Width = size,
                Height = size,
                Margin = 0,
                ErrorCorrection = errorCorrection
            }
        };

        return ConvertToBitmapImage(writer.Write(content));
    }
}
```

---

## 7. Logging

### 7.1 **Serilog**

- **Version:** 3.1.0+
- **NuGet Packages:**
  - Serilog
  - Serilog.Sinks.File
  - Serilog.Sinks.Debug
  - Serilog.Sinks.Console
  - Serilog.Extensions.Hosting

**Why Serilog:**
- Structured logging
- Multiple sinks (file, console, database)
- Easy configuration
- High performance
- Excellent .NET integration

**Configuration:**

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.File(
        path: "Logs/app-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.Debug()
    .CreateLogger();
```

**Usage:**

```csharp
_logger.Information("Generating report {TemplateId} for user {UserId}", templateId, userId);
_logger.Warning("Large dataset detected: {RecordCount} records", recordCount);
_logger.Error(ex, "Failed to generate report {TemplateId}", templateId);
```

---

## 8. Dependency Injection

### 8.1 **Microsoft.Extensions.DependencyInjection**

- **Version:** 8.0.0+
- **NuGet:** Microsoft.Extensions.DependencyInjection
- **Built-in:** Part of .NET

**Configuration:**

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(Configuration.GetConnectionString("Templates")));

        // Repositories
        services.AddScoped<ITemplateRepository, TemplateRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();

        // Services
        services.AddTransient<ITemplateService, TemplateService>();
        services.AddTransient<IReportService, ReportService>();
        services.AddTransient<IExpressionService, ExpressionService>();
        services.AddSingleton<IBarcodeService, BarcodeService>();

        // ViewModels
        services.AddTransient<DesignerViewModel>();
        services.AddTransient<PreviewViewModel>();
        services.AddTransient<TemplateListViewModel>();
    }
}
```

---

## 9. Testing Stack

### 9.1 **xUnit**

- **Version:** 2.6.0+
- **NuGet:** xunit, xunit.runner.visualstudio

**Why xUnit:**
- Modern test framework
- Parallel test execution
- No test setup/teardown (uses constructor/dispose)
- Extensible
- Microsoft recommended

```csharp
public class TemplateServiceTests
{
    private readonly ITemplateService _sut;
    private readonly Mock<ITemplateRepository> _mockRepo;

    public TemplateServiceTests()
    {
        _mockRepo = new Mock<ITemplateRepository>();
        _sut = new TemplateService(_mockRepo.Object);
    }

    [Fact]
    public async Task CreateTemplate_ValidInput_ReturnsTemplateId()
    {
        // Arrange
        var template = new Template("Test Report");

        // Act
        var result = await _sut.CreateTemplateAsync(template);

        // Assert
        result.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateTemplate_InvalidName_ThrowsException(string name)
    {
        // Arrange
        var template = new Template(name);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateTemplateAsync(template));
    }
}
```

---

### 9.2 **FluentAssertions**

- **Version:** 6.12.0+
- **NuGet:** FluentAssertions

**Why FluentAssertions:**
- Readable assertions
- Excellent error messages
- Extensive assertion library

```csharp
result.Should().NotBeNull();
result.Should().BeOfType<Template>();
result.Name.Should().Be("Test Report");
result.Sections.Should().HaveCount(3);
result.Sections.Should().Contain(s => s.Type == SectionType.Header);
```

---

### 9.3 **Moq**

- **Version:** 4.20.0+
- **NuGet:** Moq

**Why Moq:**
- Simple mocking syntax
- Powerful verification
- Callback support

```csharp
var mock = new Mock<ITemplateRepository>();
mock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
    .ReturnsAsync(new Template("Mock"));

mock.Verify(repo => repo.SaveAsync(It.IsAny<Template>()), Times.Once);
```

---

### 9.4 **Bogus**

- **Version:** 35.0.0+
- **NuGet:** Bogus

**Why Bogus:**
- Generate realistic test data
- Fluent API
- Repeatable/seeded data

```csharp
var faker = new Faker<Template>()
    .RuleFor(t => t.Name, f => f.Commerce.ProductName())
    .RuleFor(t => t.Description, f => f.Lorem.Sentence())
    .RuleFor(t => t.CreatedDate, f => f.Date.Past());

var templates = faker.Generate(100);
```

---

## 10. PDF Export (Future Phase)

### 10.1 **QuestPDF**

- **Version:** 2024.x+
- **NuGet:** QuestPDF
- **License:** Community (MIT), Commercial (paid)

**Why QuestPDF:**
- Modern fluent API
- Fast performance
- No external dependencies
- AI-friendly documentation
- Rich layout features

```csharp
Document.Create(container =>
{
    container.Page(page =>
    {
        page.Size(PageSizes.A4);
        page.Margin(2, Unit.Centimetre);

        page.Header().Text("Report Title").FontSize(20).Bold();

        page.Content().Column(column =>
        {
            column.Item().Text("Content here");
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(100);
                    columns.RelativeColumn();
                });

                table.Cell().Text("Label");
                table.Cell().Text("Value");
            });
        });

        page.Footer().AlignCenter().Text(x =>
        {
            x.Span("Page ");
            x.CurrentPageNumber();
        });
    });
}).GeneratePdf("report.pdf");
```

---

## 11. Additional Libraries

### 11.1 **Newtonsoft.Json**

- **Version:** 13.0.0+
- **NuGet:** Newtonsoft.Json
- **Use Case:** Template serialization, configuration

```csharp
var json = JsonConvert.SerializeObject(template, Formatting.Indented);
var template = JsonConvert.DeserializeObject<Template>(json);
```

---

### 11.2 **AutoMapper** (Optional)

- **Version:** 13.0.0+
- **NuGet:** AutoMapper
- **Use Case:** Entity to Domain model mapping

```csharp
var config = new MapperConfiguration(cfg =>
{
    cfg.CreateMap<TemplateEntity, Template>();
    cfg.CreateMap<Template, TemplateEntity>();
});

var mapper = config.CreateMapper();
var template = mapper.Map<Template>(entity);
```

---

## 12. Development Tools

### 12.1 **IDEs**

- **Visual Studio 2022** (17.8+)
  - Built-in WPF designer
  - Excellent debugging
  - ReSharper integration

- **JetBrains Rider** (2024.1+)
  - Cross-platform
  - Fast performance
  - Excellent refactoring

- **Visual Studio Code** (with C# Dev Kit)
  - Lightweight
  - Good for quick edits

---

### 12.2 **Code Quality Tools**

- **StyleCop.Analyzers** - Code style enforcement
- **Roslynator** - Code analysis and refactoring
- **SonarAnalyzer.CSharp** - Code quality and security

---

## 13. NuGet Package Reference

Complete list for project file:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <Nullable>enable</Nullable>
    <LangVersion>12</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <!-- UI -->
    <PackageReference Include="MaterialDesignThemes" Version="5.0.0" />
    <PackageReference Include="MaterialDesignColors" Version="3.0.0" />
    <PackageReference Include="Extended.Wpf.Toolkit" Version="4.6.0" />

    <!-- MVVM -->
    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.0" />

    <!-- Data Access -->
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
    <PackageReference Include="Dapper" Version="2.1.0" />

    <!-- Expression Engine -->
    <PackageReference Include="NCalc.Core" Version="4.3.0" />

    <!-- Barcode -->
    <PackageReference Include="ZXing.Net" Version="0.16.9" />

    <!-- Logging -->
    <PackageReference Include="Serilog" Version="3.1.0" />
    <PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
    <PackageReference Include="Serilog.Sinks.Debug" Version="2.0.0" />
    <PackageReference Include="Serilog.Extensions.Hosting" Version="8.0.0" />

    <!-- Dependency Injection -->
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="8.0.0" />

    <!-- Utilities -->
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
  </ItemGroup>

  <ItemGroup Condition="'$(Configuration)' == 'Debug'">
    <!-- Code Quality (Dev only) -->
    <PackageReference Include="StyleCop.Analyzers" Version="1.2.0">
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Roslynator.Analyzers" Version="4.7.0">
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>
</Project>
```

---

## 14. Version Matrix

| Technology | Minimum Version | Recommended | Latest Tested |
|------------|----------------|-------------|---------------|
| .NET | 8.0.0 | 8.0.1 | 8.0.1 |
| C# | 12.0 | 12.0 | 12.0 |
| MaterialDesignThemes | 5.0.0 | 5.0.0 | 5.0.0 |
| EF Core | 8.0.0 | 8.0.0 | 8.0.0 |
| NCalc | 3.10.0 | 3.10.0 | 3.10.1 |
| ZXing.Net | 0.16.8 | 0.16.9 | 0.16.9 |
| Serilog | 3.1.0 | 3.1.1 | 3.1.1 |
| CommunityToolkit.Mvvm | 8.2.0 | 8.2.2 | 8.2.2 |

---

## 15. License Summary

All selected libraries use permissive licenses:

- **MIT:** MaterialDesignThemes, NCalc, CommunityToolkit.Mvvm, Serilog
- **Apache 2.0:** ZXing.Net, Dapper
- **MS-PL:** Extended.Wpf.Toolkit

**No GPL or restrictive licenses** - safe for commercial use.

---

This technology stack provides a modern, maintainable foundation optimized for AI-assisted development with excellent documentation and community support.
