using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportGenerator.Application.Services;
using ReportGenerator.Domain.Entities;
using ReportGenerator.Domain.Interfaces;
using ReportGenerator.Infrastructure.Data;
using ReportGenerator.Infrastructure.Expressions;
using ReportGenerator.Infrastructure.Repositories;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={Path.Combine(AppContext.BaseDirectory, "templates.db")}"));

builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddTransient<TemplateService>();
builder.Services.AddSingleton<IExpressionEvaluator, NCalcExpressionEvaluator>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Ensure database exists and seed a sample template for first run
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    if (!db.Templates.Any())
    {
        db.Templates.Add(new Template
        {
            Name = "Sample",
            Description = "Invoice\n\nCustomer: {{ CustomerName }}\nTotal: {{ ROUND(Total, 2) }}"
        });
        db.SaveChanges();
    }
}

app.Run();
