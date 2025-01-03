﻿using System.Reflection;

namespace Shiron.Manila.Ext;

public class ExtensionAPI {
	private static ExtensionAPI instance;

	public string root { get; }
	public string pluginRoot { get; }
	public List<ManilaPlugin> plugins { get; private set; }

	private ExtensionAPI() {
		root = Directory.GetCurrentDirectory();
		pluginRoot = Path.Join(root, "plugins");
		plugins = new();
	}

	public void init() {
		loadPlugins();
	}

	private void loadPlugins() {
		foreach (var file in Directory.GetFiles(pluginRoot)) {
			if (!file.EndsWith(".dll")) continue;
			try {
				Assembly assembly = Assembly.LoadFrom(file);

				var pluginTypes = assembly.GetTypes()
					.Where(t => !t.IsAbstract && typeof(ManilaPlugin).IsAssignableFrom(t));
				foreach (var pluginType in pluginTypes) {
					try {
						var plugin = (ManilaPlugin) Activator.CreateInstance(pluginType);
						plugin.init();
						plugins.Add(plugin);

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

	public static ExtensionAPI getInstance() {
		if (instance == null) instance = new ExtensionAPI();
		return instance;
	}
}
