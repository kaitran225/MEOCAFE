using System;
using Avalonia;
using Avalonia.Controls;
using POS.Avalonia.Services;

namespace POS.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Program.OnApplyDisplaySize += ApplyDisplaySize;
    }

    protected override void OnClosed(EventArgs e)
    {
        Program.OnApplyDisplaySize -= ApplyDisplaySize;
        base.OnClosed(e);
    }

    private void ApplyDisplaySize(int width, int height)
    {
        Width = width;
        Height = height;
    }
}