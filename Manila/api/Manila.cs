using System.Diagnostics;
using Microsoft.ClearScript;
using Shiron.Manila.API.Toolchain;
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
		Logger.debug("Applying plugin: " + name);
		string[] sub = name.Split(":");
		foreach (var plugin in ExtensionAPI.getInstance().plugins) {
			if (plugin.group.Equals(sub[0]) && plugin.name.Equals(sub[1])) {
				foreach (var component in plugin.components) {
					var ext = (ProjectExtension) Activator.CreateInstance(component);
					if (!ext.getID().Equals(sub[2])) continue;
					Logger.debug("Applying plugin component: " + ext.getQualifier());
					this.context.project.apply(ext);
					this.context.project.flush();
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
			new Clang().compile(project);
		} else {
			throw new Exception("Unsupported toolchain: " + project.toolchain);
		}
	}
	public void run(Project project) {
		Logger.debug("Running project: " + project.name);
		Logger.debug("BinDir: " + project.binDir);
		Logger.debug("RunDir: " + project.runDir);

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
		}
	}
}
