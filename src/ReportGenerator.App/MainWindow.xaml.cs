using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportGenerator.Application.Services;
using ReportGenerator.Domain.Entities;
using ReportGenerator.Domain.Interfaces;

namespace ReportGenerator.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel(App.HostInstance!);
    }
}

public partial class MainViewModel : ObservableObject
{
    private readonly TemplateService _service;
    private readonly IExpressionEvaluator _evaluator;

    public ObservableCollection<Template> Templates { get; } = new();

    [ObservableProperty]
    private Template? selectedTemplate;

    [ObservableProperty]
    private string newTemplateName = string.Empty;

    [ObservableProperty]
    private string bodyText = "Invoice\n\nCustomer: {{ CustomerName }}\nTotal: {{ ROUND(Total, 2) }}";

    [ObservableProperty]
    private string paramsJson = "{\n  \"CustomerName\": \"John Doe\",\n  \"Total\": 123.45\n}";

    [ObservableProperty]
    private FlowDocument previewDocument = new FlowDocument(new Paragraph(new Run("")));

    public IAsyncRelayCommand AddTemplateCommand { get; }
    public IRelayCommand RefreshCommand { get; }
    public IAsyncRelayCommand SaveCommand { get; }
    public IRelayCommand PrintCommand { get; }

    public MainViewModel(Microsoft.Extensions.Hosting.IHost host)
    {
        _service = host.Services.GetService(typeof(TemplateService)) as TemplateService
                   ?? throw new System.InvalidOperationException("TemplateService not registered");
        _evaluator = host.Services.GetService(typeof(IExpressionEvaluator)) as IExpressionEvaluator
                     ?? throw new System.InvalidOperationException("IExpressionEvaluator not registered");

        AddTemplateCommand = new AsyncRelayCommand(AddTemplateAsync);
        RefreshCommand = new RelayCommand(RefreshPreview);
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        PrintCommand = new RelayCommand(Print);

        _ = LoadAsync();
        RefreshPreview();
    }

    partial void OnSelectedTemplateChanged(Template? value)
    {
        if (value == null) return;
        BodyText = value.Description ?? BodyText;
        RefreshPreview();
    }

    private async Task LoadAsync()
    {
        Templates.Clear();
        var list = await _service.ListAsync(CancellationToken.None);
        foreach (var t in list) Templates.Add(t);
    }

    private async Task AddTemplateAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTemplateName)) return;
        var created = await _service.CreateAsync(NewTemplateName, null, CancellationToken.None);
        Templates.Add(created);
        SelectedTemplate = created;
        NewTemplateName = string.Empty;
    }

    private void RefreshPreview()
    {
        var parameters = new Dictionary<string, object?>();
        try
        {
            if (!string.IsNullOrWhiteSpace(ParamsJson))
                parameters = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object?>>(ParamsJson) ?? new();
        }
        catch { parameters = new(); }

        string Render(string text)
            => Regex.Replace(text ?? string.Empty, "\\{\\{([^}]+)\\}\\}", m =>
            {
                var expr = m.Groups[1].Value.Trim();
                try { return _evaluator.Evaluate(expr, parameters)?.ToString() ?? string.Empty; }
                catch { return m.Value; }
            });

        var rendered = Render(BodyText);
        var doc = new FlowDocument();
        foreach (var line in rendered.Replace("\r\n", "\n").Split('\n'))
            doc.Blocks.Add(new Paragraph(new Run(line)));
        PreviewDocument = doc;
    }

    private async Task SaveAsync()
    {
        if (SelectedTemplate == null) return;
        SelectedTemplate.Description = BodyText;
        await _service.UpdateDescriptionAsync(SelectedTemplate.Id, BodyText, CancellationToken.None);
    }

    private void Print()
    {
        var dlg = new System.Windows.Controls.PrintDialog();
        if (dlg.ShowDialog() == true)
        {
            if (System.Windows.Application.Current.MainWindow is MainWindow mw)
            {
                var viewer = (System.Windows.Controls.FlowDocumentScrollViewer)mw.FindName("PreviewViewer")!;
                var paginator = ((IDocumentPaginatorSource)viewer.Document).DocumentPaginator;
                dlg.PrintDocument(paginator, SelectedTemplate?.Name ?? "Report");
            }
        }
    }
}
