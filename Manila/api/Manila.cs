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
	/// Applies a plugin to the current project
	/// </summary>
	/// <param name="name">The plugin name</param>
	public void apply(string name) {
		Logger.debug("Applying plugin: " + name);

		string pluginQualifier = name.Substring(0, name.LastIndexOf(':'));
		string componentName = name.Substring(name.LastIndexOf(':') + 1);

		Logger.debug("Plugin qualifier: " + pluginQualifier);
		Logger.debug("Component name: " + componentName);

		Project project = getProject();

		ExtensionAPI instance = ExtensionAPI.getInstance();
		foreach (ManilaPlugin plugin in instance.plugins) {
			if (plugin.getQualifier().Equals(pluginQualifier)) {
				Logger.debug("Found plugin: " + plugin.getQualifier());
				foreach (Type t in plugin.components) {
					PluginComponent comp = (PluginComponent) Activator.CreateInstance(t, project);
					string componentID = comp.getID();

					if (project.appliedComponents.Contains(componentID)) {
						Logger.debug("Component " + componentID + " already applied to project " + project.name);
						continue;
					}
					project.appliedComponents.Add(componentID);
					((ProjectApplicable) comp).onApply(context, project);
				}
			}
		}
	}
}
