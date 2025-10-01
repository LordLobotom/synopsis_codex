# SINEA Report Generator - Documentation Synopsis

This directory contains comprehensive technical documentation for the SINEA Report Generator version 9.50.

## Documentation Files

### 01_Technical_Overview.md
Complete overview of the project including:
- Project identification and history
- Technology stack and supported platforms
- System architecture
- Core functionality
- Component catalog
- Build configuration
- Version history
- Licensing information

### 02_Module_Reference.md
Detailed reference of all source code modules:
- 40+ module descriptions
- Module responsibilities
- Key classes and functions
- Dependencies and relationships
- File organization

### 03_Component_API_Reference.md
Programming interface documentation:
- Component hierarchy
- Complete property reference
- Event documentation with signatures
- Method descriptions
- Type definitions
- Constants reference
- Usage examples

### 04_Usage_Guide.md
Practical guide for developers:
- Installation instructions
- Quick start tutorial
- 10 common usage scenarios
- Report designer guide
- Expression syntax
- Troubleshooting tips
- Best practices

### 05_Architecture_Diagrams.md
Visual architecture documentation:
- System architecture diagram
- Class hierarchy diagrams
- Component interaction flows
- Data flow diagrams
- Bridge pattern illustration
- Designer architecture
- File format structure

## Quick Reference

### What is SINEA Report Generator?

A VCL component library for Borland/Embarcadero Delphi that provides:
- Visual report designer
- Database-bound reporting
- Expression evaluator
- Multiple output formats (Print, Preview, RTF)
- Barcode support (22+ types)
- Master-detail reports
- Grouping and aggregates

### Supported Delphi Versions
- Delphi 4, 5, 6, 7
- Delphi 2005, 2006, 2007
- Delphi 2009, 2010
- Delphi XE, XE2, XE3

### Main Components
- `TSestava` - Main report component
- `TSestDetail` - Detail/sub-report
- `TCompositeReportSinea` - Multi-report manager
- Print buttons and dialogs

### Database Support
- BDE (Borland Database Engine)
- IBObjects (InterBase/Firebird)
- OLE DB / ADO

## For Developers

### Getting Started
1. Read `01_Technical_Overview.md` for project understanding
2. Review `03_Component_API_Reference.md` for API details
3. Follow examples in `04_Usage_Guide.md`
4. Reference `02_Module_Reference.md` when working with source

### Common Tasks

**Creating a simple report:**
```pascal
Report1.DataSource := DataSource1;
Report1.ReportName := 'myreport.pts';
Report1.Preview;
```

**Designing a template:**
```pascal
// Double-click component at design-time
// Or at runtime:
Report1.Edit;
```

**Custom data source:**
```pascal
Report1.OnGetData := GetDataHandler;
Report1.OnStepData := StepDataHandler;
```

## Architecture Highlights

### Layer Structure
```
Application Layer
    ↓
Component Interface (TSestava, TSestDetail)
    ↓
Core Engine (Template, Expression, Rendering)
    ↓
Data Abstraction (Bridge Pattern)
    ↓
Platform Services (GDI, Printers)
```

### Key Features
- **43 Built-in Functions** - Math, string, date/time, logic
- **22 Barcode Types** - Code39, Code128, EAN, UPC, etc.
- **Multi-pass Rendering** - Supports aggregates
- **RTF Export** - Rich text format output
- **Custom Drawing** - PaintBox elements
- **Undo/Redo** - In visual designer

## Project Statistics

- **Source Files:** ~40 Pascal units
- **Total Lines:** ~25,000+ LOC
- **Components:** 8 public components
- **Supported Platforms:** Windows 95/98/ME/NT/2000/XP/Vista/7/8
- **Development Period:** 1995-2013

## Copyright & License

```
Copyright (c) 1995-2004 SINEA software
Commercial/Proprietary License
```

## Additional Information

**Barcode Module Author:**
- Jan Tungli
- Email: jan.tungli@seznam.cz

**Primary Language:** Czech (localized strings and documentation)

**Help File:** sestavy.hlp (Czech language)

---

## Document Updates

This documentation was generated on: 2025-10-01

For the most current information, refer to source code comments and the original help file (if available).

## Navigation Tips

- Start with `01_Technical_Overview.md` for high-level understanding
- Use `02_Module_Reference.md` as a code exploration guide
- Reference `03_Component_API_Reference.md` during development
- Follow `04_Usage_Guide.md` for practical implementation
- Consult `05_Architecture_Diagrams.md` for visual understanding

Each document is cross-referenced and can be read independently or as a complete set.
