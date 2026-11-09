namespace POS.Avalonia.Models;

public sealed class StepEntry
{
    public int Number { get; set; }
    public string Text { get; set; } = "";
    public string? Warning { get; set; }
}
