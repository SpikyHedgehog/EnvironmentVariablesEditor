using System.IO;
using System.Windows;
using EnvironmentVariablesEditor.Localization;
using EnvironmentVariablesEditor.Services;
using EnvironmentVariablesEditor.ViewModels;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace EnvironmentVariablesEditor;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var basePath = AppContext.BaseDirectory;
        try
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            var options = AppOptions.FromConfiguration(configuration);
            var logDirectory = Path.IsPathRooted(options.Logging.Directory)
                ? options.Logging.Directory
                : Path.Combine(basePath, options.Logging.Directory);
            var logPath = Path.Combine(logDirectory, options.Logging.FileName);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(options.Logging.MinimumLevel)
                .WriteTo.File(
                    logPath,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: options.Logging.OutputTemplate)
                .CreateLogger();

            var service = new WindowsEnvironmentService(Log.Logger);
            var viewModel = new MainViewModel(options, service, Log.Logger);
            var window = new MainWindow { DataContext = viewModel };
            MainWindow = window;
            window.Show();
        }
        catch (Exception exception)
        {
            Log.Fatal(exception, Strings.LogStartupFailed);
            MessageBox.Show(
                string.Format(Strings.StartupErrorMessageFormat, exception.Message),
                Strings.StartupErrorTitle,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(1);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
