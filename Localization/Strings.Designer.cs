using System.Globalization;
using System.Resources;

#nullable enable

namespace EnvironmentVariablesEditor.Localization;

/// <summary>Строго типизированный доступ к локализуемым строкам приложения.</summary>
public static class Strings
{
    private static readonly ResourceManager ResourceManager =
        new("EnvironmentVariablesEditor.Localization.Strings", typeof(Strings).Assembly);

    public static CultureInfo? Culture { get; set; }

    private static string Get(string name) =>
        ResourceManager.GetString(name, Culture)
        ?? throw new MissingManifestResourceException($"Resource '{name}' was not found.");

    public static string WindowTitle => Get(nameof(WindowTitle));
    public static string ColumnField => Get(nameof(ColumnField));
    public static string ColumnValue => Get(nameof(ColumnValue));
    public static string ColumnComment => Get(nameof(ColumnComment));
    public static string SaveButton => Get(nameof(SaveButton));
    public static string MinimizeButton => Get(nameof(MinimizeButton));
    public static string CloseButton => Get(nameof(CloseButton));
    public static string StatusReady => Get(nameof(StatusReady));
    public static string StatusUnsavedChanges => Get(nameof(StatusUnsavedChanges));
    public static string StatusAllChangesSaved => Get(nameof(StatusAllChangesSaved));
    public static string StatusSavedFormat => Get(nameof(StatusSavedFormat));
    public static string StatusSaveFailed => Get(nameof(StatusSaveFailed));
    public static string StartupErrorTitle => Get(nameof(StartupErrorTitle));
    public static string StartupErrorMessageFormat => Get(nameof(StartupErrorMessageFormat));
    public static string LogStartupFailed => Get(nameof(LogStartupFailed));
    public static string LogSaveFailed => Get(nameof(LogSaveFailed));
    public static string LogVariableCreated => Get(nameof(LogVariableCreated));
    public static string LogVariableChanged => Get(nameof(LogVariableChanged));
    public static string LogBroadcastFailed => Get(nameof(LogBroadcastFailed));
    public static string ConfigurationVariablesMissing => Get(nameof(ConfigurationVariablesMissing));
    public static string ConfigurationLoggingMissing => Get(nameof(ConfigurationLoggingMissing));
    public static string ConfigurationLoggingLevelInvalidFormat => Get(nameof(ConfigurationLoggingLevelInvalidFormat));
}
