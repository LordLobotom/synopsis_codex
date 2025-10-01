using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportGenerator.Application.Services;
using ReportGenerator.Domain.Entities;

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

    public ObservableCollection<Template> Templates { get; } = new();

    [ObservableProperty]
    private string newTemplateName = string.Empty;

    public IAsyncRelayCommand AddTemplateCommand { get; }

    public MainViewModel(Microsoft.Extensions.Hosting.IHost host)
    {
        _service = host.Services.GetService(typeof(TemplateService)) as TemplateService
                   ?? throw new System.InvalidOperationException("TemplateService not registered");

        AddTemplateCommand = new AsyncRelayCommand(AddTemplateAsync);
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
}
