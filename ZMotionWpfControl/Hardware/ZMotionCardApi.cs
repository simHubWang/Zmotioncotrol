using ZMotionWpfControl.Models;

namespace ZMotionWpfControl.Hardware;

public sealed class ZMotionCardApi : IMotionCardApi
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

    public bool IsOpen { get; private set; }
    public IReadOnlyList<AxisStatus> Axes => _axes;

    public Task OpenEthernetAsync(string ipAddress, int port)
    {
        // TODO: 接入正运动 SDK 后，在这里调用 ZAux_OpenEth。
        throw new NotImplementedException("请先添加正运动 SDK 的 zmcaux.cs 和 zauxdll.dll。");
    }

    public Task CloseAsync()
    {
        // TODO: 调用 ZAux_Close，并清理控制器句柄。
        throw new NotImplementedException("请先添加正运动 SDK 的 zmcaux.cs 和 zauxdll.dll。");
    }

    public Task SetServoAsync(int axis, bool enabled)
    {
        // TODO: 调用正运动轴使能 API。
        throw new NotImplementedException("请先添加正运动 SDK 的 zmcaux.cs 和 zauxdll.dll。");
    }

    public Task HomeAsync(int axis)
    {
        // TODO: 按实际工艺调用回零相关 API。
        throw new NotImplementedException("请先添加正运动 SDK 的 zmcaux.cs 和 zauxdll.dll。");
    }

    public Task MoveAbsoluteAsync(int axis, double position, double speed)
    {
        // TODO: 调用 ZAux_Direct_Single_MoveAbs 或 SDK 对应绝对运动 API。
        throw new NotImplementedException("请先添加正运动 SDK 的 zmcaux.cs 和 zauxdll.dll。");
    }

    public Task MoveRelativeAsync(int axis, double distance, double speed)
    {
        // TODO: 调用 ZAux_Direct_Single_Move 或 SDK 对应相对运动 API。
        throw new NotImplementedException("请先添加正运动 SDK 的 zmcaux.cs 和 zauxdll.dll。");
    }

    public Task JogAsync(int axis, double speed)
    {
        // TODO: 调用 ZAux_Direct_Single_Vmove 或 SDK 对应连续运动 API。
        throw new NotImplementedException("请先添加正运动 SDK 的 zmcaux.cs 和 zauxdll.dll。");
    }

    public Task StopAxisAsync(int axis)
    {
        // TODO: 调用 ZAux_Direct_Single_Cancel 或 SDK 对应单轴停止 API。
        throw new NotImplementedException("请先添加正运动 SDK 的 zmcaux.cs 和 zauxdll.dll。");
    }

    public Task EmergencyStopAsync()
    {
        // TODO: 调用正运动急停/全部轴停止 API。
        throw new NotImplementedException("请先添加正运动 SDK 的 zmcaux.cs 和 zauxdll.dll。");
    }

    public Task ClearAlarmAsync(int axis)
    {
        // TODO: 调用正运动报警清除 API，并刷新轴状态。
        throw new NotImplementedException("请先添加正运动 SDK 的 zmcaux.cs 和 zauxdll.dll。");
    }
}
