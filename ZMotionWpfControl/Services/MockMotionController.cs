using ZMotionWpfControl.Hardware;

namespace ZMotionWpfControl.Services;

public sealed class MockMotionController : MotionController
{
    public MockMotionController()
        : base(new SimulatedMotionCardApi())
    {
    }
}
