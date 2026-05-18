using cszmcaux;
using ZMotionWpfControl.Models;

namespace ZMotionWpfControl.Hardware;

public sealed class ZMotionCardApi : IMotionCardApi
{
    private const float DefaultAccel = 100;
    private const float DefaultDecel = 100;
    private const int StopMode = 2;

    private IntPtr _handle = IntPtr.Zero;

    private readonly List<AxisStatus> _axes =
    [
        new() { Axis = 0, Name = "X轴" },
        new() { Axis = 1, Name = "Y轴" },
        new() { Axis = 2, Name = "Z轴" },
        new() { Axis = 3, Name = "R轴" },
        new() { Axis = 4, Name = "A轴" },
        new() { Axis = 5, Name = "B轴" }
    ];

    public bool IsOpen { get; private set; }
    public IReadOnlyList<AxisStatus> Axes => _axes;

    public Task OpenEthernetAsync(string ipAddress, int port)
    {
        if (IsOpen)
        {
            return Task.CompletedTask;
        }

        CheckResult(zmcaux.ZAux_OpenEth(ipAddress, out _handle), "连接控制卡");
        IsOpen = true;
        RefreshAllAxes();
        return Task.CompletedTask;
    }

    public Task CloseAsync()
    {
        if (_handle != IntPtr.Zero)
        {
            CheckResult(zmcaux.ZAux_Close(_handle), "关闭控制卡连接");
        }

        _handle = IntPtr.Zero;
        IsOpen = false;
        foreach (var axis in _axes)
        {
            axis.Busy = false;
            axis.Speed = 0;
            axis.Enabled = false;
        }

        return Task.CompletedTask;
    }

    public Task SetServoAsync(int axis, bool enabled)
    {
        EnsureOpen();
        CheckResult(zmcaux.ZAux_Direct_SetAxisEnable(_handle, axis, enabled ? 1 : 0), "设置轴使能");
        RefreshAxis(axis);
        return Task.CompletedTask;
    }

    public Task HomeAsync(int axis)
    {
        EnsureOpen();
        CheckResult(zmcaux.ZAux_Direct_SetDpos(_handle, axis, 0), "清零轴坐标");
        RefreshAxis(axis);
        return Task.CompletedTask;
    }

    public Task MoveAbsoluteAsync(int axis, double position, double speed)
    {
        EnsureOpen();
        ConfigureMove(axis, speed);
        CheckResult(zmcaux.ZAux_Direct_Single_MoveAbs(_handle, axis, (float)position), "绝对运动");
        RefreshAxis(axis);
        return Task.CompletedTask;
    }

    public Task MoveRelativeAsync(int axis, double distance, double speed)
    {
        EnsureOpen();
        ConfigureMove(axis, speed);
        CheckResult(zmcaux.ZAux_Direct_Single_Move(_handle, axis, (float)distance), "相对运动");
        RefreshAxis(axis);
        return Task.CompletedTask;
    }

    public Task JogAsync(int axis, double speed)
    {
        EnsureOpen();
        ConfigureMove(axis, Math.Abs(speed));
        CheckResult(zmcaux.ZAux_Direct_Single_Vmove(_handle, axis, speed >= 0 ? 1 : -1), "连续点动");
        RefreshAxis(axis);
        return Task.CompletedTask;
    }

    public Task StopAxisAsync(int axis)
    {
        EnsureOpen();
        CheckResult(zmcaux.ZAux_Direct_Single_Cancel(_handle, axis, StopMode), "单轴停止");
        RefreshAxis(axis);
        return Task.CompletedTask;
    }

    public Task EmergencyStopAsync()
    {
        EnsureOpen();
        foreach (var axis in _axes)
        {
            CheckResult(zmcaux.ZAux_Direct_Single_Cancel(_handle, axis.Axis, StopMode), $"{axis.Name} 停止");
            RefreshAxis(axis.Axis);
        }

        return Task.CompletedTask;
    }

    public Task ClearAlarmAsync(int axis)
    {
        EnsureOpen();
        CheckResult(zmcaux.ZAux_BusCmd_DriveClear(_handle, (uint)axis, 0), "清除驱动报警");
        RefreshAxis(axis);
        return Task.CompletedTask;
    }

    private void ConfigureMove(int axis, double speed)
    {
        CheckResult(zmcaux.ZAux_Direct_SetSpeed(_handle, axis, (float)Math.Abs(speed)), "设置轴速度");
        CheckResult(zmcaux.ZAux_Direct_SetAccel(_handle, axis, DefaultAccel), "设置轴加速度");
        CheckResult(zmcaux.ZAux_Direct_SetDecel(_handle, axis, DefaultDecel), "设置轴减速度");
    }

    private void RefreshAllAxes()
    {
        foreach (var axis in _axes)
        {
            RefreshAxis(axis.Axis);
        }
    }

    private void RefreshAxis(int axis)
    {
        var item = _axes.First(x => x.Axis == axis);

        var dpos = 0f;
        if (zmcaux.ZAux_Direct_GetDpos(_handle, axis, ref dpos) == 0)
        {
            item.Position = dpos;
        }

        var speed = 0f;
        if (zmcaux.ZAux_Direct_GetMspeed(_handle, axis, ref speed) == 0)
        {
            item.Speed = speed;
        }

        var enabled = 0;
        if (zmcaux.ZAux_Direct_GetAxisEnable(_handle, axis, ref enabled) == 0)
        {
            item.Enabled = enabled != 0;
        }

        var idle = 1;
        if (zmcaux.ZAux_Direct_GetIfIdle(_handle, axis, ref idle) == 0)
        {
            item.Busy = idle == 0;
        }
    }

    private void EnsureOpen()
    {
        if (!IsOpen || _handle == IntPtr.Zero)
        {
            throw new InvalidOperationException("控制卡未连接。");
        }
    }

    private static void CheckResult(int result, string operation)
    {
        if (result != 0)
        {
            throw new InvalidOperationException($"{operation}失败，错误码：{result}");
        }
    }
}
