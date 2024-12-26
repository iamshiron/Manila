using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;

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
		engine.AddHostObject("Manila", new API.Manila(this));
		engine.AddHostObject("print", Logger.scriptLog);
	}

	public void execute() {
		engine.Execute(File.ReadAllText(path));
	}
}
