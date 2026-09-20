using Microsoft.Extensions.Configuration;
using EnvironmentVariablesEditor.Localization;
using Serilog.Events;

namespace EnvironmentVariablesEditor.Services;

public sealed record AppOptions(
    IReadOnlyList<string> EnvironmentVariables,
    string DefaultValue,
    IReadOnlyDictionary<string, string> Comments,
    LogOptions Logging)
{
    public static AppOptions FromConfiguration(IConfiguration configuration)
    {
        var names = configuration.GetSection("EnvironmentVariables")
            .GetChildren()
            .Select(item => item.Value?.Trim())
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Cast<string>()
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (names.Length == 0)
            throw new InvalidOperationException(Strings.ConfigurationVariablesMissing);

        var comments = configuration.GetSection("Comments")
            .GetChildren()
            .Where(item => item.Value is not null)
            .ToDictionary(item => item.Key, item => item.Value!, StringComparer.OrdinalIgnoreCase);

        return new AppOptions(
            names,
            configuration["DefaultValue"] ?? string.Empty,
            comments,
            LogOptions.FromConfiguration(configuration.GetSection("Logging")));
    }
}

public sealed record LogOptions(
    string Directory,
    string FileName,
    LogEventLevel MinimumLevel,
    string OutputTemplate)
{
    public static LogOptions FromConfiguration(IConfigurationSection section)
    {
        var directory = section["Directory"];
        var fileName = section["FileName"];
        var minimumLevelText = section["MinimumLevel"];
        var outputTemplate = section["OutputTemplate"];

        if (string.IsNullOrWhiteSpace(directory) ||
            string.IsNullOrWhiteSpace(fileName) ||
            string.IsNullOrWhiteSpace(minimumLevelText) ||
            string.IsNullOrWhiteSpace(outputTemplate))
            throw new InvalidOperationException(Strings.ConfigurationLoggingMissing);

        if (!Enum.TryParse<LogEventLevel>(minimumLevelText, true, out var minimumLevel))
            throw new InvalidOperationException(
                string.Format(Strings.ConfigurationLoggingLevelInvalidFormat, minimumLevelText));

        return new LogOptions(directory, fileName, minimumLevel, outputTemplate);
    }
}
