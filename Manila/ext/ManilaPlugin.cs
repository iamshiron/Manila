using Shiron.Manila.Utils;

namespace Shiron.Manila.Ext;

public abstract class ManilaPlugin {
	public string group { get; }
	public string name { get; }
	public string version { get; }

	public List<Type> components { get; } = new List<Type>();

	public ManilaPlugin(string group, string name, string version) {
		this.group = group;
		this.name = name;
		this.version = version;
	}

	public abstract void init();
	public abstract void release();

	public override string ToString() {
		return $"ManilaPlugin<{group}.{name}@{version}>";
	}

	public void info(params object[] message) {
		Logger.pluginInfo(this, message);
	}
	public void warn(params object[] message) {
		Logger.pluginWarn(this, message);
	}
	public void error(params object[] message) {
		Logger.pluginError(this, message);
	}
	public void debug(params object[] message) {
		Logger.pluginDebug(this, message);
	}

	protected void register(Type component) {
		bool valid = false;
		foreach (var iface in component.GetInterfaces()) {
			if (iface == typeof(PluginComponent)) {
				valid = true;
				break;
			}
		}

		if (!valid) throw new Exception($"Component {component} is not a valid PluginComponent");

		components.Add(component);
	}

	public string getQualifier() {
		return $"{group}:{name}";
	}
}
