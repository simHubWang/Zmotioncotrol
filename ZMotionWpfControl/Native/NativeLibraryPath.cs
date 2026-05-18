using System.Runtime.InteropServices;

namespace ZMotionWpfControl.Native;

public static class NativeLibraryPath
{
    public static void UseApplicationDirectory()
    {
        var appDirectory = AppContext.BaseDirectory;
        SetDllDirectory(appDirectory);
    }

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool SetDllDirectory(string lpPathName);
}
