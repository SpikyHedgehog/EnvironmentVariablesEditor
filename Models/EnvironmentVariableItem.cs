using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EnvironmentVariablesEditor.Models;

public sealed class EnvironmentVariableItem : INotifyPropertyChanged
{
    private string _value;

    public EnvironmentVariableItem(string name, string value, string comment)
    {
        Name = name;
        _value = value;
        Comment = comment;
        OriginalValue = value;
    }

    public string Name { get; }

    public string Value
    {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsChanged));
        }
    }

    public string Comment { get; }
    public string OriginalValue { get; private set; }
    public bool IsChanged => !string.Equals(Value, OriginalValue, StringComparison.Ordinal);

    public void AcceptChanges()
    {
        OriginalValue = Value;
        OnPropertyChanged(nameof(IsChanged));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
