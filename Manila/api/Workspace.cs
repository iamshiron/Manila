using Shiron.Manila.Exceptions;
using Shiron.Manila.Logging;
using Shiron.Manila.Utils;

namespace Shiron.Manila.API;

/// <summary>
/// Represents a workspace containing modules.
/// </summary>
public class Workspace : Component {
    public Dictionary<string, Module> Modules { get; } = new();
    public List<Tuple<ModuleFilter, Action<Module>>> ModuleFilters { get; } = new();

    public Workspace(string location) : base(location) {
    }

    /// <summary>
    /// Gets the task inside the workspace.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public Task GetTask(string key) {
        var parts = key.Split(":");
        if (parts.Length > 1) return GetTask(Modules[parts[0]], parts[1]);
        return GetTask(this, key);
    }
    public Task GetTask(Component component, string task) {
        return component.Tasks.FirstOrDefault(t => t.Name == task) ?? throw new TaskNotFoundException(task);
    }

    public bool HasTask(string key) {
        var parts = key.Split(":");
        if (parts.Length > 1) return HasTask(Modules[parts[0]], parts[1]);
        return HasTask(this, key);
    }
    public bool HasTask(Component component, string key) {
        return component.Tasks.FirstOrDefault(t => t.Name == key) != null;
    }

    public override string GetIdentifier() {
        return "";
    }

    public override string ToString() {
        return $"Workspace()";
    }
}
