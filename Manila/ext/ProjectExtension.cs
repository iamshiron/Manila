
using Shiron.Manila.Ext;

public abstract class ProjectExtension : PluginComponent {
	public readonly ManilaPlugin plugin;
	public readonly string id;

	public ProjectExtension(ManilaPlugin plugin, string id) {
		this.plugin = plugin;
		this.id = id;
	}

	public string getQualifier() {
		return plugin.group + ":" + plugin.name + ":" + id;
	}

	public string getID() {
		return id;
	}
}
