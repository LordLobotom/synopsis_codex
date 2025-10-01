# SINEA Report Generator - Architecture Diagrams

## System Architecture

### High-Level Component Diagram

```
┌───────────────────────────────────────────────────────────────┐
│                     User Application                          │
│  ┌─────────────┐  ┌─────────────┐  ┌──────────────────────┐ │
│  │  Form with  │  │   Report    │  │  Print/Preview       │ │
│  │  TSestava   │──│   Events    │──│  Controls            │ │
│  └─────────────┘  └─────────────┘  └──────────────────────┘ │
└───────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌───────────────────────────────────────────────────────────────┐
│              SINEA Report Generator Library                    │
│                                                               │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │           Component Layer (sestavy.pas)                  │ │
│  │  ┏━━━━━━━━━━┓  ┏━━━━━━━━━━━━┓  ┏━━━━━━━━━━━━━━━━━━━┓ │ │
│  │  ┃ TSestava ┃  ┃ TSestDetail ┃  ┃ TComposite          ┃ │ │
│  │  ┗━━━━━━━━━━┛  ┗━━━━━━━━━━━━┛  ┃ ReportSinea         ┃ │ │
│  │  ┏━━━━━━━━━━┓  ┏━━━━━━━━━━━━┓  ┗━━━━━━━━━━━━━━━━━━━┛ │ │
│  │  ┃ TSestavaD┃  ┃ TSestavaTyp ┃                          │ │
│  │  ┗━━━━━━━━━━┛  ┗━━━━━━━━━━━━┛                          │ │
│  └─────────────────────────────────────────────────────────┘ │
│                              │                                │
│  ┌───────────────────────────┼──────────────────────────────┐│
│  │                           │                               ││
│  │  ┌────────────────────┐   │   ┌──────────────────────┐  ││
│  │  │  Template Engine   │◄──┴──►│  Expression Engine   │  ││
│  │  │  (sestavyt.pas)    │       │  (sest_fn.pas)       │  ││
│  │  │                    │       │                      │  ││
│  │  │ - TPredloha        │       │ - Parser            │  ││
│  │  │ - TOddilSestavy    │       │ - Calculator        │  ││
│  │  │ - TSiReportElement │       │ - 43 Functions      │  ││
│  │  └────────────────────┘       └──────────────────────┘  ││
│  │                                                           ││
│  └───────────────────────────────────────────────────────────┘│
│                              │                                │
│  ┌───────────────────────────┼──────────────────────────────┐│
│  │                           │                               ││
│  │  ┌────────────────────┐   │   ┌──────────────────────┐  ││
│  │  │  Rendering Engine  │   │   │  Barcode Engine      │  ││
│  │  │  (sestvyst.pas)    │   │   │  (sestbarc.pas)      │  ││
│  │  │                    │   │   │                      │  ││
│  │  │ - Page composition │   │   │ - 22 barcode types  │  ││
│  │  │ - Element drawing  │   │   │ - Checksum calc     │  ││
│  │  │ - Print output     │   │   │ - Size calculation  │  ││
│  │  └────────────────────┘   │   └──────────────────────┘  ││
│  │                           │                               ││
│  └───────────────────────────┼──────────────────────────────┘│
│                              │                                │
│  ┌───────────────────────────┴──────────────────────────────┐│
│  │             Database Abstraction (Bridge Pattern)         ││
│  │                                                            ││
│  │  ┌──────────────┐  ┌──────────────┐  ┌───────────────┐  ││
│  │  │  DB_Bridge   │  │  IBO_Bridge  │  │  WO_Bridge    │  ││
│  │  │   (BDE)      │  │  (IBObjects) │  │  (OLE DB)     │  ││
│  │  └──────────────┘  └──────────────┘  └───────────────┘  ││
│  └───────────────────────────────────────────────────────────┘│
└───────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌───────────────────────────────────────────────────────────────┐
│                    Platform Services                           │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐       │
│  │  Windows GDI │  │   Printers   │  │     VCL      │       │
│  └──────────────┘  └──────────────┘  └──────────────┘       │
└───────────────────────────────────────────────────────────────┘
```

---

## Class Hierarchy Diagram

```
TComponent (VCL)
    │
    └─── TSestavaTyp (sestavyt.pas)
            │
            ├─── Properties:
            │    - Predloha: TPredloha
            │    - Sections: TOddilSestavy
            │    - Elements: TSiReportElement
            │
            └─── TSestavaD (sestavy.pas)
                    │
                    ├─── Properties:
                    │    - Bridge: TBridge
                    │    - DataSource: TSIDataSource
                    │
                    ├─── Events:
                    │    - OnGetData
                    │    - OnStepData
                    │    - OnPaintBox
                    │
                    ├─── TSestava (sestavy.pas)
                    │    │
                    │    ├─── Properties:
                    │    │    - ReportName: String
                    │    │    - ReportDir: String
                    │    │    - ReportAlias: String
                    │    │
                    │    ├─── Events:
                    │    │    - BeforePreview
                    │    │    - AfterPreview
                    │    │    - OnVypoctiFunkce
                    │    │
                    │    └─── Methods:
                    │         - Print
                    │         - Preview
                    │         - Edit
                    │
                    └─── TSestDetail (sestavy.pas)
                         │
                         ├─── Optimized for sub-reports
                         │
                         └─── Methods:
                              - Same as TSestava


TComponent (VCL)
    │
    └─── TCompositeReportSinea (sestavy.pas)
            │
            ├─── Properties:
            │    - Reports: TList<TSestava>
            │
            └─── Methods:
                 - Print (all reports)
                 - Preview (all reports)
```

---

## Template Structure Diagram

```
TPredloha (Report Template)
    │
    ├─── Properties
    │    ├─── Version: Integer
    │    ├─── PaperSize: TPaperSize
    │    ├─── Orientation: TOrientation
    │    ├─── Margins: TMargins
    │    └─── Background: TGraphic
    │
    └─── Sections (Array of TOddilSestavy)
            │
            ├─── [0] Report Header (odZacatku)
            │    └─── Elements: List<TSiReportElement>
            │
            ├─── [1] Page Header (odHlavy)
            │    └─── Elements: List<TSiReportElement>
            │
            ├─── [2] Detail/Body (odTela)
            │    └─── Elements: List<TSiReportElement>
            │
            ├─── [3] Report Footer (odKonce)
            │    └─── Elements: List<TSiReportElement>
            │
            ├─── [4] Page Footer (odPaty)
            │    └─── Elements: List<TSiReportElement>
            │
            └─── [5..N] Group Headers/Footers (odSkupiny)
                 └─── Elements: List<TSiReportElement>


TSiReportElement (Base Element)
    │
    ├─── Properties:
    │    ├─── Left, Top, Width, Height
    │    ├─── Font, Color
    │    ├─── Visible, Enabled
    │    └─── ElementType
    │
    └─── Types:
         ├─── Label (Static text)
         ├─── DBText (Database field)
         ├─── DBLabel (Database label)
         ├─── Function (Calculated)
         ├─── Parameter (User input)
         ├─── Variable (Runtime value)
         ├─── Line (Graphic)
         ├─── Rectangle (Graphic)
         ├─── RoundRect (Graphic)
         ├─── Ellipse (Graphic)
         ├─── Image (Picture)
         ├─── Barcode (Barcode)
         ├─── PaintBox (Custom)
         └─── Detail (Sub-report)
```

---

## Data Flow Diagram

### Report Execution Flow

```
┌──────────────┐
│ User calls   │
│ Print() or   │
│ Preview()    │
└──────┬───────┘
       │
       ▼
┌──────────────────────────────┐
│ Initialize Report            │
│ - Load template              │
│ - Prepare data source        │
│ - Initialize variables       │
└──────┬───────────────────────┘
       │
       ▼
┌──────────────────────────────┐
│ Fire BeforePrint/Preview     │
└──────┬───────────────────────┘
       │
       ▼
┌──────────────────────────────┐
│ First Pass (if aggregates)   │
│ - Iterate all records        │
│ - Calculate SUM, COUNT, etc. │
└──────┬───────────────────────┘
       │
       ▼
┌──────────────────────────────┐
│ Fire OnVypoctiFunkce         │
│ - User calculations          │
└──────┬───────────────────────┘
       │
       ▼
┌──────────────────────────────┐
│ Rendering Pass               │
│ ┌──────────────────────────┐ │
│ │ Print Report Header      │ │
│ └──────────────────────────┘ │
│       │                       │
│       ▼                       │
│ ┌──────────────────────────┐ │
│ │ FOR EACH PAGE:           │ │
│ │ ┌──────────────────────┐ │ │
│ │ │ Print Page Header    │ │ │
│ │ └──────────────────────┘ │ │
│ │       │                  │ │
│ │       ▼                  │ │
│ │ ┌──────────────────────┐ │ │
│ │ │ FOR EACH RECORD:     │ │ │
│ │ │ ┌──────────────────┐ │ │ │
│ │ │ │ Fire OnGetData   │ │ │ │
│ │ │ └──────────────────┘ │ │ │
│ │ │       │              │ │ │
│ │ │       ▼              │ │ │
│ │ │ ┌──────────────────┐ │ │ │
│ │ │ │ Print Group      │ │ │ │
│ │ │ │ Header (if new)  │ │ │ │
│ │ │ └──────────────────┘ │ │ │
│ │ │       │              │ │ │
│ │ │       ▼              │ │ │
│ │ │ ┌──────────────────┐ │ │ │
│ │ │ │ Fire             │ │ │ │
│ │ │ │ OnTiskOddilu     │ │ │ │
│ │ │ └──────────────────┘ │ │ │
│ │ │       │              │ │ │
│ │ │       ▼              │ │ │
│ │ │ ┌──────────────────┐ │ │ │
│ │ │ │ Render Detail    │ │ │ │
│ │ │ │ Elements         │ │ │ │
│ │ │ └──────────────────┘ │ │ │
│ │ │       │              │ │ │
│ │ │       ▼              │ │ │
│ │ │ ┌──────────────────┐ │ │ │
│ │ │ │ Fire OnStepData  │ │ │ │
│ │ │ │ (Next)           │ │ │ │
│ │ │ └──────────────────┘ │ │ │
│ │ │       │              │ │ │
│ │ │       └─── Loop ─────┘ │ │
│ │ └──────────────────────┘ │ │
│ │       │                  │ │
│ │       ▼                  │ │
│ │ ┌──────────────────────┐ │ │
│ │ │ Print Group Footer   │ │ │
│ │ └──────────────────────┘ │ │
│ │       │                  │ │
│ │       ▼                  │ │
│ │ ┌──────────────────────┐ │ │
│ │ │ Print Page Footer    │ │ │
│ │ └──────────────────────┘ │ │
│ │       │                  │ │
│ │       └─── If more ──────┘ │
│ │            records         │
│ └────────────────────────────┘
│       │                       │
│       ▼                       │
│ ┌──────────────────────────┐ │
│ │ Print Report Footer      │ │
│ └──────────────────────────┘ │
└──────┬───────────────────────┘
       │
       ▼
┌──────────────────────────────┐
│ Fire AfterPrint/Preview      │
└──────┬───────────────────────┘
       │
       ▼
┌──────────────────────────────┐
│ Output to Printer or Preview │
└──────────────────────────────┘
```

---

## Bridge Pattern (Database Abstraction)

```
┌────────────────────────────────────────────────┐
│              TSestava Component                 │
│                                                │
│  Calls methods via Bridge interface:          │
│  - Bridge.First                               │
│  - Bridge.Next                                │
│  - Bridge.FieldValue(i)                       │
└────────────────┬───────────────────────────────┘
                 │
                 ▼
     ┌──────────────────────┐
     │   TBridge Interface  │  (Abstract)
     │                      │
     │  - Open / Close      │
     │  - First / Next      │
     │  - EOF / BOF         │
     │  - FieldCount        │
     │  - FieldValue()      │
     └──────────┬───────────┘
                │
        ┌───────┴───────────────────┐
        │                           │
        ▼                           ▼
┌───────────────┐           ┌───────────────┐
│  TBDEBridge   │           │  TIBOBridge   │
│               │           │               │
│ Implements:   │           │ Implements:   │
│ - TDataSet    │           │ - TIB_Dataset │
│ - TTable      │           │ - TIB_Query   │
│ - TQuery      │           │               │
└───────────────┘           └───────────────┘
        │                           │
        ▼                           ▼
┌───────────────┐           ┌───────────────┐
│  BDE Engine   │           │  IBObjects    │
│               │           │               │
│ - Paradox     │           │ - InterBase   │
│ - dBase       │           │ - Firebird    │
│ - Access      │           │               │
└───────────────┘           └───────────────┘
```

```
        ┌──────────────────┐
        │  TBridge         │
        └────────┬─────────┘
                 │
                 ▼
        ┌──────────────────┐
        │  TWOBridge       │
        │                  │
        │ Implements:      │
        │ - TADODataSet    │
        │ - TADOQuery      │
        └────────┬─────────┘
                 │
                 ▼
        ┌──────────────────┐
        │  ADO / OLE DB    │
        │                  │
        │ - SQL Server     │
        │ - Oracle         │
        │ - MySQL          │
        └──────────────────┘
```

**Benefits:**
- Report code independent of database engine
- Easy to add new database support
- Single API for all data sources

---

## Expression Evaluation Flow

```
┌──────────────────────────┐
│ Expression in Template   │
│ "[Quantity] * [Price]"   │
└──────────┬───────────────┘
           │
           ▼
┌──────────────────────────┐
│ Tokenizer (sest_fn.pas)  │
│ ┌──────────────────────┐ │
│ │ [Quantity]  Token 1  │ │
│ │ *           Token 2  │ │
│ │ [Price]     Token 3  │ │
│ └──────────────────────┘ │
└──────────┬───────────────┘
           │
           ▼
┌──────────────────────────┐
│ Parser                   │
│ ┌──────────────────────┐ │
│ │ Binary Operation     │ │
│ │   Op: Multiply       │ │
│ │   Left: Field(Qty)   │ │
│ │   Right: Field(Prc)  │ │
│ └──────────────────────┘ │
└──────────┬───────────────┘
           │
           ▼
┌──────────────────────────┐
│ Evaluator                │
│ ┌──────────────────────┐ │
│ │ Resolve [Quantity]:  │ │
│ │   Call CalcHandler   │ │
│ │   Get field value    │ │
│ │   = 5                │ │
│ │                      │ │
│ │ Resolve [Price]:     │ │
│ │   Call CalcHandler   │ │
│ │   Get field value    │ │
│ │   = 10.50            │ │
│ │                      │ │
│ │ Calculate: 5 * 10.50 │ │
│ │ Result = 52.50       │ │
│ └──────────────────────┘ │
└──────────┬───────────────┘
           │
           ▼
┌──────────────────────────┐
│ Format Result            │
│ Apply format mask        │
│ "$52.50"                 │
└──────────┬───────────────┘
           │
           ▼
┌──────────────────────────┐
│ Render to Canvas         │
└──────────────────────────┘
```

---

## Designer Architecture

```
┌─────────────────────────────────────────────────────────┐
│           TFormSestE (seste.pas)                        │
│                Main Designer Form                        │
│                                                         │
│  ┌────────────────────────────────────────────────┐   │
│  │  Toolbar                                        │   │
│  │  [Select] [Label] [Field] [Line] [Rect] [...]  │   │
│  └────────────────────────────────────────────────┘   │
│                                                         │
│  ┌────────────────────────────────────────────────┐   │
│  │  Ruler (Horizontal)                            │   │
│  └────────────────────────────────────────────────┘   │
│                                                         │
│  ┌──┬─────────────────────────────────────────────┐   │
│  │R │  TPapir (sestdb.pas)                        │   │
│  │u │  Report Canvas                              │   │
│  │l │  ┌────────────────────────────────────────┐ │   │
│  │e │  │ Section: Report Header                 │ │   │
│  │r │  │ ┌────────┐  ┌────────┐                │ │   │
│  │  │  │ │Element │  │Element │                │ │   │
│  │V │  │ └────────┘  └────────┘                │ │   │
│  │  │  └────────────────────────────────────────┘ │   │
│  │  │  ┌────────────────────────────────────────┐ │   │
│  │  │  │ Section: Detail                        │ │   │
│  │  │  │ ┌────────┐  ┌────────┐  ┌──────────┐ │ │   │
│  │  │  │ │TSesPrvek TSesPrvek│  │TSesPrvek │ │ │   │
│  │  │  │ │Element │  │Element │  │Element   │ │ │   │
│  │  │  │ └────────┘  └────────┘  └──────────┘ │ │   │
│  │  │  └────────────────────────────────────────┘ │   │
│  │  │  ┌────────────────────────────────────────┐ │   │
│  │  │  │ Section: Page Footer                   │ │   │
│  │  │  │ ┌────────┐                             │ │   │
│  │  │  │ │Element │                             │ │   │
│  │  │  │ └────────┘                             │ │   │
│  │  │  └────────────────────────────────────────┘ │   │
│  └──┴─────────────────────────────────────────────┘   │
│                                                         │
│  ┌────────────────────────────────────────────────┐   │
│  │  Status Bar                                     │   │
│  │  Position: (120, 45)  Size: (200, 20)          │   │
│  └────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
       │
       │ Manages
       ▼
┌─────────────────────────────────────────────────────────┐
│           TOddil (sestdb.pas)                           │
│           Visual Section Container                       │
│                                                         │
│  Properties:                                            │
│  - Index: Integer (section type)                       │
│  - Height: Integer                                     │
│  - Elements: List<TSesPrvek>                           │
│                                                         │
│  Methods:                                              │
│  - Paint                                               │
│  - AddElement                                          │
│  - DeleteElement                                       │
└─────────────────────────────────────────────────────────┘
       │
       │ Contains
       ▼
┌─────────────────────────────────────────────────────────┐
│           TSesPrvek (sestdb.pas)                        │
│           Visual Report Element                          │
│                                                         │
│  Properties:                                            │
│  - Ps: TSiReportElement (data model)                   │
│  - Selected: Boolean                                   │
│  - Handles: Array[0..7] of TRect                       │
│                                                         │
│  Methods:                                              │
│  - Paint (draws element on canvas)                     │
│  - OnMouseDown (selection, drag start)                 │
│  - OnMouseMove (dragging, resizing)                    │
│  - OnMouseUp (drag end)                                │
│  - OnDblClick (open properties)                        │
│                                                         │
│  Rendering Methods:                                    │
│  - DelejDrawDetail                                     │
│  - DelejDrawTextAngle                                  │
│  - DelejDrawMemo                                       │
│  - DelejDrawCara (line)                                │
│  - DelejDrawRam (rectangle)                            │
│  - DelejDrawGraphic                                    │
│  - DelejDrawBarCode                                    │
└─────────────────────────────────────────────────────────┘
```

---

## Undo/Redo System

```
┌─────────────────────────────────────────┐
│    User Action                          │
│    (Move, Resize, Add, Delete)          │
└────────────┬────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────┐
│    Fire cm_UlozUndo Command             │
└────────────┬────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────┐
│    TUndoRecord                          │
│    ┌─────────────────────────────────┐ │
│    │ Action: TUndoAction             │ │
│    │   (zzVlozPrv, zzPosunPrv, etc) │ │
│    │ ElementId: Integer              │ │
│    │ OldState: TElementState         │ │
│    │ NewState: TElementState         │ │
│    └─────────────────────────────────┘ │
└────────────┬────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────┐
│    Push to Undo Stack                   │
│    Stack: List<TUndoRecord>             │
└─────────────────────────────────────────┘


Undo Action:
┌─────────────────────────────────────────┐
│    User presses Ctrl+Z                  │
└────────────┬────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────┐
│    Pop from Undo Stack                  │
└────────────┬────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────┐
│    Restore OldState                     │
│    Push to Redo Stack                   │
└─────────────────────────────────────────┘
```

---

## File I/O Architecture

```
Template File (.PTS)
┌─────────────────────────────────────┐
│  Header                             │
│  - Magic Number                     │
│  - Version                          │
│  - Timestamp                        │
├─────────────────────────────────────┤
│  Paper Setup                        │
│  - Paper size                       │
│  - Orientation                      │
│  - Margins                          │
├─────────────────────────────────────┤
│  Section Count                      │
├─────────────────────────────────────┤
│  FOR EACH Section:                  │
│  ├─ Section Type                    │
│  ├─ Section Height                  │
│  ├─ Element Count                   │
│  │                                  │
│  └─ FOR EACH Element:               │
│     ├─ Element Type                 │
│     ├─ Position (L,T,W,H)          │
│     ├─ Font Properties             │
│     ├─ Colors                      │
│     ├─ Text/Expression             │
│     └─ Type-specific data          │
├─────────────────────────────────────┤
│  Function Definitions               │
│  - Count                            │
│  - Name, Expression, Type           │
├─────────────────────────────────────┤
│  Variables                          │
│  - Count                            │
│  - Name, Type, Initial Value        │
└─────────────────────────────────────┘
```

This comprehensive architecture documentation provides visual representations of how the SINEA Report Generator components interact and are structured internally.
