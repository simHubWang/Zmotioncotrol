using ZMotionWpfControl.Models;

namespace ZMotionWpfControl.Services;

public sealed class MockMotionController : IMotionController
{
    private readonly List<AxisStatus> _axes =
    [
        new() { Axis = 0, Name = "X轴" },
        new() { Axis = 1, Name = "Y轴" },
        new() { Axis = 2, Name = "Z轴" },
        new() { Axis = 3, Name = "R轴" },
        new() { Axis = 4, Name = "A轴" },
        new() { Axis = 5, Name = "B轴" }
    ];

    public bool IsConnected { get; private set; }
    public IReadOnlyList<AxisStatus> Axes => _axes;

    public async Task ConnectAsync(string ipAddress, int port)
    {
        await Task.Delay(250);
        IsConnected = true;
    }

    public Task DisconnectAsync()
    {
        IsConnected = false;
        foreach (var axis in _axes)
        {
            axis.Enabled = false;
            axis.Busy = false;
            axis.Speed = 0;
        }

        return Task.CompletedTask;
    }

    public Task ServoOnAsync(int axis, bool enabled)
    {
        GetAxis(axis).Enabled = enabled;
        return Task.CompletedTask;
    }

    public async Task HomeAsync(int axis)
    {
        var item = GetAxis(axis);
        item.Busy = true;
        item.Speed = 40;
        await Task.Delay(600);
        item.Position = 0;
        item.Speed = 0;
        item.Busy = false;
    }

    public async Task MoveAbsoluteAsync(int axis, double position, double speed)
    {
        var item = GetAxis(axis);
        EnsureCanMove(item);
        item.Busy = true;
        item.Speed = speed;
        await Task.Delay(400);
        item.Position = position;
        item.Speed = 0;
        item.Busy = false;
    }

    public Task MoveRelativeAsync(int axis, double distance, double speed)
    {
        var item = GetAxis(axis);
        return MoveAbsoluteAsync(axis, item.Position + distance, speed);
    }

    public Task JogAsync(int axis, double speed)
    {
        var item = GetAxis(axis);
        EnsureCanMove(item);
        item.Busy = true;
        item.Speed = speed;
        item.Position += Math.Sign(speed) * 0.5;
        return Task.CompletedTask;
    }

    public Task StopAxisAsync(int axis)
    {
        var item = GetAxis(axis);
        item.Busy = false;
        item.Speed = 0;
        return Task.CompletedTask;
    }

    public Task EmergencyStopAsync()
    {
        foreach (var axis in _axes)
        {
            axis.Busy = false;
            axis.Speed = 0;
        }

        return Task.CompletedTask;
    }

    public Task ClearAlarmAsync(int axis)
    {
        GetAxis(axis).Alarm = false;
        return Task.CompletedTask;
    }

    private AxisStatus GetAxis(int axis)
    {
        if (!IsConnected)
        {
            throw new InvalidOperationException("控制卡未连接。");
        }

        return _axes.First(item => item.Axis == axis);
    }

    private static void EnsureCanMove(AxisStatus axis)
    {
        if (axis.Alarm)
        {
            throw new InvalidOperationException($"{axis.Name} 存在报警，请先清除报警。");
        }

        if (!axis.Enabled)
        {
            throw new InvalidOperationException($"{axis.Name} 未使能。");
        }
    }
}
