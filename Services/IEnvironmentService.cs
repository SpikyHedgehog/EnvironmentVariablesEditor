namespace EnvironmentVariablesEditor.Services;

public interface IEnvironmentService
{
    string GetOrInitialize(string name, string defaultValue);
    void Set(string name, string value);
    void NotifyEnvironmentChanged();
}
