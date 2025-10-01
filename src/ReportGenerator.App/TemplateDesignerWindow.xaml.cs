using System;
using System.Collections.Generic;
using System.IO;
using System.Printing;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Hosting;
using ReportGenerator.Application.Services;
using ReportGenerator.Domain.Entities;
using ReportGenerator.Domain.Interfaces;

namespace ReportGenerator.App;

public partial class TemplateDesignerWindow : Window
{
    public TemplateDesignerWindow(TemplateDesignerViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}

public partial class TemplateDesignerViewModel : ObservableObject
{
    private readonly TemplateService _service;
    private readonly IExpressionEvaluator _evaluator;
    private Template _template;

    [ObservableProperty]
    private string bodyText = SampleBody();

    [ObservableProperty]
    private string paramsJson = "{\n  \"CustomerName\": \"John Doe\",\n  \"Total\": 123.45\n}";

    [ObservableProperty]
    private FlowDocument previewDocument = new FlowDocument(new Paragraph(new Run("")));

    public IRelayCommand RefreshPreviewCommand { get; }
    public IAsyncRelayCommand SaveCommand { get; }
    public IRelayCommand PrintCommand { get; }

    public TemplateDesignerViewModel(Template template, TemplateService service, IExpressionEvaluator evaluator)
    {
        _template = template;
        _service = service;
        _evaluator = evaluator;

        if (!string.IsNullOrWhiteSpace(template.Description))
        {
            BodyText = template.Description!;
        }

        RefreshPreviewCommand = new RelayCommand(UpdatePreview);
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        PrintCommand = new RelayCommand(Print);

        UpdatePreview();
    }

    private static string SampleBody() =>
        "Invoice\n\n" +
        "Customer: {{ CustomerName }}\n" +
        "Total: {{ ROUND(Total, 2) }}\n\n" +
        "Thank you for your purchase!";

    private void UpdatePreview()
    {
        Dictionary<string, object?> parameters = new();
        try
        {
            if (!string.IsNullOrWhiteSpace(ParamsJson))
            {
                parameters = JsonSerializer.Deserialize<Dictionary<string, object?>>(ParamsJson) ?? new();
            }
        }
        catch
        {
            // ignore parse errors; use empty params
            parameters = new();
        }

        var rendered = RenderTemplate(BodyText ?? string.Empty, parameters);
        PreviewDocument = ToFlowDocument(rendered);
    }

    private string RenderTemplate(string body, IDictionary<string, object?> parameters)
    {
        // Replace {{ expr }} with evaluated values via NCalc
        return Regex.Replace(body, @"\{\{([^}]+)\}\}", m =>
        {
            var expr = m.Groups[1].Value.Trim();
            try
            {
                var val = _evaluator.Evaluate(expr, parameters);
                return val?.ToString() ?? string.Empty;
            }
            catch
            {
                return m.Value; // leave as-is on error
            }
        });
    }

    private static FlowDocument ToFlowDocument(string text)
    {
        var doc = new FlowDocument();
        foreach (var line in text.Replace("\r\n", "\n").Split('\n'))
        {
            doc.Blocks.Add(new Paragraph(new Run(line)));
        }
        return doc;
    }

    private async System.Threading.Tasks.Task SaveAsync()
    {
        _template.Description = BodyText;
        await _service.UpdateDescriptionAsync(_template.Id, _template.Description);
    }

    private void Print()
    {
        var dlg = new PrintDialog();
        if (dlg.ShowDialog() == true)
        {
            IDocumentPaginatorSource dps = PreviewDocument;
            dlg.PrintDocument(dps.DocumentPaginator, _template.Name);
        }
    }
}

