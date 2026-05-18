using ZMotionWpfControl.Hardware;
using ZMotionWpfControl.Models;

namespace ZMotionWpfControl.Services;

public class MotionController : IMotionController
{
    private readonly IMotionCardApi _api;

    public MotionController(IMotionCardApi api)
    {
        _api = api;
    }

    public bool IsConnected => _api.IsOpen;
    public IReadOnlyList<AxisStatus> Axes => _api.Axes;

    public Task ConnectAsync(string ipAddress, int port)
    {
        return _api.OpenEthernetAsync(ipAddress, port);
    }

    public Task DisconnectAsync()
    {
        return _api.CloseAsync();
    }

    public Task ServoOnAsync(int axis, bool enabled)
    {
        return _api.SetServoAsync(axis, enabled);
    }

    public Task HomeAsync(int axis)
    {
        return _api.HomeAsync(axis);
    }

    public Task MoveAbsoluteAsync(int axis, double position, double speed)
    {
        return _api.MoveAbsoluteAsync(axis, position, speed);
    }

    public Task MoveRelativeAsync(int axis, double distance, double speed)
    {
        return _api.MoveRelativeAsync(axis, distance, speed);
    }

    public Task JogAsync(int axis, double speed)
    {
        return _api.JogAsync(axis, speed);
    }

    public Task StopAxisAsync(int axis)
    {
        return _api.StopAxisAsync(axis);
    }

    public Task EmergencyStopAsync()
    {
        return _api.EmergencyStopAsync();
    }

    public Task ClearAlarmAsync(int axis)
    {
        return _api.ClearAlarmAsync(axis);
    }
}
