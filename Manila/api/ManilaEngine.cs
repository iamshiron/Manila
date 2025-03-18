using System.Diagnostics;
using Shiron.Manila.API.Toolchain;
using Shiron.Manila.Ext;
using Shiron.Manila.Utils;

namespace Shiron.Manila.API;

public class ManilaEngine {
	private ScriptContext context;

	public ManilaEngine(ScriptContext context) {
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
	public UnresolvedProject getProject(string identifier) {
		return new UnresolvedProject(identifier);
	}

	public BuildConfig getBuildConfig() {
		return context.instance.workspace.buildConfig;
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
		var component = ExtensionAPI.getInstance().getLoadedComponent(name);
		if (component == null) throw new Exception("PluginComponent not found: " + name);

		apply(component);
	}
	private void apply(PluginComponent component) {
		var project = getProject();
		project.appliedComponents.Add(component.GetType(), component);

		foreach (var prop in component.GetType().GetProperties()) {
			var type = prop.PropertyType;
			var attr = prop.GetCustomAttributes(typeof(ScriptAttribute), false);

			if (attr.Length > 0) {
				Type delegateType = typeof(Action<>).MakeGenericType(type);
				var setValue = (Action<object>) ((value) => {
					Logger.debug("Setting value: " + value + " to " + prop.Name);
					prop.SetValue(component, value);
				});

				context.engine.AddHostObject(prop.Name, setValue);

				Logger.debug("Added property: " + prop.Name);
			}

			var typeAttr = type.GetCustomAttributes(typeof(ScriptEnum), false);
			if (typeAttr.Length > 0) {
				if (!context.enums.ContainsKey(type.Name)) {
					context.enums.Add(type.Name, type);
					context.engine.AddHostType(type.Name[1..], type);
				}
			}
		}
	}

	/// <summary>
	/// Create a new source set at the root directory
	/// </summary>
	/// <param name="d">The root directory of the source set</param>
	/// <returns>A newly created source set</returns>
	public SourceSet sourceSet(string root) {
		return new(root);
	}

	public DependencyStaticCompile compile(UnresolvedProject project) {
		return new(project);
	}
	public DependencyStaticLink link(string libFile) {
		return new(libFile);
	}

	public void build(Workspace workspace, Project project, BuildConfig config) {
		if (project.toolchain.Equals(EToolChain.clang)) {
			new Clang(project).compile();
		} else {
			throw new Exception("Unsupported toolchain: " + project.toolchain);
		}
	}

	public void run(UnresolvedProject project) {
		Project resolvedProject = project.resolve();
		run(resolvedProject);
	}
	public void run(Project project) {
		Logger.debug("Running project: " + project.name);
		Logger.debug("BinDir: " + project.binDir);

		var startInfo = new ProcessStartInfo() {
			FileName = project.binDir + "/" + project.name + ".exe",
			Arguments = "",
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardError = true
		};

		using (Process process = Process.Start(startInfo)) {
			process.OutputDataReceived += (sender, e) => {
				if (e.Data != null) context.scriptLog(e.Data);
			};
			process.ErrorDataReceived += (sender, e) => {
				if (e.Data != null) context.scriptLog(e.Data);
			};

			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
			process.WaitForExit();

			if (process.ExitCode != 0) throw new CommandExecutionException(startInfo.FileName, null, null, process.ExitCode);
		}
	}
}
