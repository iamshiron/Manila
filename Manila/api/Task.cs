using Shiron.Manila.Utils;

namespace Shiron.Manila.API;

/// <summary>
/// Represents a task in the build script.
/// </summary>
public class Task {
    public readonly string name;
    public readonly List<string> dependencies = new();
    public Action? Action { get; private set; }
    private readonly ScriptContext context;
    private readonly Component component;

    /// <summary>
    /// Get the identifier of the task.
    /// </summary>
    /// <returns>The unique identifier of the task</returns>
    public string GetIdentifier() {
        return $"{component.GetIdentifier()}:{name}";
    }

    public Task(string name, Component component, ScriptContext context) {
        this.name = name;
        this.component = component;
        this.context = context;
        this.component.tasks.Add(this);
    }

    /// <summary>
    /// Add a dependency to the task.
    /// </summary>
    /// <param name="task">The dependents task ID</param>
    /// <returns>Task instance for chaining calls</returns>
    public Task after(string task) {
        if (task.StartsWith(":")) {
            dependencies.Add(task[1..]);
        } else {
            dependencies.Add($"{component.GetIdentifier()}:{task}");
        }

        return this;
    }
    /// <summary>
    /// The action to be executed by the task.
    /// </summary>
    /// <param name="action">The action</param>
    /// <returns>Task instance for chaining calls</returns>
    public Task execute(dynamic action) {
        this.Action = () => {
            try {
                action();
            } catch (Exception e) {
                Logger.error("Task failed: " + name);
                Logger.error(e.GetType().Name + ": " + e.Message);
                throw;
            }
        };
        return this;
    }

    /// <summary>
    /// Gets the execution order of the task and its dependencies.
    /// </summary>
    /// <returns>A ascending list of the task execution order</returns>
    public List<string> GetExecutionOrder() {
        List<string> result = [];
        foreach (string dependency in dependencies) {
            Task? dependentTask = ManilaEngine.GetInstance().Workspace.GetTask(dependency);
            if (dependentTask == null) { Logger.warn("Task not found: " + dependency); continue; }
            List<string> dependencyOrder = dependentTask.GetExecutionOrder();
            foreach (string depTask in dependencyOrder) {
                if (!result.Contains(depTask)) {
                    result.Add(depTask);
                }
            }

        }
        if (!result.Contains(GetIdentifier())) {
            result.Add(GetIdentifier());
        }

        return result;
    }
}
