using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using EnvironmentVariablesEditor.Models;
using EnvironmentVariablesEditor.Localization;
using EnvironmentVariablesEditor.Services;
using Serilog;

namespace EnvironmentVariablesEditor.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly IEnvironmentService _environmentService;
    private readonly ILogger _logger;
    private string _statusText = Strings.StatusReady;
    private bool _hasChanges;

    public MainViewModel(AppOptions options, IEnvironmentService environmentService, ILogger logger)
    {
        _environmentService = environmentService;
        _logger = logger;
        Variables = new ObservableCollection<EnvironmentVariableItem>(options.EnvironmentVariables.Select(name =>
        {
            var value = environmentService.GetOrInitialize(name, options.DefaultValue);
            options.Comments.TryGetValue(name, out var comment);
            var item = new EnvironmentVariableItem(name, value, comment ?? string.Empty);
            item.PropertyChanged += ItemOnPropertyChanged;
            return item;
        }));

        SaveCommand = new RelayCommand(Save, () => HasChanges);
    }

    public ObservableCollection<EnvironmentVariableItem> Variables { get; }
    public RelayCommand SaveCommand { get; }

    public string StatusText
    {
        get => _statusText;
        private set { _statusText = value; OnPropertyChanged(); }
    }

    public bool HasChanges
    {
        get => _hasChanges;
        private set
        {
            if (_hasChanges == value) return;
            _hasChanges = value;
            OnPropertyChanged();
            SaveCommand.RaiseCanExecuteChanged();
        }
    }

    private void ItemOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(EnvironmentVariableItem.IsChanged) &&
            e.PropertyName != nameof(EnvironmentVariableItem.Value)) return;

        HasChanges = Variables.Any(item => item.IsChanged);
        StatusText = HasChanges ? Strings.StatusUnsavedChanges : Strings.StatusAllChangesSaved;
    }

    private void Save()
    {
        var changed = Variables.Where(item => item.IsChanged).ToArray();
        if (changed.Length == 0) return;

        try
        {
            foreach (var item in changed)
            {
                _environmentService.Set(item.Name, item.Value ?? string.Empty);
                item.AcceptChanges();
            }

            _environmentService.NotifyEnvironmentChanged();
            HasChanges = false;
            StatusText = string.Format(Strings.StatusSavedFormat, changed.Length);
        }
        catch (Exception exception)
        {
            _logger.Error(exception, Strings.LogSaveFailed);
            StatusText = Strings.StatusSaveFailed;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
