# SINEA Report Generator - Module Reference

## Core Modules

### 1. sestavy.pas
**Purpose:** Main component implementation
**Key Classes:**
- `TSestavaD` - Base report component with data binding
- `TSestava` - Full-featured report component
- `TSestDetail` - Detail/sub-report component
- `TCompositeReportSinea` - Composite report manager

**Key Events:**
- `OnGetData` - Custom data retrieval
- `OnStepData` - Data navigation control
- `OnGetDataType` - Field type determination
- `OnPaintBox` - Custom drawing
- `OnPrintElement` - Element print filtering
- `OnVypoctiFunkce` - Function calculation
- `OnTiskOddilu` - Section print control
- `OnError` - Error handling

**Responsibilities:**
- Component lifecycle management
- Event dispatching
- Data source integration
- Report execution coordination

---

### 2. SestavyT.pas
**Purpose:** Type definitions and base report template engine
**Key Classes:**
- `TSestavaTyp` - Base template class
- `TPredloha` - Template container
- `TOddilSestavy` - Report section
- `TSiReportElement` - Base element class

**Responsibilities:**
- Template structure definition
- Report element hierarchy
- Section management
- Template serialization

---

### 3. sest_typ.pas
**Purpose:** Constants, types, and version information
**Key Constants:**
```pascal
// Section indices
odZacatku = 0;  // Report header
odHlavy   = 1;  // Page header
odTela    = 2;  // Body
odKonce   = 3;  // Report footer
odPaty    = 4;  // Page footer
odSkupiny = 5;  // Group sections

// Element types
ccSesLabel    = 1;  // Label
ccSesCara     = 2;  // Line
ccSesRam      = 3;  // Rectangle
ccSesRamObl   = 4;  // Rounded rectangle
ccSesElipsa   = 5;  // Ellipse
ccSesObraz    = 6;  // Image
ccSesBarCode  = 7;  // Barcode
ccSesPaintBox = 8;  // Custom drawing
```

**Key Types:**
- `TFieldTypeS` - Field data types (50+ types)
- `TValType` - Expression value types

**Version Information:**
- `VerzeKnihovny` - Library version number (4-16)
- `PodVerzeKnihovny` - Sub-version (62)
- `RevizeKnihovny` - Revision (3)

---

### 4. sest_fn.pas
**Purpose:** Expression evaluator and function library
**Function Count:** 43 built-in functions

**Function Categories:**

**Mathematical:**
- `ABS(x)` - Absolute value
- `INT(x)` - Integer part
- `SGN(x)` - Sign
- `MAX(a,b)` - Maximum
- `MIN(a,b)` - Minimum
- `ROUND(x)` - Rounding
- `EXROUND(x,d)` - Extended rounding

**Trigonometric:**
- `SIN(x)`, `COS(x)`, `TAN(x)`, `COTG(x)`
- `ASIN(x)`, `ACOS(x)`, `ATAN(x)`, `ACOTG(x)`
- `EXP(x)`, `LOG(x)`

**String:**
- `LEFT(s,n)` - Left substring
- `RIGHT(s,n)` - Right substring
- `SUBSTR(s,pos,len)` - Substring
- `UPPER(s)` - Uppercase
- `LOWER(s)` - Lowercase
- `LTRIM(s)`, `RTRIM(s)`, `ALLTRIM(s)` - Trimming
- `LENGTH(s)` - String length
- `POS(sub,s)` - Position search
- `CHR(n)` - Character from code
- `ORD(c)` - Character code

**Date/Time:**
- `YEAR(d)`, `MONTH(d)`, `DAY(d)`
- `HOUR(t)`, `MINUTE(t)`, `SECOND(t)`
- `FORMATDT(fmt,dt)` - Date/time formatting
- `STRTODA(s)` - String to date
- `STRTODT(s)` - String to datetime

**Conversion:**
- `FORMAT(fmt,val)` - Formatted output
- `STRTONUM(s)` - String to number
- `NUMTOSTR(n)` - Number to string

**Logic:**
- `IIF(cond,true_val,false_val)` - Conditional
- `ISNULL(val)` - Null check

**Key Classes:**
- `TCalcType` - Expression value container
- `TCalcHandler` - Expression evaluator callback

**Expression Types:**
- `vtNeurc` - Undetermined
- `vtCislo` - Number
- `vtRetez` - String
- `vtDatum` - Date
- `vtLogic` - Boolean
- `vtMnozina` - Set

---

### 5. sest_cmp.pas
**Purpose:** Component implementation details (compiled only)
**Status:** Source not available (only .dcu present)
**Presumed Content:**
- Component property implementations
- Component state management
- IDE integration support

---

### 6. sest_rtf.pas
**Purpose:** RTF (Rich Text Format) export functionality
**Key Features:**
- RTF document generation
- Font and style mapping
- Table generation
- Image embedding
- Color handling

**Responsibilities:**
- Convert report to RTF format
- Handle text formatting
- Manage RTF structure
- Export to file or stream

---

### 7. sest_str.pas
**Purpose:** String resources and localization
**Contains:**
- Czech language strings
- Error messages
- UI labels
- Dialog captions
- Menu text

**Key Resource Groups:**
- Editor messages
- Dialog prompts
- Error descriptions
- Format strings

---

### 8. sest_dlg.pas
**Purpose:** Standard dialogs
**Key Classes:**
- `TSestPrintDlg` - Print dialog
- Custom print configuration dialogs

**Features:**
- Printer selection
- Page range selection
- Copy count
- Print options

---

### 9. sest_new.pas
**Purpose:** New report wizard
**Key Classes:**
- `TDlgNovaSestava` - New report dialog

**Features:**
- Template selection
- Initial configuration
- Report type selection

---

### 10. sestdb.pas
**Purpose:** Designer database integration
**Key Classes:**
- `TSesPrvek` - Visual element in designer
- `TOddil` - Section in designer
- `TPapir` - Paper/canvas representation

**Responsibilities:**
- Visual element rendering in designer
- Mouse handling
- Element selection
- Drag and drop support
- Undo/Redo management

**Command Constants:**
```pascal
cm_LeftTop      = 1;   // Position
cm_WidthHeight  = 2;   // Size
cm_StiskniSipku = 4;   // Arrow key
cm_NastavAktPrv = 5;   // Set active element
cm_ZjistiAktPrv = 6;   // Get active element
cm_VyberVice    = 8;   // Multi-select
cm_UlozUndo     = 10;  // Save undo
cm_KonfOdd      = 11;  // Section config
cm_KonfPrv      = 12;  // Element config
```

**Undo Actions:**
```pascal
zzVlozPrv    = 1;   // Insert element
zzZmenaPisma = 2;   // Font change
zzZrusPrvek  = 3;   // Delete element
zzNastOdd    = 4;   // Section settings
zzVyskaOdd   = 5;   // Section height
zzPosunPrv   = 7;   // Move element
zzOprFunkce  = 9;   // Edit function
```

---

### 11. seste.pas
**Purpose:** Report template editor main form
**Key Classes:**
- `TFormSestE` - Main editor form

**Features:**
- Visual design surface
- Toolbox
- Property inspector integration
- Menu and toolbar
- Section headers
- Grid and rulers

**Form Size:**
- Large form (186KB .dfm file)
- Complex UI with multiple panels
- Extensive menu system

---

### 12. sestbarc.pas
**Purpose:** Barcode rendering engine
**Author:** Jan Tungli

**Supported Barcode Types:**
```pascal
bcCode_2_5_interleaved
bcCode_2_5_industrial
bcCode_2_5_matrix
bcCode39
bcCode39Extended
bcCode128A, bcCode128B, bcCode128C
bcCode93, bcCode93Extended
bcCodeMSI
bcCodePostNet
bcCodeCodabar
bcCodeEAN8, bcCodeEAN13
bcCodeUPC_A, bcCodeUPC_E0, bcCodeUPC_E1
bcCodeUPC_Supp2, bcCodeUPC_Supp5
bcCodeEAN_8_13
bcCodeEAN_128
```

**Key Functions:**
- `DrawBarcode()` - Main rendering function
- `BarCodeNames()` - Get barcode type names
- `BarCodeZooms()` - Get zoom levels
- `BarCodeSize()` - Calculate barcode size

**Parameters:**
- Canvas for drawing
- Barcode type
- Module size
- Rotation angle
- Checksum calculation
- Text display options

---

### 13. sestvyst.pas
**Purpose:** Report output/rendering engine
**Responsibilities:**
- Page composition
- Element rendering
- Print output
- Preview generation
- Multi-pass rendering for aggregates

**Key Features:**
- Page break logic
- Section repetition
- Group processing
- Aggregate calculation
- Print spooling

---

### 14. sestprn.pas / SestPrn.pas
**Purpose:** Print management
**Key Classes:**
- Print job management
- Printer configuration
- Page setup
- Print preview canvas

**Features:**
- Printer enumeration
- Paper size selection
- Orientation control
- Margin management
- Print queue handling

---

### 15. prnbuttn.pas / Prnbuttn.pas
**Purpose:** Print button controls
**Key Classes:**
- `TPrintButton` - Standard print button
- `TPrintButtonComposite` - Composite report print button
- `TComboBoxPredloha` - Template selector
- `TComboBoxPrinter` - Printer selector

**Features:**
- One-click printing
- Template selection UI
- Printer selection UI
- Icon integration

---

### 16. sestreg.pas
**Purpose:** Component registration
**IDE Integration:**

**Registered Components:**
```pascal
TSestava
TSestDetail
TCompositeReportSinea
TComboBoxPredloha
TComboBoxPrinter
TPrintButton
TPrintButtonComposite
TPrintDialogSinea
```

**Property Editors:**
- `TPredlohaProperty` - Template editor
- `TReportAlias` - BDE alias selector
- `TReportDir` - Directory selector
- `TReportName` - Report file selector
- `TBackgroundName` - Background image selector

**Component Editors:**
- `TSestavaEditor` - Report component editor
- `TCompositeEditor` - Composite report editor

---

## Bridge Modules (Data Abstraction)

### 17. DB_Bridge.pas
**Purpose:** BDE/VCL database abstraction
**Key Classes:**
- `TBridge` - Base bridge interface
- `TSIDataSource` - Data source wrapper

**Features:**
- TDataSet abstraction
- Field access
- Navigation methods
- State management

---

### 18. IBO_Bridge.pas
**Purpose:** IBObjects database abstraction
**Features:**
- InterBase/Firebird support
- IB_DataSource integration
- Native IB object mapping

---

### 19. WO_Bridge.pas
**Purpose:** Wide OLE/ADO database abstraction
**Features:**
- OLE DB provider support
- Unicode string handling
- ADO integration

---

## Helper Modules

### 20. sest_lib.pas
**Purpose:** Utility library
**Contains:** Common utility functions and helpers

---

### 21. sest_zn.pas
**Purpose:** Character/font utilities
**Features:** Font management and text measurement

---

### 22. sest_ver.inc
**Purpose:** Compiler version detection and conditional compilation
**Contains:**
- Version-specific defines
- Compatibility switches
- Platform detection

---

### 23. sestavya.pas
**Purpose:** Additional report functionality (advanced features)

---

### 24. sestavyc.pas
**Purpose:** Report component helpers

---

### 25. sestavyu.pas
**Purpose:** Report utility functions

---

## UI Dialog Modules

### 26. sest_fne.pas
**Purpose:** Function editor dialog
**Key Classes:**
- `TFunkceDlg` - Function definition dialog

---

### 27. sest_fnv.pas
**Purpose:** Expression editor dialog
**Key Classes:**
- `TSest_FnEditVyraz` - Expression editor form

---

### 28. sest_kfp.pas
**Purpose:** Element configuration dialog
**Key Classes:**
- `TDlgKonfPrvku` - Element properties dialog

---

### 29. sestp.pas
**Purpose:** Print preview form
**Key Classes:**
- `TFormSestP` - Print preview window

**Features:**
- Zoom control
- Page navigation
- Print from preview
- Page thumbnails

---

### 30. sestepr.pas
**Purpose:** Print configuration dialog
**Key Classes:**
- `TSestKonfigDlg` - Print settings dialog

---

### 31. sestese.pas
**Purpose:** Report selection dialog
**Key Classes:**
- `TSestDlg` - Report selector

---

### 32. sestfrmp.pas
**Purpose:** Paper format dialog
**Key Classes:**
- `TDlgFormatPapiru` - Paper format configuration

---

### 33. sestinf.pas
**Purpose:** Information dialog
**Key Classes:**
- `TFormInfo` - About/info dialog

---

### 34. sestlogo.pas
**Purpose:** Logo/splash screen
**Key Classes:**
- `TFormLogo` - Logo display form

---

### 35. sestzpr.pas
**Purpose:** Zoom preview dialog
**Key Classes:**
- `TFormSineaReportZoom` - Zoom viewer

---

### 36. sestcmpe.pas
**Purpose:** Component editor form
**Key Classes:**
- `TFormSestCmpEd` - Component property editor

---

## Utility Modules

### 37. sestcand.pas
**Purpose:** Canvas drawing utilities (stub file)

---

### 38. sestcanm.pas
**Purpose:** Canvas management
**Key Classes:**
- `TSineaCanvas` - Enhanced canvas wrapper

---

### 39. sestcepr.pas
**Purpose:** Czech printer support
**Features:** Localized printer names and settings

---

### 40. sestspl.pas
**Purpose:** Print spooler interface

---

## Module Dependencies

### Core Dependency Chain:
```
sest_ver.inc (included by all)
    ↓
sest_typ.pas (types and constants)
    ↓
sest_str.pas (strings)
    ↓
sestavyt.pas (template base)
    ↓
sest_fn.pas (functions)
    ↓
sestavy.pas (components)
```

### Database Layer:
```
Bridge interface
    ↓
DB_Bridge / IBO_Bridge / WO_Bridge
    ↓
sestavy.pas (TSestavaD)
```

### Designer:
```
sestavyt.pas
    ↓
sestdb.pas (visual elements)
    ↓
seste.pas (editor form)
```

### Output:
```
sestavy.pas
    ↓
sestvyst.pas (rendering)
    ↓
sestprn.pas (printing)
```

## File Count Summary

- **Pascal Source Files (.pas):** ~40 files
- **Compiled Units (.dcu):** ~40 files
- **Form Files (.dfm):** ~15 files
- **Package Files (.dpk):** 18 files
- **Project Files (.dproj):** 8 files
- **Resource Files (.res):** 15 files
- **Total Lines of Code:** Estimated 25,000+ lines
