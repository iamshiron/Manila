using Shiron.Manila.Exceptions;
using Shiron.Manila.Utils;

namespace Shiron.Manila.API;

/// <summary>
/// Represents a workspace containing projects.
/// </summary>
public class Workspace : Component {
    public Dictionary<string, Project> Projects { get; } = new();
    public List<Tuple<ProjectFilter, Action<Project>>> ProjectFilters { get; } = new();

    public Workspace(string location) : base(location) {
    }

    /// <summary>
    /// Gets the task inside the workspace.
    /// If the key starts with a colon, it is treated as a absolute path to the task.
    /// If the key does not start with a colon, it is treated as a relative path to the task inside the project.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public Task GetTask(string key) {
        Logger.Debug("Key: '" + key + "' - " + string.Join(", ", Projects.Keys));

        if (!key.StartsWith(":")) return GetTask(key, Projects.FirstOrDefault().Value);
        key = key[1..]; // Strip the colon that was just added to mark it as absolute path
        if (!key.Contains(":")) return GetTask(key, Projects.FirstOrDefault().Value);
        var parts = key.Split(":");
        return GetTask(parts[1], Projects[parts[0]]);
    }
    public Task GetTask(string task, Component? component = null) {
        if (component == null) component = this;
        Logger.Debug("Getting task: " + task + " from " + component.Path + " Available tasks: " + string.Join(", ", component.tasks.Select(t => t.name)));

        return component.tasks.FirstOrDefault(t => t.name == task) ?? throw new TaskNotFoundException(task);
    }

    public override string GetIdentifier() {
        return "";
    }
}
