using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ZMotionWpfControl.Models;
using ZMotionWpfControl.Services;

namespace ZMotionWpfControl;

public partial class MainWindow : Window
{
    private readonly IMotionController _controller = new MockMotionController();
    private readonly ObservableCollection<AxisStatus> _axisStatuses = [];
    private readonly ObservableCollection<string> _logs = [];

    public MainWindow()
    {
        InitializeComponent();

        foreach (var axis in _controller.Axes)
        {
            _axisStatuses.Add(axis);
        }

        AxisComboBox.ItemsSource = _axisStatuses;
        AxisStatusGrid.ItemsSource = _axisStatuses;
        LogListBox.ItemsSource = _logs;
        RefreshUi();
        AddLog("程序启动，当前使用模拟控制器。");
    }

    private async void ConnectButton_Click(object sender, RoutedEventArgs e)
    {
        await RunCommandAsync(async () =>
        {
            if (!int.TryParse(PortTextBox.Text, out var port))
            {
                throw new InvalidOperationException("端口格式不正确。");
            }

            await _controller.ConnectAsync(IpAddressTextBox.Text.Trim(), port);
            AddLog($"已连接控制卡 {IpAddressTextBox.Text.Trim()}:{port}");
        });
    }

    private async void DisconnectButton_Click(object sender, RoutedEventArgs e)
    {
        await RunCommandAsync(async () =>
        {
            await _controller.DisconnectAsync();
            AddLog("控制卡连接已断开。");
        });
    }

    private async void ServoOnButton_Click(object sender, RoutedEventArgs e)
    {
        await RunAxisCommandAsync(axis => _controller.ServoOnAsync(axis, true), "伺服使能");
    }

    private async void ServoOffButton_Click(object sender, RoutedEventArgs e)
    {
        await RunAxisCommandAsync(axis => _controller.ServoOnAsync(axis, false), "伺服断开");
    }

    private async void HomeButton_Click(object sender, RoutedEventArgs e)
    {
        await RunAxisCommandAsync(axis => _controller.HomeAsync(axis), "回零完成");
    }

    private async void ClearAlarmButton_Click(object sender, RoutedEventArgs e)
    {
        await RunAxisCommandAsync(axis => _controller.ClearAlarmAsync(axis), "报警已清除");
    }

    private async void MoveAbsoluteButton_Click(object sender, RoutedEventArgs e)
    {
        await RunAxisCommandAsync(axis =>
        {
            var target = ReadDouble(TargetPositionTextBox.Text, "目标位置");
            var speed = ReadDouble(MoveSpeedTextBox.Text, "速度");
            return _controller.MoveAbsoluteAsync(axis, target, speed);
        }, "绝对运动完成");
    }

    private async void MoveRelativeButton_Click(object sender, RoutedEventArgs e)
    {
        await RunAxisCommandAsync(axis =>
        {
            var distance = ReadDouble(TargetPositionTextBox.Text, "相对距离");
            var speed = ReadDouble(MoveSpeedTextBox.Text, "速度");
            return _controller.MoveRelativeAsync(axis, distance, speed);
        }, "相对运动完成");
    }

    private async void StopAxisButton_Click(object sender, RoutedEventArgs e)
    {
        await RunAxisCommandAsync(axis => _controller.StopAxisAsync(axis), "单轴已停止");
    }

    private async void EmergencyStopButton_Click(object sender, RoutedEventArgs e)
    {
        await RunCommandAsync(async () =>
        {
            await _controller.EmergencyStopAsync();
            AddLog("急停已执行，全部轴停止。");
        });
    }

    private async void JogPositiveButton_Down(object sender, MouseButtonEventArgs e)
    {
        await RunJogAsync(1);
    }

    private async void JogNegativeButton_Down(object sender, MouseButtonEventArgs e)
    {
        await RunJogAsync(-1);
    }

    private async void JogButton_MouseUp(object sender, MouseButtonEventArgs e)
    {
        await StopJogAsync();
    }

    private async void JogButton_MouseLeave(object sender, MouseEventArgs e)
    {
        await StopJogAsync();
    }

    private async Task StopJogAsync()
    {
        var axis = GetSelectedAxis();
        if (axis < 0 || !_controller.IsConnected)
        {
            return;
        }

        await _controller.StopAxisAsync(axis);
        AddLog($"{GetSelectedAxisName()} 点动停止");
        RefreshUi();
    }

    private async Task RunJogAsync(int direction)
    {
        await RunAxisCommandAsync(axis =>
        {
            var speed = Math.Abs(ReadDouble(JogSpeedTextBox.Text, "点动速度")) * direction;
            return _controller.JogAsync(axis, speed);
        }, direction > 0 ? "正向点动" : "负向点动");
    }

    private async Task RunAxisCommandAsync(Func<int, Task> action, string message)
    {
        var axis = GetSelectedAxis();
        if (axis < 0)
        {
            AddLog("请先选择轴。");
            return;
        }

        await RunCommandAsync(async () =>
        {
            await action(axis);
            AddLog($"{GetSelectedAxisName()} {message}");
        });
    }

    private async Task RunCommandAsync(Func<Task> action)
    {
        try
        {
            SetBusy(true);
            await action();
        }
        catch (Exception ex)
        {
            AddLog($"错误：{ex.Message}");
            MessageBox.Show(ex.Message, "运动控制", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        finally
        {
            SetBusy(false);
            RefreshUi();
        }
    }

    private int GetSelectedAxis()
    {
        return AxisComboBox.SelectedValue is int axis ? axis : -1;
    }

    private string GetSelectedAxisName()
    {
        return AxisComboBox.SelectedItem is AxisStatus axis ? axis.Name : "当前轴";
    }

    private static double ReadDouble(string value, string name)
    {
        if (!double.TryParse(value, out var number))
        {
            throw new InvalidOperationException($"{name}格式不正确。");
        }

        return number;
    }

    private void SetBusy(bool isBusy)
    {
        Cursor = isBusy ? Cursors.Wait : Cursors.Arrow;
    }

    private void RefreshUi()
    {
        ConnectionStateText.Text = _controller.IsConnected ? "已连接" : "未连接";
        AxisStatusGrid.Items.Refresh();
    }

    private void AddLog(string message)
    {
        _logs.Insert(0, $"{DateTime.Now:HH:mm:ss}  {message}");
        while (_logs.Count > 120)
        {
            _logs.RemoveAt(_logs.Count - 1);
        }
    }
}
