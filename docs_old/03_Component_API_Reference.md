# SINEA Report Generator - Component API Reference

## Component: TSestava

### Description
Main report component providing complete report generation, preview, and printing capabilities with template support.

### Hierarchy
```
TComponent
  └─ TSestavaTyp
      └─ TSestavaD
          └─ TSestava
```

### Key Properties

#### Report Configuration
| Property | Type | Description |
|----------|------|-------------|
| `ReportName` | String | Filename of the report template (.PTS) |
| `ReportDir` | String | Directory containing report templates |
| `ReportAlias` | String | BDE alias for template location |
| `Predloha` | TPredloha | Template object (design-time editable) |
| `Background` | String | Background image filename |

#### Data Binding
| Property | Type | Description |
|----------|------|-------------|
| `DataSource` | TSIDataSource | Data source for report data |
| `Bridge` | TBridge | Database bridge object (read-only) |

#### Runtime Control
| Property | Type | Description |
|----------|------|-------------|
| `Prepared` | Boolean | Indicates if report is prepared |
| `IsStored` | Boolean | Template storage flag |

### Key Events

#### Data Events
```pascal
property OnGetData: TSestGetDataEvent;
// Event: procedure(Sender: TSestavaD) of object;
// Purpose: Custom data retrieval for non-database reports
// Usage: Populate fields manually when stepping through data

property OnStepData: TSestStepDataEvent;
// Event: function(Sender: TSestavaD; Dir: TStepDir): TStepResult of object;
// Purpose: Control data navigation
// Returns: srSuccess, srEOF, srBOF
// Directions: sdFirst, sdLast, sdNext, sdPrior
```

#### Data Type Events
```pascal
property OnGetDataType: TGetDataTypeEvent;
// Event: procedure(Sender: TSestavaD; Index: integer; var DataType: TFieldTypeS);
// Purpose: Specify field data types for formatting

property OnGetAlignment: TGetAlignmentEvent;
// Event: procedure(Sender: TSestavaD; Index: integer; var Alignment: TAlignment);
// Purpose: Specify field alignment (taLeftJustify, taCenter, taRightJustify)
```

#### Rendering Events
```pascal
property OnPaintBox: TOnPaintBoxEvent;
// Event: procedure(Sender: TSestavaD; AIndex, AWidth, AHeight: integer; ACanvas: TCanvas);
// Purpose: Custom drawing in PaintBox elements

property OnPaintBoxSize: TOnPaintBoxSizeEvent;
// Event: procedure(Sender: TSestavaD; AIndex, AWidth, AHeight: integer;
//                  var APos, ANext: integer; ACanvas: TCanvas);
// Purpose: Calculate size for PaintBox elements

property OnPrintElement: TOnPrintElementEvent;
// Event: procedure(Sender: TSestavaD; ElementFont: TFont;
//                  const ElementIdent: string; var CanPrint: boolean);
// Purpose: Control which elements print (conditional printing)
```

#### Function Events
```pascal
property OnVypoctiFunkce: TOnVypoctiFunkceEvent;
// Event: procedure(Sender: TSestava);
// Purpose: Calculate custom functions before report rendering
// Usage: Update function values for report
```

#### Section Events
```pascal
property OnTiskOddilu: TOnTiskOddiluEvent;
// Event: procedure(Sender: TSestava; Index: integer; var Tisknout: boolean);
// Purpose: Control section printing
// Index: Section index (odZacatku, odHlavy, odTela, odKonce, odPaty, odSkupiny+)
```

#### Lifecycle Events
```pascal
property BeforeEdit: TNotifyEvent;
property AfterEdit: TNotifyEvent;
property BeforePreview: TNotifyEvent;
property AfterPreview: TNotifyEvent;
property BeforePrint: TNotifyEvent;
property AfterPrint: TNotifyEvent;
property BeforePrintPreview: TNotifyEvent;
property AfterPrintPreview: TNotifyEvent;
```

#### Template Events
```pascal
property OnUlozPredlohu: TUlozPredlohuEvent;
// Event: function(Sender: TSestava): boolean;
// Purpose: Save template to custom storage

property OnUlozPredlohuJako: TUlozPredlohuJakoEvent;
// Event: function(Sender: TSestava; var Soubor: string): boolean;
// Purpose: Save template as... to custom storage

property OnNactiPredlohu: TNactiPredlohuEvent;
// Event: function(Sender: TSestava): boolean;
// Purpose: Load template from custom storage

property OnNactiSeznamObrazu: TNactiSeznamObrazuEvent;
// Event: procedure(Sender: TSestava; Items: TStrings);
// Purpose: Provide list of available images

property OnNactiObraz: TNactiObrazEvent;
// Event: function(Sender: TSestava; const ANazev: String; APicture: TPicture): boolean;
// Purpose: Load image from custom storage

property OnPreparePicture: TPreparePictureEvent;
// Event: procedure(Sender: TSestava; APicture: TPicture; AWidth, AHeight: integer);
// Purpose: Process/resize images before rendering
```

#### Error Handling
```pascal
property OnError: TOnErrorEvent;
// Event: function(Sender: TSestava; Error: TSestError): boolean;
// Purpose: Handle report errors
// Returns: True to continue, False to abort
```

### Key Methods

#### Report Execution
```pascal
procedure Print;
// Prints the report to the default printer

procedure Preview;
// Shows print preview dialog

procedure PrintPreview;
// Prints and shows preview

procedure Edit;
// Opens visual report designer

function Execute: boolean;
// Prepares and executes the report
```

#### Template Management
```pascal
function LoadTemplate: boolean;
// Loads template from file

function SaveTemplate: boolean;
// Saves template to file

function SaveTemplateAs(const Filename: string): boolean;
// Saves template with new name

procedure EditDesign;
// Opens template editor
```

#### Data Control
```pascal
procedure Aktivuj;
// Activates the report (opens dataset)

procedure Deaktivuj;
// Deactivates the report (closes dataset)

function KontrolaAktivity: boolean;
// Checks if report is active

function MaDetail: boolean;
// Returns True if report has detail section
```

#### Field Access
```pascal
function PolozkaAsString(Index: integer): string;
// Gets field value as string

function PolozkaAsFloat(Index: integer): double;
// Gets field value as float

function PolozkaAsGraphic(Index: integer): TPicture;
// Gets field value as graphic

function PolozkaIsNull(Index: integer): boolean;
// Checks if field is null

function DataTypePolozky(Index: integer): TFieldTypeS;
// Gets field data type

function AlignmentPolozky(Index: integer): TAlignment;
// Gets field alignment
```

### Usage Example
```pascal
var
  Report: TSestava;
begin
  Report := TSestava.Create(Self);
  try
    Report.DataSource := DataSource1;
    Report.ReportDir := 'c:\reports\';
    Report.ReportName := 'invoice.pts';

    // Optional: Handle functions
    Report.OnVypoctiFunkce := ReportCalculateFunctions;

    // Execute
    Report.Preview;
  finally
    Report.Free;
  end;
end;

procedure TForm1.ReportCalculateFunctions(Sender: TSestava);
begin
  // Set function values
  Sender.SetFunction('TOTAL', CalculateTotal);
  Sender.SetFunction('DATE', FormatDateTime('dd.mm.yyyy', Now));
end;
```

---

## Component: TSestDetail

### Description
Detail report component for master-detail relationships and sub-reports.

### Hierarchy
```
TComponent
  └─ TSestavaTyp
      └─ TSestavaD
          └─ TSestDetail
```

### Key Differences from TSestava
- Designed for use as child report
- Can be embedded in master report
- Supports detail bands
- Synchronized with master data

### Key Events
```pascal
property OnVypoctiFunkce: TOnVypoctiFunkceDetEvent;
property OnTiskOddilu: TOnTiskOddiluDetEvent;
```

### Usage Example
```pascal
// In master report, embed detail
Detail := TSestDetail.Create(Self);
Detail.DataSource := DetailDataSource;
Detail.ReportName := 'detail.pts';
```

---

## Component: TCompositeReportSinea

### Description
Manages multiple reports as a single composite unit for batch printing.

### Key Features
- Print multiple reports sequentially
- Unified preview
- Single print job
- Report ordering

### Key Properties
```pascal
property Reports: TStrings;
// List of report components to include
```

### Key Methods
```pascal
procedure Print;
// Prints all reports in sequence

procedure Preview;
// Shows preview of all reports

procedure AddReport(AReport: TSestava);
// Adds report to composite

procedure RemoveReport(AReport: TSestava);
// Removes report from composite
```

### Usage Example
```pascal
var
  Composite: TCompositeReportSinea;
begin
  Composite := TCompositeReportSinea.Create(Self);
  try
    Composite.AddReport(Report1);
    Composite.AddReport(Report2);
    Composite.AddReport(Report3);
    Composite.Preview; // Shows all three reports
  finally
    Composite.Free;
  end;
end;
```

---

## Component: TComboBoxPredloha

### Description
ComboBox control for selecting report templates.

### Key Properties
```pascal
property ReportDir: string;
// Directory to scan for .PTS files

property Report: TSestava;
// Associated report component

property Items: TStrings;
// List of available templates (read-only)
```

### Behavior
- Automatically populates with .PTS files from ReportDir
- Updates associated report when selection changes
- Filters by file extension

---

## Component: TComboBoxPrinter

### Description
ComboBox control for selecting printers.

### Key Properties
```pascal
property Items: TStrings;
// List of installed printers (read-only)

property SelectedPrinter: string;
// Currently selected printer
```

### Behavior
- Automatically populates with installed printers
- Updates printer selection globally or for specific report

---

## Component: TPrintButton

### Description
Button control for quick printing of single reports.

### Key Properties
```pascal
property Report: TSestava;
// Associated report component

property ShowPreview: boolean;
// Show preview before printing (default: True)

property Caption: string;
// Button text

property Glyph: TBitmap;
// Button icon
```

### Key Events
```pascal
property OnBeforePrint: TNotifyEvent;
property OnAfterPrint: TNotifyEvent;
```

### Usage Example
```pascal
PrintButton1.Report := Sestava1;
PrintButton1.ShowPreview := True;
// Click button to print
```

---

## Component: TPrintButtonComposite

### Description
Button control for quick printing of composite reports.

### Key Properties
```pascal
property CompositeReport: TCompositeReportSinea;
// Associated composite report

property ShowPreview: boolean;
// Show preview before printing
```

### Behavior
Same as TPrintButton but for composite reports.

---

## Component: TPrintDialogSinea

### Description
Custom print dialog for SINEA reports.

### Key Properties
```pascal
property Report: TSestava;
// Associated report

property PrintRange: TPrintRange;
// prAllPages, prSelection, prPageRange

property FromPage: integer;
property ToPage: integer;
// Page range for printing

property Copies: integer;
// Number of copies

property Collate: boolean;
// Collate copies
```

### Key Methods
```pascal
function Execute: boolean;
// Shows dialog and returns True if user clicks OK
```

### Usage Example
```pascal
if PrintDialogSinea1.Execute then
begin
  // User clicked OK, print with settings
  Sestava1.Print;
end;
```

---

## Type Reference

### TFieldTypeS (Field Data Types)
```pascal
type TFieldTypeS = (
  ftsUnknown,      // Unknown type
  ftsString,       // String
  ftsSmallint,     // 16-bit integer
  ftsInteger,      // 32-bit integer
  ftsWord,         // 16-bit unsigned
  ftsBoolean,      // Boolean
  ftsFloat,        // Floating point
  ftsCurrency,     // Currency
  ftsBCD,          // BCD decimal
  ftsDate,         // Date only
  ftsTime,         // Time only
  ftsDateTime,     // Date and time
  ftsBytes,        // Byte array
  ftsVarBytes,     // Variable bytes
  ftsAutoInc,      // Auto-increment
  ftsBlob,         // BLOB
  ftsMemo,         // Memo text
  ftsGraphic,      // Graphic
  ftsFmtMemo,      // Formatted memo
  ftsParadoxOle,   // Paradox OLE
  ftsDBaseOle,     // dBase OLE
  ftsTypedBinary,  // Binary
  ftsCursor,       // Cursor
  ftsFixedChar,    // Fixed char
  ftsWideString,   // Unicode string
  ftsLargeint,     // 64-bit integer
  ftsADT,          // ADT
  ftsArray,        // Array
  ftsReference,    // Reference
  ftsDataSet,      // Nested dataset
  ftsOraBlob,      // Oracle BLOB
  ftsOraClob,      // Oracle CLOB
  ftsVariant,      // Variant
  ftsInterface,    // Interface
  ftsIDispatch,    // IDispatch
  ftsGuid,         // GUID
  ftsTimeStamp,    // Timestamp
  ftsFMTBcd,       // Formatted BCD
  ftsFixedWideChar,// Fixed wide char
  ftsWideMemo,     // Wide memo
  ftsOraTimeStamp, // Oracle timestamp
  ftsOraInterval,  // Oracle interval
  ftsLongWord,     // 32-bit unsigned
  ftsShortint,     // 8-bit signed
  ftsByte,         // 8-bit unsigned
  ftsExtended,     // Extended float
  ftsConnection,   // Connection
  ftsParams,       // Parameters
  ftsStream,       // Stream
  ftsTimeStampOffset, // Timestamp with offset
  ftsObject,       // Object
  ftsSingle        // Single precision float
);
```

### TStepDir (Navigation Direction)
```pascal
type TStepDir = (
  sdFirst,  // Move to first record
  sdLast,   // Move to last record
  sdNext,   // Move to next record
  sdPrior   // Move to previous record
);
```

### TStepResult (Navigation Result)
```pascal
type TStepResult = (
  srSuccess,  // Navigation successful
  srEOF,      // End of file reached
  srBOF       // Beginning of file reached
);
```

### TSestError (Report Errors)
```pascal
type TSestError = (
  seFileNotFound,      // Template file not found
  seInvalidFormat,     // Invalid template format
  sePrinterError,      // Printer error
  seDataError,         // Data error
  seExpressionError,   // Expression evaluation error
  seRenderError        // Rendering error
);
```

---

## Constants Reference

### Section Indices
```pascal
const
  odZacatku = 0;  // Report header (prints once at start)
  odHlavy   = 1;  // Page header (prints at top of each page)
  odTela    = 2;  // Detail/body (prints for each record)
  odKonce   = 3;  // Report footer (prints once at end)
  odPaty    = 4;  // Page footer (prints at bottom of each page)
  odSkupiny = 5;  // First group header index (groups are 5, 6, 7...)
```

### Element Type Constants
```pascal
const
  ccSesLabel    = 1;  // Static label
  ccSesCara     = 2;  // Line
  ccSesRam      = 3;  // Rectangle
  ccSesRamObl   = 4;  // Rounded rectangle
  ccSesElipsa   = 5;  // Ellipse
  ccSesObraz    = 6;  // Image/graphic
  ccSesBarCode  = 7;  // Barcode
  ccSesPaintBox = 8;  // Custom drawing area
```

### Aggregate Functions
```pascal
const
  dzNic      = 0;  // None
  dzPocet    = 1;  // Count
  dzSoucet   = 2;  // Sum
  dzPrumer   = 3;  // Average
  dzMinimum  = 4;  // Minimum
  dzMaximum  = 5;  // Maximum
  dzSmerOd   = 6;  // Direction from
  dzRelSmOd  = 7;  // Relative direction from
```

---

## Bridge Interface

### TBridge (Database Abstraction)
```pascal
type TBridge = class
  // Dataset control
  procedure Open;
  procedure Close;
  function Active: boolean;

  // Navigation
  procedure First;
  procedure Last;
  procedure Next;
  procedure Prior;
  function EOF: boolean;
  function BOF: boolean;

  // State control
  procedure DisableControls;
  procedure EnableControls;

  // Field access
  function FieldCount: integer;
  function FieldName(Index: integer): string;
  function FieldType(Index: integer): TFieldTypeS;
  function FieldValue(Index: integer): Variant;
  function FieldAsString(Index: integer): string;
  function FieldAsFloat(Index: integer): double;
  function FieldIsNull(Index: integer): boolean;
end;
```

The Bridge pattern allows SINEA to work with different database engines (BDE, IBObjects, ADO) through a uniform interface.
