using System;
using Avalonia;
using Avalonia.Controls;
using POS.Avalonia.Services;

namespace POS.Avalonia.Views;

public partial class MainWindow : Window
{
    private AppWindowDebugInfo? _debugInfo;

    public MainWindow()
    {
        InitializeComponent();
        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Program.OnApplyDisplaySize += ApplyDisplaySize;
        Program.OnApplyFullScreen += ApplyFullScreen;
        try { _debugInfo = Program.GetService<AppWindowDebugInfo>(); } catch { }
        SizeChanged += OnSizeChanged;
        ApplyInitialSize();
        UpdateDebugInfo();
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        UpdateDebugInfo();
    }

    private void ApplyInitialSize()
    {
        try
        {
            var store = Program.GetService<ISettingsStore>();
            var key = store.Get("UI:Size") ?? UiSizePresets.Key1080;
            if (UiSizePresets.IsFullScreen(key))
            {
                WindowState = WindowState.FullScreen;
            }
            else
            {
                var (w, h) = UiSizePresets.GetDimensions(key);
                Width = w;
                Height = h;
            }
            UpdateDebugInfo();
        }
        catch { }
    }

    protected override void OnClosed(EventArgs e)
    {
        Program.OnApplyDisplaySize -= ApplyDisplaySize;
        Program.OnApplyFullScreen -= ApplyFullScreen;
        SizeChanged -= OnSizeChanged;
        base.OnClosed(e);
    }

    private void ApplyDisplaySize(int width, int height)
    {
        WindowState = WindowState.Normal;
        Width = width;
        Height = height;
        UpdateDebugInfo();
    }

    private void ApplyFullScreen()
    {
        WindowState = WindowState.Normal;
        try
        {
            WindowState = WindowState.FullScreen;
        }
        catch
        {
            WindowState = WindowState.Maximized;
        }
        UpdateDebugInfo();
    }

    private void UpdateDebugInfo()
    {
        if (_debugInfo == null) return;
        if (WindowState == WindowState.FullScreen)
        {
            var client = ClientSize;
            _debugInfo.Update((int)client.Width, (int)client.Height, isFullScreen: true);
        }
        else
        {
            var client = ClientSize;
            _debugInfo.Update((int)client.Width, (int)client.Height, isFullScreen: false);
        }
    }
}