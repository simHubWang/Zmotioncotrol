namespace ZMotionWpfControl.Models;

public sealed class AxisStatus
{
    public int Axis { get; init; }
    public string Name { get; init; } = string.Empty;
    public double Position { get; set; }
    public double Speed { get; set; }
    public bool Enabled { get; set; }
    public bool Alarm { get; set; }
    public bool Busy { get; set; }
    public string State => Alarm ? "报警" : Busy ? "运动中" : Enabled ? "使能" : "未使能";
}
