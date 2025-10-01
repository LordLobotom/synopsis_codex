# SINEA Report Generator - Usage Guide

## Quick Start

### Installation

1. **For Runtime (Application Distribution):**
   - Include appropriate `sessinea*.bpl` runtime package
   - Or link statically by including units in uses clause

2. **For Development:**
   - Install design-time package `sessinea*d.bpl` in Delphi IDE
   - Components will appear in "Sinea" palette tab

### Package Selection by Delphi Version

| Delphi Version | Runtime Package | Design Package |
|----------------|----------------|----------------|
| Delphi 4 | sessinea40.dpk | N/A |
| Delphi 5 | sessinea50.dpk | sessinea50d.dpk |
| Delphi 6 | sessinea60.dpk | sessinea60d.dpk |
| Delphi 7 | sessinea70.dpk | sessinea70d.dpk |
| Delphi 2009 | sessinea2009.dpk | sessinea2009d.dpk |
| Delphi 2010 | sessinea2010.dpk | sessinea2010d.dpk |
| Delphi XE | sessineaXE.dpk | sessineaXEd.dpk |
| Delphi XE2 | sessineaXE2.dpk | sessineaXE2d.dpk |
| Delphi XE3 | sessineaXE3.dpk | sessineaXE3d.dpk |

---

## Basic Usage Scenarios

### Scenario 1: Simple Database Report

**Goal:** Print a customer list from a database table.

#### Step 1: Place Components
```pascal
// On form:
- TSestava (Report1)
- TDataSource (DataSource1)
- TTable (Table1)
```

#### Step 2: Configure Components
```pascal
// Set at design-time:
Table1.DatabaseName := 'DBDEMOS';
Table1.TableName := 'CUSTOMER.DB';
Table1.Active := True;

DataSource1.DataSet := Table1;

Report1.DataSource := DataSource1;
Report1.ReportDir := '.\reports\';
Report1.ReportName := 'customers.pts';
```

#### Step 3: Design Template
```pascal
// At design-time, double-click Report1
// This opens the visual report designer

// Add elements:
1. Page Header section:
   - Add Label: "Customer List"
   - Add Labels: "Name", "Address", "Phone"

2. Detail section:
   - Add DB Field: [CustNo]
   - Add DB Field: [Company]
   - Add DB Field: [Addr1]
   - Add DB Field: [Phone]

3. Page Footer section:
   - Add Function: PAGE_NUMBER

// Save template (File -> Save)
```

#### Step 4: Print Report
```pascal
procedure TForm1.Button1Click(Sender: TObject);
begin
  Report1.Preview;  // or Report1.Print;
end;
```

---

### Scenario 2: Report with Calculations

**Goal:** Create invoice with totals.

#### Step 1: Design Template with Functions
```pascal
// In report designer:
1. Detail section:
   - [Quantity] × [Price] = calculated inline

2. Report Footer:
   - Label: "Total:"
   - Function: SUM([Quantity] * [Price])
```

#### Step 2: Handle Function Event
```pascal
procedure TForm1.Report1VypoctiFunkce(Sender: TSestava);
var
  Total: Double;
begin
  Total := 0;
  Table1.First;
  while not Table1.EOF do
  begin
    Total := Total + Table1.FieldByName('Amount').AsFloat;
    Table1.Next;
  end;
  Table1.First;

  // Set function value
  Sender.SetFunction('TOTAL', Total);
end;
```

---

### Scenario 3: Master-Detail Report

**Goal:** Print orders with order items.

#### Step 1: Setup Data
```pascal
// Master-Detail relationship
Table1.DatabaseName := 'DBDEMOS';
Table1.TableName := 'ORDERS.DB';

Table2.DatabaseName := 'DBDEMOS';
Table2.TableName := 'ITEMS.DB';
Table2.MasterSource := DataSource1;
Table2.MasterFields := 'OrderNo';
Table2.IndexFieldNames := 'OrderNo';

DataSource1.DataSet := Table1;
DataSource2.DataSet := Table2;
```

#### Step 2: Configure Reports
```pascal
// Master report
Report1.DataSource := DataSource1;
Report1.ReportName := 'orders.pts';

// Detail report
DetailReport1.DataSource := DataSource2;
DetailReport1.ReportName := 'order_items.pts';
```

#### Step 3: Design Templates
```pascal
// Master template (orders.pts):
- Report Header: "Orders Report"
- Page Header: Order headers
- Detail: Order info + placeholder for detail
- Page Footer: Page number

// Detail template (order_items.pts):
- Detail only: Item info, quantity, price
```

#### Step 4: Embed Detail in Master
```pascal
// In master report designer:
1. Add "Detail" element to Detail section
2. Point to DetailReport1 component
3. Position and size appropriately
```

---

### Scenario 4: Non-Database Report

**Goal:** Create report from custom data source (arrays, objects).

#### Step 1: Setup Report
```pascal
Report1.DataSource := nil;  // No database
Report1.OnGetData := Report1GetData;
Report1.OnStepData := Report1StepData;
```

#### Step 2: Data Structure
```pascal
type
  TCustomerRec = record
    Name: string;
    Address: string;
    Phone: string;
  end;

var
  CustomerList: array of TCustomerRec;
  CurrentIndex: Integer;
```

#### Step 3: Implement Data Events
```pascal
procedure TForm1.Report1GetData(Sender: TSestavaD);
begin
  // Populate fields from current record
  if (CurrentIndex >= 0) and (CurrentIndex < Length(CustomerList)) then
  begin
    Sender.SetField(0, CustomerList[CurrentIndex].Name);
    Sender.SetField(1, CustomerList[CurrentIndex].Address);
    Sender.SetField(2, CustomerList[CurrentIndex].Phone);
  end;
end;

function TForm1.Report1StepData(Sender: TSestavaD; Dir: TStepDir): TStepResult;
begin
  case Dir of
    sdFirst:
      begin
        CurrentIndex := 0;
        Result := srSuccess;
      end;

    sdLast:
      begin
        CurrentIndex := Length(CustomerList) - 1;
        Result := srSuccess;
      end;

    sdNext:
      begin
        Inc(CurrentIndex);
        if CurrentIndex >= Length(CustomerList) then
          Result := srEOF
        else
          Result := srSuccess;
      end;

    sdPrior:
      begin
        Dec(CurrentIndex);
        if CurrentIndex < 0 then
          Result := srBOF
        else
          Result := srSuccess;
      end;
  else
    Result := srEOF;
  end;
end;
```

#### Step 4: Populate Data and Print
```pascal
procedure TForm1.Button1Click(Sender: TObject);
begin
  // Populate custom data
  SetLength(CustomerList, 3);
  CustomerList[0].Name := 'John Doe';
  CustomerList[0].Address := '123 Main St';
  CustomerList[0].Phone := '555-1234';
  // ... more records ...

  CurrentIndex := 0;
  Report1.Preview;
end;
```

---

### Scenario 5: Conditional Printing

**Goal:** Print certain elements only when conditions are met.

#### Method 1: OnPrintElement Event
```pascal
procedure TForm1.Report1PrintElement(Sender: TSestavaD;
  ElementFont: TFont; const ElementIdent: string; var CanPrint: boolean);
begin
  // Don't print discount if zero
  if ElementIdent = 'DISCOUNT' then
    CanPrint := Table1.FieldByName('Discount').AsFloat > 0;

  // Don't print notes if empty
  if ElementIdent = 'NOTES' then
    CanPrint := Table1.FieldByName('Notes').AsString <> '';
end;
```

#### Method 2: OnTiskOddilu Event (Section Control)
```pascal
procedure TForm1.Report1TiskOddilu(Sender: TSestava;
  Index: integer; var Tisknout: boolean);
begin
  // Don't print page header on first page
  if (Index = odHlavy) and (Sender.CurrentPage = 1) then
    Tisknout := False;

  // Only print summary section if total > 0
  if (Index = odKonce) and (TotalAmount = 0) then
    Tisknout := False;
end;
```

---

### Scenario 6: Custom Graphics

**Goal:** Add custom drawings to report.

#### Step 1: Add PaintBox Element
```pascal
// In report designer:
1. Add element type "PaintBox"
2. Set size and position
3. Assign index number (e.g., 0)
```

#### Step 2: Implement OnPaintBox Event
```pascal
procedure TForm1.Report1PaintBox(Sender: TSestavaD;
  AIndex, AWidth, AHeight: integer; ACanvas: TCanvas);
begin
  case AIndex of
    0: // Draw chart
      begin
        ACanvas.Brush.Color := clYellow;
        ACanvas.Rectangle(0, 0, AWidth, AHeight);
        ACanvas.Brush.Color := clBlue;
        ACanvas.Rectangle(10, 10, 50, 50);
      end;

    1: // Draw logo
      begin
        if FileExists('logo.bmp') then
          ACanvas.Draw(0, 0, LoadBitmap('logo.bmp'));
      end;
  end;
end;
```

---

### Scenario 7: Barcode Printing

**Goal:** Print barcodes on labels.

#### Step 1: Add Barcode Element
```pascal
// In report designer:
1. Add element type "BarCode"
2. Configure properties:
   - BarcodeType: bcCode39, bcCodeEAN13, etc.
   - Module: Bar width
   - ShowText: True/False
   - CheckSum: True/False
3. Bind to data field or expression
```

#### Step 2: Barcode from Database Field
```pascal
// In designer, set barcode source:
- Field: [ProductCode]
- Or Expression: [Prefix] + [ProductCode]
```

---

### Scenario 8: Grouping

**Goal:** Group report by category with subtotals.

#### Step 1: Design Template with Groups
```pascal
// In report designer:
1. Add Group Header section:
   - Add Label: [CategoryName]

2. Detail section:
   - Product info

3. Add Group Footer section:
   - Add Function: "Subtotal: " + SUM([Amount])

4. Configure group:
   - Group by field: Category
   - Break on: Category change
```

#### Step 2: Ensure Data is Sorted
```pascal
Table1.IndexFieldNames := 'Category;ProductName';
// Or SQL: ORDER BY Category, ProductName
```

---

### Scenario 9: Multi-Report Printing

**Goal:** Print multiple related reports as one job.

#### Step 1: Setup Composite Report
```pascal
// Place on form:
- TCompositeReportSinea (CompositeReport1)
- Multiple TSestava components (Report1, Report2, Report3)
```

#### Step 2: Configure Composite
```pascal
CompositeReport1.Reports.Add('Report1');
CompositeReport1.Reports.Add('Report2');
CompositeReport1.Reports.Add('Report3');
```

#### Step 3: Print All
```pascal
procedure TForm1.ButtonPrintAllClick(Sender: TObject);
begin
  CompositeReport1.Preview;  // Shows all reports
end;
```

---

### Scenario 10: Runtime Template Loading

**Goal:** Let user select template at runtime.

#### Method 1: Using TComboBoxPredloha
```pascal
// Place on form:
- TComboBoxPredloha (ComboBoxPredloha1)

// Configure:
ComboBoxPredloha1.ReportDir := 'c:\reports\';
ComboBoxPredloha1.Report := Report1;

// User selects from combo, report automatically updates
```

#### Method 2: Programmatic Selection
```pascal
procedure TForm1.SelectTemplate(const TemplateName: string);
begin
  Report1.ReportName := TemplateName;
  if Report1.LoadTemplate then
    Report1.Preview
  else
    ShowMessage('Template not found');
end;
```

---

## Report Designer Guide

### Opening the Designer
```pascal
// Design-time: Double-click TSestava component
// Runtime: Report1.Edit;
```

### Designer Interface Elements

#### Toolbar Buttons
- **Select** - Selection tool
- **Label** - Add static text
- **DB Field** - Add database field
- **Line** - Draw line
- **Rectangle** - Draw rectangle
- **Rounded Rect** - Draw rounded rectangle
- **Ellipse** - Draw ellipse/circle
- **Image** - Add image
- **Barcode** - Add barcode
- **PaintBox** - Add custom drawing area
- **Detail** - Add sub-report

#### Sections
- **Report Header** - Prints once at start
- **Page Header** - Top of each page
- **Detail** - Once per record
- **Page Footer** - Bottom of each page
- **Report Footer** - Prints once at end
- **Groups** - Group headers/footers

### Adding Elements

1. **Click toolbar button** for element type
2. **Click and drag** on report canvas
3. **Double-click element** to configure properties
4. **Right-click element** for context menu

### Element Properties

#### Text Elements
- Font, Size, Style, Color
- Alignment (Left, Center, Right)
- Word wrap
- Auto-size
- Format mask

#### Graphic Elements
- Line width
- Line color
- Fill color
- Border style

#### Database Fields
- Field name
- Display format
- Null value display

#### Functions
- Function expression
- Aggregate scope
- Format

### Expression Editor

Access via: Double-click function element or right-click field

**Operators:**
```
+  -  *  /  div  mod
=  <>  <  >  <=  >=
AND  OR  NOT
```

**Examples:**
```
[Quantity] * [Price]
[FirstName] + ' ' + [LastName]
IIF([Amount] > 1000, 'High', 'Normal')
FORMAT('%.2f', [Price])
UPPER([Code])
```

### Alignment Tools

- **Align Left/Right/Top/Bottom** - Align multiple elements
- **Align to Grid** - Snap to grid
- **Same Width/Height** - Make elements same size
- **Space Evenly** - Distribute elements

### Section Height

- **Drag section divider** to resize
- **Double-click divider** for precise height
- **Right-click section** for properties

---

## Troubleshooting

### Common Issues

#### Template Not Found
```pascal
// Check paths
ShowMessage(Report1.NazevSouboru);  // Full path to template

// Ensure directory exists
if not DirectoryExists(Report1.ReportDir) then
  CreateDir(Report1.ReportDir);
```

#### No Data Printing
```pascal
// Verify data source
if Report1.DataSource = nil then
  ShowMessage('No data source');

// Check if dataset is active
if not Report1.DataSource.DataSet.Active then
  Report1.Aktivuj;

// Verify records exist
if Report1.DataSource.DataSet.IsEmpty then
  ShowMessage('No records');
```

#### Fonts Not Printing Correctly
```pascal
// Ensure fonts are installed on print computer
// Use standard fonts for compatibility:
// Arial, Times New Roman, Courier New

// Check printer capabilities
if Printer.Fonts.IndexOf('Arial') < 0 then
  ShowMessage('Font not available on printer');
```

#### Page Breaks in Wrong Place
```pascal
// Adjust section heights
// Enable "Keep Together" on grouped elements
// Check page margins and paper size
```

#### Expression Errors
```pascal
// Handle in OnError event
function TForm1.Report1Error(Sender: TSestava;
  Error: TSestError): boolean;
begin
  case Error of
    seExpressionError:
      begin
        ShowMessage('Expression error: check function syntax');
        Result := True;  // Continue with blank value
      end;
  else
    Result := False;  // Abort
  end;
end;
```

---

## Performance Tips

### Large Datasets
```pascal
// Disable controls during report generation
Report1.Bridge.DisableControls;
try
  Report1.Print;
finally
  Report1.Bridge.EnableControls;
end;
```

### Complex Expressions
```pascal
// Pre-calculate values in OnVypoctiFunkce
// Store in function variables instead of recalculating
```

### Multi-Pass Reports
```pascal
// Reports with aggregates require two passes
// Minimize aggregate usage for faster printing
// Use OnVypoctiFunkce for manual calculation when possible
```

### Preview Performance
```pascal
// Preview generates all pages
// For large reports, consider page range:
Report1.PrintRange := prPageRange;
Report1.FromPage := 1;
Report1.ToPage := 10;
```

---

## Best Practices

1. **Always save templates** with meaningful names
2. **Use consistent naming** for database fields
3. **Test with sample data** before production
4. **Handle errors gracefully** with OnError event
5. **Document custom functions** in code comments
6. **Keep expressions simple** for maintainability
7. **Use groups** instead of manual break logic
8. **Version templates** when making changes
9. **Include page numbers** in page footer
10. **Test on target printer** before deployment
