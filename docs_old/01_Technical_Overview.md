# SINEA Report Generator - Technical Overview

## Project Identification

**Project Name:** SINEA Report Generator
**Version:** 9.50 (Build 62.3)
**Release Date:** May 3, 2013
**Copyright:** 1995-2004 SINEA software
**License:** Commercial/Proprietary

## Executive Summary

SINEA Report Generator is a comprehensive VCL component library for Borland/Embarcadero Delphi that provides visual report design, generation, and printing capabilities. It allows developers to integrate professional reporting functionality into Delphi applications through a suite of design-time and runtime components.

## Technology Stack

### Programming Language
- **Object Pascal (Delphi)**
- Native Windows API integration
- VCL (Visual Component Library) framework

### Supported Delphi Versions
- Delphi 4 (VER120)
- Delphi 5 (VER130)
- Delphi 6 (VER140)
- Delphi 7 (VER150)
- Delphi 2005 (VER170)
- Delphi 2006 (VER180)
- Delphi 2007 (VER190)
- Delphi 2009 (VER200)
- Delphi 2010 (VER210)
- Delphi XE (VER220)
- Delphi XE2 (VER230)
- Delphi XE3 (VER240)

### Database Support
- **BDE (Borland Database Engine)** - Primary support via DB_Bridge
- **IBObjects** - InterBase/Firebird support via IBO_Bridge
- **Wide OLE** - OLE DB support via WO_Bridge
- Generic TDataSource integration

### File Formats
- `.PTS` - Report template files
- `.RES` - Resource files
- `.DCR` - Component resources (icons)
- `.DPK` - Delphi package files
- `.DPROJ` - Project files (Delphi 2007+)

## Architecture Overview

### Layer Architecture

```
┌─────────────────────────────────────────────────┐
│         Application Layer                        │
│    (User's Delphi Application)                  │
└─────────────────────────────────────────────────┘
                      ↓
┌─────────────────────────────────────────────────┐
│    Component Interface Layer                     │
│  TSestava, TSestDetail, TCompositeReportSinea   │
└─────────────────────────────────────────────────┘
                      ↓
┌─────────────────────────────────────────────────┐
│         Core Engine Layer                        │
│  Report Processing, Rendering, Functions        │
└─────────────────────────────────────────────────┘
                      ↓
┌─────────────────────────────────────────────────┐
│      Data Abstraction Layer                      │
│     Bridge Pattern (DB/IBO/WO)                  │
└─────────────────────────────────────────────────┘
                      ↓
┌─────────────────────────────────────────────────┐
│          Platform Layer                          │
│    Windows GDI, Printers, VCL                   │
└─────────────────────────────────────────────────┘
```

### Design Patterns

1. **Bridge Pattern** - Database abstraction (`DB_Bridge`, `IBO_Bridge`, `WO_Bridge`)
2. **Composite Pattern** - Report composition (`TCompositeReportSinea`)
3. **Template Method** - Report generation pipeline
4. **Observer Pattern** - Event-driven callbacks
5. **Component Pattern** - VCL component architecture

## Core Functionality

### Primary Features

1. **Visual Report Designer**
   - WYSIWYG editor for report templates
   - Drag-and-drop element placement
   - Grid alignment and snap-to-grid
   - Undo/Redo functionality

2. **Report Elements**
   - Static Labels
   - Database-bound Text Fields
   - Lines and Shapes (Rectangle, Rounded Rectangle, Ellipse)
   - Images/Graphics
   - Barcodes (multiple formats)
   - Custom PaintBox elements
   - Sub-reports (Details)

3. **Report Sections**
   - Report Header (Začátek)
   - Page Header (Hlava)
   - Body/Detail (Tělo)
   - Page Footer (Pata)
   - Report Footer (Konec)
   - Group Headers/Footers (Skupiny)

4. **Data Binding**
   - Direct TDataSource binding
   - Custom data event handlers
   - Database field mapping
   - Expression evaluator

5. **Expression Engine**
   - 43+ built-in functions
   - Mathematical operations
   - String manipulation
   - Date/Time functions
   - Conditional logic (IIF)
   - Aggregate functions (Sum, Count, Avg, Min, Max)

6. **Output Formats**
   - Direct printer output
   - Print preview
   - RTF export
   - Custom canvas rendering

7. **Barcode Support**
   - Code 2/5 (Interleaved, Industrial, Matrix)
   - Code 39/39 Extended
   - Code 128 (A, B, C)
   - Code 93/93 Extended
   - MSI, PostNet, Codabar
   - EAN-8, EAN-13, EAN-128
   - UPC-A, UPC-E

## Key Components

### Public Components (Registered in IDE)

| Component | Purpose |
|-----------|---------|
| `TSestava` | Main report component with template support |
| `TSestDetail` | Detail/sub-report component |
| `TCompositeReportSinea` | Composite report manager (multiple reports) |
| `TComboBoxPredloha` | Template selector combo box |
| `TComboBoxPrinter` | Printer selector combo box |
| `TPrintButton` | Print button control |
| `TPrintButtonComposite` | Print button for composite reports |
| `TPrintDialogSinea` | Custom print dialog |

### Component Palette
All components register under the **"Sinea"** tab in the Delphi IDE.

## Build Configuration

### Compiler Defines

- `VCLDB` - Enable VCL database support (default)
- `WIDEOLE` - Enable OLE DB support
- `IBOBJECTS` - Enable IBObjects support
- `SI_NON_BDE` - Disable BDE support
- `DELPHI3_` through `DELPHI12_` - Version-specific features

### Package Structure

**Runtime Packages:**
- `sessinea*.dpk` - Runtime package (varies by Delphi version)
- Contains all core functionality
- Requires: `vcl`, `vclx`, `vcldb`, `bdertl`, `rtl`

**Design-time Packages:**
- `sessinea*d.dpk` - Design-time package
- Component editors and property editors
- Registers components in IDE

## Version History

**Version 9.50** (Build 62.3) - May 3, 2013
- Last known release in repository
- Support for Delphi XE3
- Library version tracking via `VerzeKnihovny` constant

### Version Keys
Each Delphi version has a unique key for licensing:
- Delphi XE/XE2/XE3: `5749521458783659`
- Delphi 7: `6458735891453798`
- Delphi 6: `5536478952416492`
- Delphi 5: `6547923495124793`
- Delphi 4: `8542396574851293`

## System Requirements

### Development Environment
- Windows OS (NT/2000/XP/Vista/7/8)
- Delphi IDE (versions 4 through XE3)
- Minimum 32MB RAM (Delphi 4) to 1GB (XE3)
- VGA or higher resolution display

### Runtime Requirements
- Windows 95/98/ME/NT/2000/XP/Vista/7/8
- Printer drivers for printing functionality
- BDE or alternative database engine (if using data-bound reports)

## Licensing Model

The component appears to use a key-based licensing system with version-specific keys embedded in the compiled code. The keys are defined in `sest_typ.pas` as constants.

## Localization

The software appears to be Czech-localized based on:
- Comments in Czech language
- Resource strings in Czech (`sest_str.pas`)
- Help file reference: `sestavy.hlp` (Czech)

Resource string sections:
- `EditorSestav` - Report Editor
- `Oddily` - Sections

## Author Information

**Barcode Module Author:**
- Jan Tungli
- Email: jan.tungli@seznam.cz, tungli@datapac.sk
- Website: http://free.netlap.hu/4321/

## Distribution

The repository structure suggests this is a complete SDK for integrating the report generator into client applications:
- Source code included (`.pas` files)
- Compiled units provided (`.dcu` files)
- Resources and icons (`.res`, `.dcr`)
- Package files for multiple Delphi versions
- Group projects for easy compilation
