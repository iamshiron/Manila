using Microsoft.ClearScript;
using Shiron.Manila.Ext;
using Shiron.Manila.Utils;

namespace Shiron.Manila.API;

public class Manila {
	private ScriptContext context;

	public Manila(ScriptContext context) {
		this.context = context;
	}

	public Workspace getWorkspace() {
		return context.instance.workspace;
	}
	public Project getProject() {
		if (context.instance.currentProject == null) throw new Exception("Not in a project context!");

		Project project = context.instance.currentProject;
		return project;
	}
	public BuildConfig getBuildConfig() {
		return new BuildConfig();
	}

	/// <summary>
	/// Creates a new task in the current project
	/// </summary>
	/// <param name="name">The name of the task</param>
	/// <returns>The created task</returns>
	public Task task(string name) {
		return new Task(context, getProject(), name);
	}

	/// <summary>
	/// Creates a new directory object
	/// </summary>
	/// <param name="path">The path</param>
	/// <returns>The created directory object</returns>
	public Dir dir(string path) {
		return new Dir(path);
	}
	/// <summary>
	/// Creates a new file object
	/// </summary>
	/// <param name="path">The path</param>
	/// <returns>The created file opbject</returns>
	public File file(string path) {
		return new File(path);
	}

	/// <summary>
	/// Applies a plugin to the current project
	/// </summary>
	/// <param name="name">The plugin name</param>
	public void apply(string name) {
		Logger.debug("Applying: " + name);
	}

	/// <summary>
	/// Create a new source set at the root directory
	/// </summary>
	/// <param name="d">The directories to automatically include</param>
	/// <returns>A newly created source set</returns>
	public SourceSet sourceSet(params object[] d) {
		SourceSet set = new();
		foreach (object o in d) {
			if (o is string) set.include((string) o);
			else if (o is Dir) set.include((Dir) o);
			else throw new Exception("Invalid type");
		}
		return set;
	}
}
