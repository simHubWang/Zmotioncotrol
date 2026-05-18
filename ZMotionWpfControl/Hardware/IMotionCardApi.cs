using ZMotionWpfControl.Models;

namespace ZMotionWpfControl.Hardware;

public interface IMotionCardApi
{
    bool IsOpen { get; }
    IReadOnlyList<AxisStatus> Axes { get; }

    Task OpenEthernetAsync(string ipAddress, int port);
    Task CloseAsync();
    Task SetServoAsync(int axis, bool enabled);
    Task HomeAsync(int axis);
    Task MoveAbsoluteAsync(int axis, double position, double speed);
    Task MoveRelativeAsync(int axis, double distance, double speed);
    Task JogAsync(int axis, double speed);
    Task StopAxisAsync(int axis);
    Task EmergencyStopAsync();
    Task ClearAlarmAsync(int axis);
}
