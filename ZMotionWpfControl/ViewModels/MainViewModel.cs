using System.Collections.ObjectModel;
using System.Windows.Input;
using ZMotionWpfControl.Models;
using ZMotionWpfControl.Services;

namespace ZMotionWpfControl.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private readonly IMotionController _controller;
    private AxisStatus? _selectedAxis;
    private string _ipAddress = "192.168.0.11";
    private string _port = "8089";
    private string _jogSpeed = "20";
    private string _targetPosition = "100";
    private string _moveSpeed = "50";
    private bool _isBusy;

    public MainViewModel(IMotionController controller)
    {
        _controller = controller;
        Axes = new ObservableCollection<AxisStatus>(_controller.Axes);
        Logs = [];
        SelectedAxis = Axes.FirstOrDefault();

        ConnectCommand = new AsyncRelayCommand(ConnectAsync);
        DisconnectCommand = new AsyncRelayCommand(DisconnectAsync);
        ServoOnCommand = new AsyncRelayCommand(() => RunAxisCommandAsync(axis => _controller.ServoOnAsync(axis, true), "伺服使能"));
        ServoOffCommand = new AsyncRelayCommand(() => RunAxisCommandAsync(axis => _controller.ServoOnAsync(axis, false), "伺服断开"));
        HomeCommand = new AsyncRelayCommand(() => RunAxisCommandAsync(axis => _controller.HomeAsync(axis), "回零完成"));
        ClearAlarmCommand = new AsyncRelayCommand(() => RunAxisCommandAsync(axis => _controller.ClearAlarmAsync(axis), "报警已清除"));
        MoveAbsoluteCommand = new AsyncRelayCommand(MoveAbsoluteAsync);
        MoveRelativeCommand = new AsyncRelayCommand(MoveRelativeAsync);
        StopAxisCommand = new AsyncRelayCommand(() => RunAxisCommandAsync(axis => _controller.StopAxisAsync(axis), "单轴已停止"));
        EmergencyStopCommand = new AsyncRelayCommand(EmergencyStopAsync);
        JogPositiveCommand = new AsyncRelayCommand(() => JogAsync(1));
        JogNegativeCommand = new AsyncRelayCommand(() => JogAsync(-1));
        StopJogCommand = new AsyncRelayCommand(StopJogAsync);

        AddLog("程序启动，当前使用模拟控制器。");
    }

    public ObservableCollection<AxisStatus> Axes { get; }
    public ObservableCollection<string> Logs { get; }

    public AxisStatus? SelectedAxis
    {
        get => _selectedAxis;
        set => SetProperty(ref _selectedAxis, value);
    }

    public string IpAddress
    {
        get => _ipAddress;
        set => SetProperty(ref _ipAddress, value);
    }

    public string Port
    {
        get => _port;
        set => SetProperty(ref _port, value);
    }

    public string JogSpeed
    {
        get => _jogSpeed;
        set => SetProperty(ref _jogSpeed, value);
    }

    public string TargetPosition
    {
        get => _targetPosition;
        set => SetProperty(ref _targetPosition, value);
    }

    public string MoveSpeed
    {
        get => _moveSpeed;
        set => SetProperty(ref _moveSpeed, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    public string ConnectionState => _controller.IsConnected ? "已连接" : "未连接";

    public ICommand ConnectCommand { get; }
    public ICommand DisconnectCommand { get; }
    public ICommand ServoOnCommand { get; }
    public ICommand ServoOffCommand { get; }
    public ICommand HomeCommand { get; }
    public ICommand ClearAlarmCommand { get; }
    public ICommand MoveAbsoluteCommand { get; }
    public ICommand MoveRelativeCommand { get; }
    public ICommand StopAxisCommand { get; }
    public ICommand EmergencyStopCommand { get; }
    public ICommand JogPositiveCommand { get; }
    public ICommand JogNegativeCommand { get; }
    public ICommand StopJogCommand { get; }

    private Task ConnectAsync()
    {
        return RunCommandAsync(async () =>
        {
            if (!int.TryParse(Port, out var port))
            {
                throw new InvalidOperationException("端口格式不正确。");
            }

            await _controller.ConnectAsync(IpAddress.Trim(), port);
            AddLog($"已连接控制卡 {IpAddress.Trim()}:{port}");
        });
    }

    private Task DisconnectAsync()
    {
        return RunCommandAsync(async () =>
        {
            await _controller.DisconnectAsync();
            AddLog("控制卡连接已断开。");
        });
    }

    private Task MoveAbsoluteAsync()
    {
        return RunAxisCommandAsync(axis =>
        {
            var target = ReadDouble(TargetPosition, "目标位置");
            var speed = ReadDouble(MoveSpeed, "速度");
            return _controller.MoveAbsoluteAsync(axis, target, speed);
        }, "绝对运动完成");
    }

    private Task MoveRelativeAsync()
    {
        return RunAxisCommandAsync(axis =>
        {
            var distance = ReadDouble(TargetPosition, "相对距离");
            var speed = ReadDouble(MoveSpeed, "速度");
            return _controller.MoveRelativeAsync(axis, distance, speed);
        }, "相对运动完成");
    }

    private Task EmergencyStopAsync()
    {
        return RunCommandAsync(async () =>
        {
            await _controller.EmergencyStopAsync();
            AddLog("急停已执行，全部轴停止。");
        });
    }

    private Task JogAsync(int direction)
    {
        return RunAxisCommandAsync(axis =>
        {
            var speed = Math.Abs(ReadDouble(JogSpeed, "点动速度")) * direction;
            return _controller.JogAsync(axis, speed);
        }, direction > 0 ? "正向点动" : "负向点动");
    }

    private Task StopJogAsync()
    {
        if (SelectedAxis is null || !_controller.IsConnected)
        {
            return Task.CompletedTask;
        }

        return RunCommandAsync(async () =>
        {
            await _controller.StopAxisAsync(SelectedAxis.Axis);
            AddLog($"{SelectedAxis.Name} 点动停止");
        });
    }

    private Task RunAxisCommandAsync(Func<int, Task> action, string message)
    {
        if (SelectedAxis is null)
        {
            AddLog("请先选择轴。");
            return Task.CompletedTask;
        }

        var axis = SelectedAxis.Axis;
        var axisName = SelectedAxis.Name;
        return RunCommandAsync(async () =>
        {
            await action(axis);
            AddLog($"{axisName} {message}");
        });
    }

    private async Task RunCommandAsync(Func<Task> action)
    {
        try
        {
            IsBusy = true;
            await action();
        }
        catch (Exception ex)
        {
            AddLog($"错误：{ex.Message}");
        }
        finally
        {
            IsBusy = false;
            OnPropertyChanged(nameof(ConnectionState));
        }
    }

    private static double ReadDouble(string value, string name)
    {
        if (!double.TryParse(value, out var number))
        {
            throw new InvalidOperationException($"{name}格式不正确。");
        }

        return number;
    }

    private void AddLog(string message)
    {
        Logs.Insert(0, $"{DateTime.Now:HH:mm:ss}  {message}");
        while (Logs.Count > 120)
        {
            Logs.RemoveAt(Logs.Count - 1);
        }
    }
}
