using Avalonia.Controls;

namespace POS.Avalonia.Views;

public partial class PlaceholderView : UserControl
{
    public PlaceholderView() => InitializeComponent();
    public string PlaceholderText { get => Tb?.Text ?? ""; set { if (Tb != null) Tb.Text = value; } }
}
