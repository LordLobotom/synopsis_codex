using System;
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportGenerator.Application.Services;
using ReportGenerator.Domain.Interfaces;
using ReportGenerator.Infrastructure.Data;
using ReportGenerator.Infrastructure.Expressions;
using ReportGenerator.Infrastructure.Repositories;
using Serilog;
using System.Threading.Tasks;

namespace ReportGenerator.App;

public partial class App : System.Windows.Application
{
    public static IHost? HostInstance { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var envConfig = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var logsPath = Path.Combine(AppContext.BaseDirectory, "Logs");
        Directory.CreateDirectory(logsPath);

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(envConfig)
            .WriteTo.File(Path.Combine(logsPath, "app-.log"), rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // Global exception logging
        this.DispatcherUnhandledException += (s, exArgs) =>
        {
            Log.Error(exArgs.Exception, "DispatcherUnhandledException");
            try
            {
                var logsPath = Path.Combine(AppContext.BaseDirectory, "Logs");
                MessageBox.Show($"Došlo k chybě: {exArgs.Exception.Message}\nPodrobnosti v logu: {logsPath}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch { }
            exArgs.Handled = true; // prevent app crash; user can continue
        };
        AppDomain.CurrentDomain.UnhandledException += (s, exArgs) =>
        {
            Log.Fatal(exArgs.ExceptionObject as Exception, "UnhandledException");
        };
        TaskScheduler.UnobservedTaskException += (s, exArgs) =>
        {
            Log.Error(exArgs.Exception, "UnobservedTaskException");
            exArgs.SetObserved();
        };

        HostInstance = new HostBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite($"Data Source={Path.Combine(AppContext.BaseDirectory, "templates.db")}"));

                services.AddScoped<ITemplateRepository, TemplateRepository>();
                services.AddTransient<TemplateService>();
                services.AddSingleton<IExpressionEvaluator, NCalcExpressionEvaluator>();

                services.AddSingleton<MainWindow>();
                services.AddTransient<TemplateDesignerWindow>();
            })
            .UseSerilog()
            .Build();

        using var scope = HostInstance.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();

        var main = scope.ServiceProvider.GetRequiredService<MainWindow>();
        MainWindow = main;
        main.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        Log.CloseAndFlush();
        HostInstance?.Dispose();
    }
}
