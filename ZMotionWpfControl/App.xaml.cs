using System.Windows;
using ZMotionWpfControl.Native;

namespace ZMotionWpfControl;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        NativeLibraryPath.UseApplicationDirectory();
        base.OnStartup(e);
    }
}
