using System.Reflection;
using System.Text.RegularExpressions;
using Shiron.Manila.Utils;

namespace Shiron.Manila.Ext;

public class ExtensionAPI {
	private static ExtensionAPI instance;

	public string root { get; }
	public string pluginRoot { get; }
	public List<ManilaPlugin> plugins { get; private set; }

	private static readonly Regex pluginPattern = new(
		@"^(?<group>[^:]+):(?<name>[^:@]+)(@(?<version>[^:]+))?$",
		RegexOptions.Compiled);
	private static readonly Regex pluginComponentPattern = new(@"^(?<group>[^:]+):(?<name>[^:@]+)(@(?<version>[^:]+))?(:(?<component>.+))?$");

	private ExtensionAPI() {
		root = Directory.GetCurrentDirectory();
		pluginRoot = Path.Join(root, "/.manila/plugins");
		plugins = new();
	}

	public void init() {
		loadPlugins();
	}

	private void loadPlugins() {
		if (!Directory.Exists(pluginRoot)) return;
		Logger.debug("Discovering plugins...");

		foreach (var file in Directory.GetFiles(pluginRoot)) {
			if (!file.EndsWith(".dll")) continue;
			try {
				Logger.debug($"Loading assembly {file}");
				Assembly assembly = Assembly.LoadFrom(file);

				var pluginTypes = assembly.GetTypes()
					.Where(t => !t.IsAbstract && typeof(ManilaPlugin).IsAssignableFrom(t));
				foreach (var pluginType in pluginTypes) {
					try {
						var plugin = (ManilaPlugin) Activator.CreateInstance(pluginType);
						plugin.init();
						plugins.Add(plugin);

						foreach (var field in pluginType.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.SetProperty)) {
							Logger.debug($"Checking field {field.Name} in {pluginType.FullName}");
							if (field.GetCustomAttribute<PluginInstance>() != null) {
								field.SetValue(plugin, plugin);
							}
						}

					} catch (Exception ex) {
						Console.WriteLine($"Failed to initialize plugin from {pluginType.FullName}: {ex.Message}");
						Console.WriteLine(ex.StackTrace);
					}
				}
			} catch (Exception ex) {
				Console.WriteLine($"Failed to load assembly {file}: {ex.Message}");
			}
		}
	}
	public void releasePlugins() {
		foreach (var plugin in plugins) {
			plugin.release();
		}
	}

	public ManilaPlugin getLoadedPlugin(string id) {
		var match = pluginPattern.Match(id);
		if (!match.Success) throw new Exception($"Invalid plugin identifier: {id}");
		return getLoadedPlugin(match.Groups["group"].Value, match.Groups["name"].Value, match.Groups["version"].Success ? match.Groups["version"].Value : null);
	}
	public ManilaPlugin getLoadedPlugin(string group, string name, string? version) {
		Logger.debug($"Searching for plugin {group}.{name}@{version}");

		var plugin = plugins.FirstOrDefault(p => p.group == group && p.name == name && (version == null || p.version == version));
		if (plugin == null) throw new Exception($"Plugin {group}.{name}@{version} not found!");
		return plugin;
	}

	public PluginComponent getLoadedComponent(string id) {
		var match = pluginComponentPattern.Match(id);

		if (!match.Success) throw new Exception($"Invalid component identifier: {id}");
		var plugin = getLoadedPlugin(match.Groups["group"].Value, match.Groups["name"].Value, match.Groups["version"].Success ? match.Groups["version"].Value : null);
		return getLoadedComponent(plugin, match.Groups["component"].Value);
	}
	public PluginComponent getLoadedComponent(ManilaPlugin plugin, string id) {
		Logger.debug($"Searching for component {id} in {plugin.group}.{plugin.name}@{plugin.version}");

		foreach (var c in plugin.components) {
			var comp = (PluginComponent) Activator.CreateInstance(c);
			if (comp.getID() == id) return comp;
		}
		throw new Exception($"Component {id} not found in plugin {plugin.group}.{plugin.name}@{plugin.version}");
	}

	public static ExtensionAPI getInstance() {
		if (instance == null) instance = new ExtensionAPI();
		return instance;
	}
}
