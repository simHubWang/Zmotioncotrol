using ZMotionWpfControl.Models;

namespace ZMotionWpfControl.Services;

public interface IMotionController
{
    bool IsConnected { get; }
    IReadOnlyList<AxisStatus> Axes { get; }

    Task ConnectAsync(string ipAddress, int port);
    Task DisconnectAsync();
    Task ServoOnAsync(int axis, bool enabled);
    Task HomeAsync(int axis);
    Task MoveAbsoluteAsync(int axis, double position, double speed);
    Task MoveRelativeAsync(int axis, double distance, double speed);
    Task JogAsync(int axis, double speed);
    Task StopAxisAsync(int axis);
    Task EmergencyStopAsync();
    Task ClearAlarmAsync(int axis);
}
