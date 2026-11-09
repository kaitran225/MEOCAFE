using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace POS.Avalonia.Controls;

public partial class QuantityStepper : UserControl
{
    public static readonly StyledProperty<int> ValueProperty =
        AvaloniaProperty.Register<QuantityStepper, int>(nameof(Value), 1);

    public static readonly StyledProperty<int> MinimumProperty =
        AvaloniaProperty.Register<QuantityStepper, int>(nameof(Minimum), 1);

    public static readonly StyledProperty<int> MaximumProperty =
        AvaloniaProperty.Register<QuantityStepper, int>(nameof(Maximum), 99);

    public int Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public int Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public int Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public ICommand IncrementCommand { get; }
    public ICommand DecrementCommand { get; }

    public QuantityStepper()
    {
        IncrementCommand = new SimpleCommand(() => Value < Maximum, () => Value++);
        DecrementCommand = new SimpleCommand(() => Value > Minimum, () => Value--);
        InitializeComponent();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ValueProperty ||
            change.Property == MinimumProperty ||
            change.Property == MaximumProperty)
        {
            (IncrementCommand as SimpleCommand)?.RaiseCanExecuteChanged();
            (DecrementCommand as SimpleCommand)?.RaiseCanExecuteChanged();
        }
    }

    private sealed class SimpleCommand : ICommand
    {
        private readonly Func<bool> _canExecute;
        private readonly Action _execute;

        public SimpleCommand(Func<bool> canExecute, Action execute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _canExecute();

        public void Execute(object? parameter) => _execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
