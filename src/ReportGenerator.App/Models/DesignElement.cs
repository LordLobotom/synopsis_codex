using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;

namespace ReportGenerator.App.Models;

public enum ElementKind
{
    Text,
    Expression,
    Rectangle,
    Barcode
}

public partial class DesignElement : ObservableObject
{
    public Guid Id { get; init; } = Guid.NewGuid();

    [ObservableProperty]
    private ElementKind kind;

    [ObservableProperty]
    private double x;

    [ObservableProperty]
    private double y;

    [ObservableProperty]
    private double width = 120;

    [ObservableProperty]
    private double height = 24;

    [ObservableProperty]
    private string? text;

    [ObservableProperty]
    private string? expression;

    [ObservableProperty]
    private string? displayText;

    [ObservableProperty]
    private double fontSize = 12;

    [ObservableProperty]
    private ImageSource? imageSource;

    [ObservableProperty]
    private string barcodeFormat = "CODE_128";
}
