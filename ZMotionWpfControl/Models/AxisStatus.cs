using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ZMotionWpfControl.Models;

public sealed class AxisStatus : INotifyPropertyChanged
{
    private double _position;
    private double _speed;
    private bool _enabled;
    private bool _alarm;
    private bool _busy;

    public event PropertyChangedEventHandler? PropertyChanged;

    public int Axis { get; init; }
    public string Name { get; init; } = string.Empty;

    public double Position
    {
        get => _position;
        set => SetProperty(ref _position, value);
    }

    public double Speed
    {
        get => _speed;
        set => SetProperty(ref _speed, value);
    }

    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (SetProperty(ref _enabled, value))
            {
                OnPropertyChanged(nameof(State));
            }
        }
    }

    public bool Alarm
    {
        get => _alarm;
        set
        {
            if (SetProperty(ref _alarm, value))
            {
                OnPropertyChanged(nameof(State));
            }
        }
    }

    public bool Busy
    {
        get => _busy;
        set
        {
            if (SetProperty(ref _busy, value))
            {
                OnPropertyChanged(nameof(State));
            }
        }
    }

    public string State => Alarm ? "报警" : Busy ? "运动中" : Enabled ? "使能" : "未使能";

    private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
        {
            return false;
        }

        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
