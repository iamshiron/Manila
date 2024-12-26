using Shiron.Manila.Utils;

namespace Shiron.Manila.API;

public class Task {
	private readonly string name;
	private readonly List<string> dependencies = new();
	private Action? action;
	private readonly ScriptContext context;
	private readonly Project project;

	public Task(ScriptContext context, Project project, string name) {
		this.name = name;
		this.project = project;
		this.context = context;

		Logger.debug("Task registered: " + this.name);

		project.tasks.Add(name, this);
	}

	public Task after(string taskName) {
		if (taskName.StartsWith(':')) dependencies.Add(Path.GetDirectoryName(Path.GetRelativePath(context.instance.root, context.path)).ToLower() + taskName);
		else dependencies.Add(taskName);
		return this;
	}

	public Task execute(dynamic action) {
		this.action = () => action();
		return this;
	}

	internal void run() {
		foreach (var dep in dependencies) {
			context.instance.workspace.runTask(dep);
		}

		action?.Invoke();
	}
}
