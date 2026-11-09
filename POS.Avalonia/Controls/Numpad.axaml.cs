using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace POS.Avalonia.Controls;

public partial class Numpad : UserControl
{
    public static readonly StyledProperty<string> DisplayValueProperty =
        AvaloniaProperty.Register<Numpad, string>(nameof(DisplayValue), "0");

    public string DisplayValue
    {
        get => GetValue(DisplayValueProperty);
        set => SetValue(DisplayValueProperty, value);
    }

    public ICommand AppendCommand { get; }
    public ICommand BackspaceCommand { get; }
    public ICommand ClearCommand { get; }

    public Numpad()
    {
        AppendCommand = new NumpadCommand(param =>
        {
            var digit = param?.ToString() ?? "";
            DisplayValue = DisplayValue == "0" ? digit : DisplayValue + digit;
        });

        BackspaceCommand = new NumpadCommand(_ =>
        {
            if (DisplayValue.Length > 1)
                DisplayValue = DisplayValue[..^1];
            else
                DisplayValue = "0";
        });

        ClearCommand = new NumpadCommand(_ => DisplayValue = "0");

        InitializeComponent();
    }

    private sealed class NumpadCommand : ICommand
    {
        private readonly Action<object?> _execute;

        public NumpadCommand(Action<object?> execute) => _execute = execute;

#pragma warning disable CS0067
        public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => _execute(parameter);
    }
}
