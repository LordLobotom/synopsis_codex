using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportGenerator.Application.Services;
using ReportGenerator.Domain.Entities;
using ReportGenerator.Domain.Interfaces;

namespace ReportGenerator.App;

public partial class MainWindow : Window
{
    private bool _dragging;
    private Point _dragStart;
    private Models.DesignElement? _dragElement;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel(App.HostInstance!, DesignCanvas);
    }

    private void Element_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is Border border && border.DataContext is Models.DesignElement el)
        {
            _dragging = true;
            _dragStart = e.GetPosition(DesignCanvas);
            _dragElement = el;
            border.CaptureMouse();
        }
    }

    private void Element_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (_dragging && _dragElement != null)
        {
            var pos = e.GetPosition(DesignCanvas);
            var dx = pos.X - _dragStart.X;
            var dy = pos.Y - _dragStart.Y;
            _dragStart = pos;
            _dragElement.X += dx;
            _dragElement.Y += dy;
        }
    }

    private void Element_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (_dragging)
        {
            _dragging = false;
            (sender as Border)?.ReleaseMouseCapture();
            _dragElement = null;
        }
    }
}

public partial class MainViewModel : ObservableObject
{
    private readonly TemplateService _service;
    private readonly IExpressionEvaluator _evaluator;
    private readonly Canvas _canvas;

    public ObservableCollection<Template> Templates { get; } = new();

    [ObservableProperty]
    private string newTemplateName = string.Empty;

    [ObservableProperty]
    private Template? selectedTemplate;

    public ObservableCollection<Models.DesignElement> Elements { get; } = new();

    [ObservableProperty]
    private string paramsJson = "{\n  \"CustomerName\": \"John Doe\",\n  \"Total\": 123.45\n}";

    public IAsyncRelayCommand AddTemplateCommand { get; }
    public IRelayCommand AddTextCommand { get; }
    public IRelayCommand AddExprCommand { get; }
    public IRelayCommand AddRectCommand { get; }
    public IRelayCommand ClearElementsCommand { get; }
    public IRelayCommand RefreshRenderCommand { get; }
    public IRelayCommand SaveLayoutCommand { get; }
    public IRelayCommand PrintCommand { get; }

    public MainViewModel(Microsoft.Extensions.Hosting.IHost host, Canvas canvas)
    {
        _service = host.Services.GetService(typeof(TemplateService)) as TemplateService
                   ?? throw new System.InvalidOperationException("TemplateService not registered");
        _evaluator = host.Services.GetService(typeof(IExpressionEvaluator)) as IExpressionEvaluator
                     ?? throw new System.InvalidOperationException("IExpressionEvaluator not registered");
        _canvas = canvas;

        AddTemplateCommand = new AsyncRelayCommand(AddTemplateAsync);
        AddTextCommand = new RelayCommand(AddText);
        AddExprCommand = new RelayCommand(AddExpr);
        AddRectCommand = new RelayCommand(AddRect);
        ClearElementsCommand = new RelayCommand(() => Elements.Clear());
        RefreshRenderCommand = new RelayCommand(RefreshRender);
        SaveLayoutCommand = new RelayCommand(SaveLayout);
        PrintCommand = new RelayCommand(Print);
        _ = LoadAsync();
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
        NewTemplateName = string.Empty;
    }

    partial void OnSelectedTemplateChanged(Template? value)
    {
        LoadLayout();
        RefreshRender();
    }

    private void AddText()
    {
        Elements.Add(new Models.DesignElement { Kind = Models.ElementKind.Text, DisplayText = "Text", X = 50, Y = 50, Width = 120, Height = 24, FontSize = 14 });
    }
    private void AddExpr()
    {
        Elements.Add(new Models.DesignElement { Kind = Models.ElementKind.Expression, Expression = "ROUND(Total,2)", DisplayText = "{{expr}}", X = 50, Y = 90, Width = 140, Height = 24, FontSize = 14 });
    }
    private void AddRect()
    {
        Elements.Add(new Models.DesignElement { Kind = Models.ElementKind.Rectangle, X = 40, Y = 140, Width = 200, Height = 60 });
    }

    private void RefreshRender()
    {
        Dictionary<string, object?> parameters = new();
        try
        {
            if (!string.IsNullOrWhiteSpace(ParamsJson))
            {
                parameters = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object?>>(ParamsJson) ?? new();
            }
        }
        catch { parameters = new(); }

        foreach (var el in Elements)
        {
            if (el.Kind == Models.ElementKind.Text)
            {
                // leave DisplayText
            }
            else if (el.Kind == Models.ElementKind.Expression)
            {
                try
                {
                    var val = _evaluator.Evaluate(el.Expression ?? string.Empty, parameters);
                    el.DisplayText = val?.ToString() ?? string.Empty;
                }
                catch
                {
                    el.DisplayText = "{{error}}";
                }
            }
        }
    }

    private void SaveLayout()
    {
        if (SelectedTemplate is null) return;
        var json = System.Text.Json.JsonSerializer.Serialize(Elements);
        SelectedTemplate.Description = json;
        _ = _service.UpdateDescriptionAsync(SelectedTemplate.Id, json);
    }

    private void LoadLayout()
    {
        Elements.Clear();
        if (SelectedTemplate is null) return;
        try
        {
            if (!string.IsNullOrWhiteSpace(SelectedTemplate.Description))
            {
                var list = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.List<Models.DesignElement>>(SelectedTemplate.Description!);
                if (list != null)
                {
                    foreach (var el in list) Elements.Add(el);
                }
            }
        }
        catch
        {
            // ignore malformed json
        }
    }

    private void Print()
    {
        var dlg = new PrintDialog();
        if (dlg.ShowDialog() == true)
        {
            dlg.PrintVisual(_canvas, SelectedTemplate?.Name ?? "Report");
        }
    }
}
