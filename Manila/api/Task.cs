using Shiron.Manila.Exceptions;
using Shiron.Manila.Utils;

namespace Shiron.Manila.API;

public class Task {
	public readonly string name;
	public readonly List<string> dependencies = new();
	private Action? action;
	private readonly ScriptContext context;
	private readonly Project project;

	public static Task? currentTask { get; private set; } = null;

	public Task(ScriptContext context, Project project, string name) {
		this.name = name;
		this.project = project;
		this.context = context;

		Logger.debug("Task registered: " + this.name);

		project.tasks.Add(name, this);
	}

	public Task after(string taskName) {
		if (context.instance.currentProject is Workspace) {
			dependencies.Add(taskName[1..]);
			return this;
		}
		if (taskName.StartsWith(':')) dependencies.Add(Path.GetDirectoryName(Path.GetRelativePath(context.instance.root, context.path)).ToLower() + taskName);
		else dependencies.Add(taskName);
		return this;
	}

	public Task execute(dynamic action) {
		this.action = () => {
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

	public void run(bool runDependencies = true) {
		Shiron.Manila.Manila.getInstance().activityLogger.task(this);
		currentTask = this;
		if (runDependencies) foreach (var dep in dependencies) context.instance.workspace.runTask(dep);

		try {
			action?.Invoke();
		} catch (CompileException) {
			throw;
		}
		currentTask = null;
		Shiron.Manila.Manila.getInstance().activityLogger.taskEnd(this);
	}

	public string getQualifiedName() {
		return Path.GetDirectoryName(Path.GetRelativePath(context.instance.root, context.path)).ToLower().Replace("/", ":").Replace("\\", ":") + ":" + name;
	}
}
