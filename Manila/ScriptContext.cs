using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using Microsoft.VisualBasic;
using Shiron.Manila.Ext;
using Shiron.Manila.Utils;

namespace Shiron.Manila;

public class ScriptContext {
	public string path { get; }
	public ScriptEngine engine { get; private set; }
	public Manila instance { get; private set; }

	public ScriptContext(Manila manila, string scriptPath) {
		instance = manila;
		path = scriptPath;
		engine = new V8ScriptEngine();
	}

	public void init() {
		Logger.debug("Initializing script context...");

		engine.AddHostObject("Manila", new API.Manila(this));
		engine.AddHostObject("print", Logger.scriptLog);

		foreach (var plugin in ExtensionAPI.getInstance().plugins) {
			Logger.debug("Adding plugin: " + plugin.GetType().Name);
			engine.AddHostObject(plugin.GetType().Name, plugin);
		}
	}

	public void execute() {
		engine.Execute(System.IO.File.ReadAllText(path));
	}
}
