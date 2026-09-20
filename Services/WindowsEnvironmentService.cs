using System.ComponentModel;
using System.Runtime.InteropServices;
using EnvironmentVariablesEditor.Localization;
using Serilog;

namespace EnvironmentVariablesEditor.Services;

public sealed class WindowsEnvironmentService(ILogger logger) : IEnvironmentService
{
    private const int HwndBroadcast = 0xffff;
    private const int WmSettingChange = 0x001a;
    private const uint SmtoAbortIfHung = 0x0002;

    public string GetOrInitialize(string name, string defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User);
        if (value is not null) return value;

        Environment.SetEnvironmentVariable(name, defaultValue, EnvironmentVariableTarget.User);
        logger.Information(Strings.LogVariableCreated,
            name, defaultValue.Length);
        NotifyEnvironmentChanged();
        return defaultValue;
    }

    public void Set(string name, string value)
    {
        Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.User);
        logger.Information(Strings.LogVariableChanged,
            name, value.Length);
    }

    public void NotifyEnvironmentChanged()
    {
        var result = SendMessageTimeout(
            new IntPtr(HwndBroadcast), WmSettingChange, IntPtr.Zero, "Environment",
            SmtoAbortIfHung, 5000, out _);

        if (result == IntPtr.Zero)
            logger.Warning(Strings.LogBroadcastFailed,
                new Win32Exception().NativeErrorCode);
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr SendMessageTimeout(
        IntPtr hWnd, int msg, IntPtr wParam, string lParam,
        uint flags, uint timeout, out IntPtr result);
}
