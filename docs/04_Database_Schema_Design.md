# Report Generator - Database Schema Design

**Version:** 1.0.0
**Date:** October 1, 2025
**Status:** Design Phase

---

## 1. Database Overview

### 1.1 Database Strategy

The system uses **two separate databases**:

1. **Template Database (SQLite)**
   - Stores report templates, metadata, and configuration
   - Embedded, no server required
   - File-based: `templates.db`
   - Size: < 100 MB (thousands of templates)

2. **Production Database (MSSQL)**
   - User's business data
   - External, managed by user
   - Read-only access from report generator
   - Connected via connection strings

---

## 2. Template Database (SQLite)

### 2.1 Schema Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                    TEMPLATES DATABASE                        │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌──────────────┐                                          │
│  │  Templates   │                                          │
│  │──────────────│                                          │
│  │ Id (PK)      │◄──────┐                                  │
│  │ Name         │       │                                  │
│  │ Description  │       │                                  │
│  │ CreatedDate  │       │                                  │
│  │ ModifiedDate │       │                                  │
│  │ Version      │       │                                  │
│  │ PageSetup    │       │ (FK)                             │
│  │ IsActive     │       │                                  │
│  └──────────────┘       │                                  │
│         │               │                                  │
│         │ 1:N           │ N:1                              │
│         ▼               │                                  │
│  ┌──────────────┐       │                                  │
│  │  Sections    │       │                                  │
│  │──────────────│       │                                  │
│  │ Id (PK)      │       │                                  │
│  │ TemplateId   │───────┘                                  │
│  │ Type         │                                          │
│  │ Name         │                                          │
│  │ Height       │                                          │
│  │ IsVisible    │                                          │
│  │ OrderIndex   │                                          │
│  └──────────────┘                                          │
│         │                                                  │
│         │ 1:N                                              │
│         ▼                                                  │
│  ┌──────────────────────────┐                             │
│  │  Elements                │                             │
│  │──────────────────────────│                             │
│  │ Id (PK)                  │                             │
│  │ SectionId (FK)           │                             │
│  │ ElementType              │                             │
│  │ Name                     │                             │
│  │ PositionX, PositionY     │                             │
│  │ Width, Height            │                             │
│  │ Content                  │  (JSON)                     │
│  │ Style                    │  (JSON)                     │
│  │ IsVisible                │                             │
│  │ OrderIndex               │                             │
│  └──────────────────────────┘                             │
│                                                             │
│  ┌──────────────────────────┐                             │
│  │  DataSources             │                             │
│  │──────────────────────────│                             │
│  │ Id (PK)                  │◄──────┐                      │
│  │ TemplateId (FK)          │       │                      │
│  │ Name                     │       │                      │
│  │ ConnectionStringId (FK)  │       │ N:1                  │
│  │ Query                    │       │                      │
│  │ QueryType                │       │                      │
│  │ IsActive                 │       │                      │
│  └──────────────────────────┘       │                      │
│         │                           │                      │
│         │ 1:N                       │                      │
│         ▼                           │                      │
│  ┌──────────────────────────┐       │                      │
│  │  DataSourceParameters    │       │                      │
│  │──────────────────────────│       │                      │
│  │ Id (PK)                  │       │                      │
│  │ DataSourceId (FK)        │       │                      │
│  │ Name                     │       │                      │
│  │ DataType                 │       │                      │
│  │ DefaultValue             │       │                      │
│  │ IsRequired               │       │                      │
│  └──────────────────────────┘       │                      │
│                                     │                      │
│  ┌──────────────────────────┐       │                      │
│  │  ConnectionStrings       │───────┘                      │
│  │──────────────────────────│                             │
│  │ Id (PK)                  │                             │
│  │ Name                     │                             │
│  │ ProviderType             │                             │
│  │ EncryptedConnectionString│                             │
│  │ IsActive                 │                             │
│  │ CreatedDate              │                             │
│  └──────────────────────────┘                             │
│                                                             │
│  ┌──────────────────────────┐                             │
│  │  Expressions             │                             │
│  │──────────────────────────│                             │
│  │ Id (PK)                  │                             │
│  │ TemplateId (FK)          │                             │
│  │ Name                     │                             │
│  │ Expression               │                             │
│  │ DataType                 │                             │
│  │ Description              │                             │
│  └──────────────────────────┘                             │
│                                                             │
│  ┌──────────────────────────┐                             │
│  │  Groups                  │                             │
│  │──────────────────────────│                             │
│  │ Id (PK)                  │                             │
│  │ TemplateId (FK)          │                             │
│  │ Name                     │                             │
│  │ GroupField               │                             │
│  │ SortOrder                │                             │
│  │ HeaderSectionId (FK)     │                             │
│  │ FooterSectionId (FK)     │                             │
│  │ Level                    │                             │
│  └──────────────────────────┘                             │
└─────────────────────────────────────────────────────────────┘
```

---

### 2.2 Table Definitions

#### **2.2.1 Templates**

Core table storing report template metadata.

```sql
CREATE TABLE Templates (
    Id                  TEXT PRIMARY KEY,          -- GUID
    Name                TEXT NOT NULL,
    Description         TEXT,
    CreatedDate         TEXT NOT NULL,             -- ISO 8601
    ModifiedDate        TEXT NOT NULL,             -- ISO 8601
    CreatedBy           TEXT,
    ModifiedBy          TEXT,
    Version             INTEGER NOT NULL DEFAULT 1,
    PageWidth           REAL NOT NULL DEFAULT 210, -- mm
    PageHeight          REAL NOT NULL DEFAULT 297, -- mm (A4)
    Orientation         TEXT NOT NULL DEFAULT 'Portrait', -- Portrait/Landscape
    MarginTop           REAL NOT NULL DEFAULT 25,
    MarginBottom        REAL NOT NULL DEFAULT 25,
    MarginLeft          REAL NOT NULL DEFAULT 25,
    MarginRight         REAL NOT NULL DEFAULT 25,
    BackgroundImage     TEXT,                      -- Path or embedded base64
    IsActive            INTEGER NOT NULL DEFAULT 1, -- Boolean
    Tags                TEXT,                      -- JSON array
    Metadata            TEXT,                      -- JSON object

    CONSTRAINT CHK_Orientation CHECK (Orientation IN ('Portrait', 'Landscape'))
);

CREATE INDEX IX_Templates_Name ON Templates(Name);
CREATE INDEX IX_Templates_CreatedDate ON Templates(CreatedDate);
CREATE INDEX IX_Templates_IsActive ON Templates(IsActive);
```

**Example Data:**
```sql
INSERT INTO Templates (Id, Name, Description, CreatedDate, ModifiedDate, Version, Tags)
VALUES (
    '550e8400-e29b-41d4-a716-446655440000',
    'Invoice Report',
    'Standard invoice template with company logo',
    '2025-01-15T10:30:00Z',
    '2025-01-15T10:30:00Z',
    1,
    '["invoice", "sales", "finance"]'
);
```

---

#### **2.2.2 Sections**

Report sections (header, footer, body, groups).

```sql
CREATE TABLE Sections (
    Id              TEXT PRIMARY KEY,
    TemplateId      TEXT NOT NULL,
    Type            TEXT NOT NULL,              -- Header/Footer/Body/GroupHeader/GroupFooter
    Name            TEXT NOT NULL,
    Height          REAL NOT NULL DEFAULT 50,   -- mm
    BackgroundColor TEXT,                       -- Hex color #RRGGBB
    IsVisible       INTEGER NOT NULL DEFAULT 1,
    PrintOnFirstPage INTEGER NOT NULL DEFAULT 1,
    PrintOnLastPage INTEGER NOT NULL DEFAULT 1,
    KeepTogether    INTEGER NOT NULL DEFAULT 0, -- Prevent page break within section
    OrderIndex      INTEGER NOT NULL,
    Metadata        TEXT,                       -- JSON

    FOREIGN KEY (TemplateId) REFERENCES Templates(Id) ON DELETE CASCADE,
    CONSTRAINT CHK_Type CHECK (Type IN ('ReportHeader', 'PageHeader', 'Detail',
                                        'PageFooter', 'ReportFooter', 'GroupHeader', 'GroupFooter'))
);

CREATE INDEX IX_Sections_TemplateId ON Sections(TemplateId);
CREATE INDEX IX_Sections_Type ON Sections(Type);
CREATE INDEX IX_Sections_OrderIndex ON Sections(OrderIndex);
```

**Example Data:**
```sql
INSERT INTO Sections (Id, TemplateId, Type, Name, Height, OrderIndex)
VALUES
    ('sec-001', '550e8400-e29b-41d4-a716-446655440000', 'PageHeader', 'Page Header', 30, 1),
    ('sec-002', '550e8400-e29b-41d4-a716-446655440000', 'Detail', 'Detail', 20, 2),
    ('sec-003', '550e8400-e29b-41d4-a716-446655440000', 'PageFooter', 'Page Footer', 25, 3);
```

---

#### **2.2.3 Elements**

Report elements (labels, fields, shapes, barcodes, etc.).

```sql
CREATE TABLE Elements (
    Id              TEXT PRIMARY KEY,
    SectionId       TEXT NOT NULL,
    ElementType     TEXT NOT NULL,              -- Label/Field/Line/Rectangle/Image/Barcode/etc.
    Name            TEXT NOT NULL,
    PositionX       REAL NOT NULL,              -- mm from left
    PositionY       REAL NOT NULL,              -- mm from top
    Width           REAL NOT NULL,
    Height          REAL NOT NULL,
    ZIndex          INTEGER NOT NULL DEFAULT 0, -- Layering
    IsVisible       INTEGER NOT NULL DEFAULT 1,
    Content         TEXT,                       -- JSON: type-specific properties
    Style           TEXT,                       -- JSON: font, color, border, etc.
    Binding         TEXT,                       -- Field name or expression
    FormatString    TEXT,                       -- Display format
    OrderIndex      INTEGER NOT NULL,
    Metadata        TEXT,                       -- JSON

    FOREIGN KEY (SectionId) REFERENCES Sections(Id) ON DELETE CASCADE,
    CONSTRAINT CHK_ElementType CHECK (ElementType IN (
        'Label', 'DatabaseField', 'Expression', 'Line', 'Rectangle',
        'RoundedRectangle', 'Ellipse', 'Image', 'Barcode', 'QRCode',
        'PaintBox', 'SubReport'
    ))
);

CREATE INDEX IX_Elements_SectionId ON Elements(SectionId);
CREATE INDEX IX_Elements_ElementType ON Elements(ElementType);
CREATE INDEX IX_Elements_OrderIndex ON Elements(OrderIndex);
```

**Content JSON Structure (varies by type):**

**Label:**
```json
{
  "text": "Invoice Number:",
  "textAlignment": "Left",
  "wordWrap": false
}
```

**DatabaseField:**
```json
{
  "fieldName": "InvoiceNumber",
  "dataSourceName": "Main",
  "nullDisplayText": "N/A"
}
```

**Barcode:**
```json
{
  "barcodeType": "Code128",
  "content": "[ProductCode]",
  "showText": true,
  "moduleWidth": 1,
  "rotation": 0
}
```

**Style JSON Structure:**
```json
{
  "font": {
    "family": "Arial",
    "size": 12,
    "weight": "Normal",
    "style": "Normal"
  },
  "foreground": "#000000",
  "background": "#FFFFFF",
  "border": {
    "width": 1,
    "color": "#000000",
    "style": "Solid"
  },
  "padding": {
    "left": 2,
    "top": 2,
    "right": 2,
    "bottom": 2
  },
  "alignment": {
    "horizontal": "Left",
    "vertical": "Top"
  }
}
```

---

#### **2.2.4 DataSources**

Data source definitions for reports.

```sql
CREATE TABLE DataSources (
    Id                  TEXT PRIMARY KEY,
    TemplateId          TEXT NOT NULL,
    Name                TEXT NOT NULL,          -- Unique within template
    ConnectionStringId  TEXT NOT NULL,
    Query               TEXT NOT NULL,
    QueryType           TEXT NOT NULL DEFAULT 'SQL', -- SQL/StoredProcedure/TableDirect
    CommandTimeout      INTEGER NOT NULL DEFAULT 30, -- Seconds
    IsActive            INTEGER NOT NULL DEFAULT 1,
    CreatedDate         TEXT NOT NULL,
    Metadata            TEXT,                   -- JSON

    FOREIGN KEY (TemplateId) REFERENCES Templates(Id) ON DELETE CASCADE,
    FOREIGN KEY (ConnectionStringId) REFERENCES ConnectionStrings(Id),
    CONSTRAINT CHK_QueryType CHECK (QueryType IN ('SQL', 'StoredProcedure', 'TableDirect')),
    CONSTRAINT UQ_DataSource_Name UNIQUE (TemplateId, Name)
);

CREATE INDEX IX_DataSources_TemplateId ON DataSources(TemplateId);
CREATE INDEX IX_DataSources_ConnectionStringId ON DataSources(ConnectionStringId);
```

**Example Data:**
```sql
INSERT INTO DataSources (Id, TemplateId, Name, ConnectionStringId, Query, QueryType)
VALUES (
    'ds-001',
    '550e8400-e29b-41d4-a716-446655440000',
    'Main',
    'conn-001',
    'SELECT * FROM Invoices WHERE InvoiceDate >= @StartDate',
    'SQL'
);
```

---

#### **2.2.5 DataSourceParameters**

Parameters for data source queries.

```sql
CREATE TABLE DataSourceParameters (
    Id              TEXT PRIMARY KEY,
    DataSourceId    TEXT NOT NULL,
    Name            TEXT NOT NULL,              -- e.g., @StartDate
    DataType        TEXT NOT NULL,              -- String/Integer/DateTime/etc.
    DefaultValue    TEXT,
    IsRequired      INTEGER NOT NULL DEFAULT 0,
    Direction       TEXT NOT NULL DEFAULT 'Input', -- Input/Output/InputOutput
    Metadata        TEXT,                       -- JSON

    FOREIGN KEY (DataSourceId) REFERENCES DataSources(Id) ON DELETE CASCADE,
    CONSTRAINT CHK_DataType CHECK (DataType IN (
        'String', 'Integer', 'Long', 'Decimal', 'DateTime', 'Boolean', 'Guid'
    )),
    CONSTRAINT CHK_Direction CHECK (Direction IN ('Input', 'Output', 'InputOutput'))
);

CREATE INDEX IX_DataSourceParameters_DataSourceId ON DataSourceParameters(DataSourceId);
```

---

#### **2.2.6 ConnectionStrings**

Reusable database connection strings.

```sql
CREATE TABLE ConnectionStrings (
    Id                          TEXT PRIMARY KEY,
    Name                        TEXT NOT NULL UNIQUE,
    ProviderType                TEXT NOT NULL,      -- MSSQL/SQLite/PostgreSQL/MySQL/Oracle
    EncryptedConnectionString   TEXT NOT NULL,      -- Encrypted with DPAPI
    Description                 TEXT,
    IsActive                    INTEGER NOT NULL DEFAULT 1,
    CreatedDate                 TEXT NOT NULL,
    LastTestedDate              TEXT,
    Metadata                    TEXT,               -- JSON

    CONSTRAINT CHK_ProviderType CHECK (ProviderType IN (
        'MSSQL', 'SQLite', 'PostgreSQL', 'MySQL', 'Oracle'
    ))
);

CREATE INDEX IX_ConnectionStrings_Name ON ConnectionStrings(Name);
CREATE INDEX IX_ConnectionStrings_IsActive ON ConnectionStrings(IsActive);
```

**Example Data:**
```sql
INSERT INTO ConnectionStrings (Id, Name, ProviderType, EncryptedConnectionString, CreatedDate)
VALUES (
    'conn-001',
    'Production Database',
    'MSSQL',
    'AQAAANCMnd8BFdERjHoAwE/Cl+s...', -- Encrypted
    '2025-01-15T09:00:00Z'
);
```

---

#### **2.2.7 Expressions**

User-defined expressions/calculations.

```sql
CREATE TABLE Expressions (
    Id              TEXT PRIMARY KEY,
    TemplateId      TEXT NOT NULL,
    Name            TEXT NOT NULL,              -- Display name
    Expression      TEXT NOT NULL,              -- NCalc expression
    DataType        TEXT NOT NULL,              -- Result type
    Description     TEXT,
    Scope           TEXT NOT NULL DEFAULT 'Report', -- Report/Group/Detail
    CreatedDate     TEXT NOT NULL,
    Metadata        TEXT,                       -- JSON

    FOREIGN KEY (TemplateId) REFERENCES Templates(Id) ON DELETE CASCADE,
    CONSTRAINT CHK_DataType CHECK (DataType IN ('String', 'Number', 'DateTime', 'Boolean')),
    CONSTRAINT CHK_Scope CHECK (Scope IN ('Report', 'Group', 'Detail')),
    CONSTRAINT UQ_Expression_Name UNIQUE (TemplateId, Name)
);

CREATE INDEX IX_Expressions_TemplateId ON Expressions(TemplateId);
```

**Example Data:**
```sql
INSERT INTO Expressions (Id, TemplateId, Name, Expression, DataType, CreatedDate)
VALUES (
    'expr-001',
    '550e8400-e29b-41d4-a716-446655440000',
    'TotalAmount',
    'SUM([Quantity] * [UnitPrice])',
    'Number',
    '2025-01-15T10:30:00Z'
);
```

---

#### **2.2.8 Groups**

Grouping configuration for reports.

```sql
CREATE TABLE Groups (
    Id                  TEXT PRIMARY KEY,
    TemplateId          TEXT NOT NULL,
    Name                TEXT NOT NULL,
    GroupField          TEXT NOT NULL,          -- Field name to group by
    SortOrder           TEXT NOT NULL DEFAULT 'Ascending', -- Ascending/Descending
    HeaderSectionId     TEXT,                   -- Reference to Sections
    FooterSectionId     TEXT,
    Level               INTEGER NOT NULL,       -- Nesting level (0-based)
    KeepTogether        INTEGER NOT NULL DEFAULT 0,
    StartOnNewPage      INTEGER NOT NULL DEFAULT 0,
    Metadata            TEXT,                   -- JSON

    FOREIGN KEY (TemplateId) REFERENCES Templates(Id) ON DELETE CASCADE,
    FOREIGN KEY (HeaderSectionId) REFERENCES Sections(Id) ON DELETE SET NULL,
    FOREIGN KEY (FooterSectionId) REFERENCES Sections(Id) ON DELETE SET NULL,
    CONSTRAINT CHK_SortOrder CHECK (SortOrder IN ('Ascending', 'Descending'))
);

CREATE INDEX IX_Groups_TemplateId ON Groups(TemplateId);
CREATE INDEX IX_Groups_Level ON Groups(Level);
```

---

### 2.3 SQLite Migration Script

```sql
-- Enable foreign keys (important for SQLite)
PRAGMA foreign_keys = ON;

-- Create all tables
BEGIN TRANSACTION;

-- (All CREATE TABLE statements from above)

-- Insert sample data for testing
INSERT INTO Templates (Id, Name, Description, CreatedDate, ModifiedDate, Version)
VALUES (
    'sample-template-001',
    'Sample Invoice Report',
    'A sample template for testing',
    datetime('now'),
    datetime('now'),
    1
);

COMMIT;

-- Create views for common queries

CREATE VIEW V_TemplateDetails AS
SELECT
    t.Id AS TemplateId,
    t.Name AS TemplateName,
    t.Description,
    t.CreatedDate,
    t.ModifiedDate,
    COUNT(DISTINCT s.Id) AS SectionCount,
    COUNT(DISTINCT e.Id) AS ElementCount,
    COUNT(DISTINCT ds.Id) AS DataSourceCount
FROM Templates t
LEFT JOIN Sections s ON t.Id = s.TemplateId
LEFT JOIN Elements e ON s.Id = e.SectionId
LEFT JOIN DataSources ds ON t.Id = ds.TemplateId
WHERE t.IsActive = 1
GROUP BY t.Id, t.Name, t.Description, t.CreatedDate, t.ModifiedDate;

CREATE VIEW V_ElementsWithSections AS
SELECT
    e.Id AS ElementId,
    e.Name AS ElementName,
    e.ElementType,
    e.PositionX,
    e.PositionY,
    e.Width,
    e.Height,
    s.Id AS SectionId,
    s.Name AS SectionName,
    s.Type AS SectionType,
    t.Id AS TemplateId,
    t.Name AS TemplateName
FROM Elements e
INNER JOIN Sections s ON e.SectionId = s.Id
INNER JOIN Templates t ON s.TemplateId = t.Id;
```

---

## 3. Production Database (MSSQL)

### 3.1 Overview

The production database is **user-managed** and contains business data. The report generator connects **read-only** to query data.

**Key Points:**
- No schema modifications by report generator
- Read-only access recommended
- Supports standard SQL queries
- Stored procedure support
- Parameterized queries only (SQL injection prevention)

---

### 3.2 Sample Production Schema

This is an **example** schema for documentation purposes. Actual production schemas vary by user.

```sql
-- Sample business database for invoice reporting

CREATE TABLE Customers (
    CustomerId      INT PRIMARY KEY IDENTITY,
    CustomerCode    NVARCHAR(20) NOT NULL UNIQUE,
    CompanyName     NVARCHAR(100) NOT NULL,
    ContactName     NVARCHAR(100),
    Email           NVARCHAR(100),
    Phone           NVARCHAR(20),
    Address         NVARCHAR(200),
    City            NVARCHAR(50),
    PostalCode      NVARCHAR(10),
    Country         NVARCHAR(50),
    CreatedDate     DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsActive        BIT NOT NULL DEFAULT 1
);

CREATE TABLE Invoices (
    InvoiceId       INT PRIMARY KEY IDENTITY,
    InvoiceNumber   NVARCHAR(20) NOT NULL UNIQUE,
    CustomerId      INT NOT NULL,
    InvoiceDate     DATE NOT NULL,
    DueDate         DATE NOT NULL,
    SubTotal        DECIMAL(18, 2) NOT NULL,
    TaxAmount       DECIMAL(18, 2) NOT NULL,
    TotalAmount     DECIMAL(18, 2) NOT NULL,
    Status          NVARCHAR(20) NOT NULL,
    Notes           NVARCHAR(500),
    CreatedDate     DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT FK_Invoices_Customers FOREIGN KEY (CustomerId)
        REFERENCES Customers(CustomerId),
    CONSTRAINT CHK_Status CHECK (Status IN ('Draft', 'Sent', 'Paid', 'Overdue', 'Cancelled'))
);

CREATE TABLE InvoiceItems (
    InvoiceItemId   INT PRIMARY KEY IDENTITY,
    InvoiceId       INT NOT NULL,
    ProductCode     NVARCHAR(20) NOT NULL,
    Description     NVARCHAR(200) NOT NULL,
    Quantity        DECIMAL(18, 2) NOT NULL,
    UnitPrice       DECIMAL(18, 2) NOT NULL,
    Discount        DECIMAL(18, 2) NOT NULL DEFAULT 0,
    LineTotal       AS (Quantity * UnitPrice * (1 - Discount / 100)) PERSISTED,

    CONSTRAINT FK_InvoiceItems_Invoices FOREIGN KEY (InvoiceId)
        REFERENCES Invoices(InvoiceId) ON DELETE CASCADE
);

-- Indexes for reporting performance
CREATE INDEX IX_Invoices_CustomerId ON Invoices(CustomerId);
CREATE INDEX IX_Invoices_InvoiceDate ON Invoices(InvoiceDate);
CREATE INDEX IX_Invoices_Status ON Invoices(Status);
CREATE INDEX IX_InvoiceItems_InvoiceId ON InvoiceItems(InvoiceId);

-- Sample view for reporting
CREATE VIEW V_InvoiceReport AS
SELECT
    i.InvoiceId,
    i.InvoiceNumber,
    i.InvoiceDate,
    i.DueDate,
    i.Status,
    c.CustomerId,
    c.CustomerCode,
    c.CompanyName,
    c.ContactName,
    c.Address,
    c.City,
    c.PostalCode,
    c.Country,
    i.SubTotal,
    i.TaxAmount,
    i.TotalAmount,
    DATEDIFF(DAY, i.DueDate, GETDATE()) AS DaysOverdue
FROM Invoices i
INNER JOIN Customers c ON i.CustomerId = c.CustomerId
WHERE i.Status <> 'Cancelled';

-- Sample stored procedure
CREATE PROCEDURE usp_GetInvoicesByDateRange
    @StartDate DATE,
    @EndDate DATE,
    @Status NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        i.InvoiceId,
        i.InvoiceNumber,
        i.InvoiceDate,
        i.DueDate,
        i.Status,
        c.CompanyName,
        i.TotalAmount
    FROM Invoices i
    INNER JOIN Customers c ON i.CustomerId = c.CustomerId
    WHERE i.InvoiceDate BETWEEN @StartDate AND @EndDate
      AND (@Status IS NULL OR i.Status = @Status)
    ORDER BY i.InvoiceDate DESC;
END;
```

---

## 4. Entity Framework Core Models

### 4.1 Template Entity

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ReportGenerator.Infrastructure.Data.Entities;

public class TemplateEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
    public int Version { get; set; }

    // Page Setup
    public double PageWidth { get; set; }
    public double PageHeight { get; set; }
    public string Orientation { get; set; } = "Portrait";
    public double MarginTop { get; set; }
    public double MarginBottom { get; set; }
    public double MarginLeft { get; set; }
    public double MarginRight { get; set; }
    public string? BackgroundImage { get; set; }

    public bool IsActive { get; set; }
    public string? Tags { get; set; }  // JSON
    public string? Metadata { get; set; }  // JSON

    // Navigation properties
    public ICollection<SectionEntity> Sections { get; set; } = new List<SectionEntity>();
    public ICollection<DataSourceEntity> DataSources { get; set; } = new List<DataSourceEntity>();
    public ICollection<ExpressionEntity> Expressions { get; set; } = new List<ExpressionEntity>();
    public ICollection<GroupEntity> Groups { get; set; } = new List<GroupEntity>();
}

public class TemplateEntityConfiguration : IEntityTypeConfiguration<TemplateEntity>
{
    public void Configure(EntityTypeBuilder<TemplateEntity> builder)
    {
        builder.ToTable("Templates");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.CreatedDate)
            .IsRequired();

        builder.Property(t => t.ModifiedDate)
            .IsRequired();

        builder.Property(t => t.Orientation)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(t => t.Name);
        builder.HasIndex(t => t.CreatedDate);
        builder.HasIndex(t => t.IsActive);

        // Relationships
        builder.HasMany(t => t.Sections)
            .WithOne()
            .HasForeignKey(s => s.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

---

### 4.2 Section Entity

```csharp
public class SectionEntity
{
    public Guid Id { get; set; }
    public Guid TemplateId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double Height { get; set; }
    public string? BackgroundColor { get; set; }
    public bool IsVisible { get; set; }
    public bool PrintOnFirstPage { get; set; }
    public bool PrintOnLastPage { get; set; }
    public bool KeepTogether { get; set; }
    public int OrderIndex { get; set; }
    public string? Metadata { get; set; }  // JSON

    // Navigation properties
    public ICollection<ElementEntity> Elements { get; set; } = new List<ElementEntity>();
}
```

---

### 4.3 Element Entity

```csharp
public class ElementEntity
{
    public Guid Id { get; set; }
    public Guid SectionId { get; set; }
    public string ElementType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double PositionX { get; set; }
    public double PositionY { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public int ZIndex { get; set; }
    public bool IsVisible { get; set; }
    public string? Content { get; set; }  // JSON
    public string? Style { get; set; }  // JSON
    public string? Binding { get; set; }
    public string? FormatString { get; set; }
    public int OrderIndex { get; set; }
    public string? Metadata { get; set; }  // JSON
}
```

---

## 5. Database Migrations

### 5.1 Initial Migration (EF Core)

```bash
# Create initial migration
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project Presentation

# Apply migration
dotnet ef database update --project Infrastructure --startup-project Presentation
```

### 5.2 Migration Class Example

```csharp
using Microsoft.EntityFrameworkCore.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Templates",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                Name = table.Column<string>(maxLength: 200, nullable: false),
                Description = table.Column<string>(maxLength: 1000, nullable: true),
                CreatedDate = table.Column<DateTime>(nullable: false),
                ModifiedDate = table.Column<DateTime>(nullable: false),
                Version = table.Column<int>(nullable: false, defaultValue: 1),
                // ... other columns
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Templates", x => x.Id);
            });

        // ... other tables
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Templates");
        // ... other drops
    }
}
```

---

## 6. Data Access Patterns

### 6.1 Repository Pattern

```csharp
public interface ITemplateRepository
{
    Task<Template?> GetByIdAsync(Guid id);
    Task<IEnumerable<Template>> GetAllAsync();
    Task<IEnumerable<Template>> SearchAsync(string searchTerm);
    Task<Guid> AddAsync(Template template);
    Task UpdateAsync(Template template);
    Task DeleteAsync(Guid id);
}

public class TemplateRepository : ITemplateRepository
{
    private readonly AppDbContext _context;

    public async Task<Template?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Templates
            .Include(t => t.Sections)
                .ThenInclude(s => s.Elements)
            .Include(t => t.DataSources)
            .Include(t => t.Expressions)
            .Include(t => t.Groups)
            .FirstOrDefaultAsync(t => t.Id == id);

        return entity?.ToDomainModel();
    }
}
```

---

## 7. Performance Optimization

### 7.1 Indexes

All critical indexes are created in schema for:
- Primary keys (automatic)
- Foreign keys
- Frequently queried columns (Name, Date, Type)
- Composite indexes where needed

### 7.2 Query Optimization

```csharp
// Use AsNoTracking for read-only queries
var templates = await _context.Templates
    .AsNoTracking()
    .Where(t => t.IsActive)
    .ToListAsync();

// Use projection to load only needed data
var templateList = await _context.Templates
    .Select(t => new
    {
        t.Id,
        t.Name,
        t.ModifiedDate,
        SectionCount = t.Sections.Count
    })
    .ToListAsync();

// Compiled queries for frequently executed operations
private static readonly Func<AppDbContext, Guid, Task<TemplateEntity>> _getTemplateById =
    EF.CompileAsyncQuery((AppDbContext ctx, Guid id) =>
        ctx.Templates.FirstOrDefault(t => t.Id == id));
```

---

This database design provides a solid, normalized foundation for storing report templates while maintaining flexibility for future enhancements.
