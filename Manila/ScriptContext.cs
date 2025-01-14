using System.Reflection;
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
	public API.Project project { get; private set; }

	private readonly Action<object[]> scriptLogger;


	public ScriptContext(API.Project project, Manila manila, string scriptPath, Action<object[]> scriptLogger) {
		instance = manila;
		path = scriptPath;
		engine = new V8ScriptEngine();
		this.project = project;
		this.scriptLogger = scriptLogger;
	}


	public void init() {
		Logger.debug("Initializing script context...");

		engine.AddHostObject("Manila", new API.Manila(this));
		engine.AddHostObject("print", (params object[] args) => {
			scriptLogger(args);
		});


		foreach (var plugin in ExtensionAPI.getInstance().plugins) {
			Logger.debug("Adding plugin: " + plugin.GetType().Name);
			engine.AddHostObject(plugin.GetType().Name, plugin);
		}

		// Add Script Attributes
		foreach (var prop in project.GetType().GetProperties()) {
			var attr = prop.GetCustomAttributes(typeof(ScriptAttribute), false);
			if (attr.Length > 0) {
				Type delegateType = typeof(Action<>).MakeGenericType(prop.PropertyType);
				var setValue = (Action<object>) ((value) => {
					prop.SetValue(this.project, value);
				});

				engine.AddHostObject(prop.Name, setValue);
			}
		}

		// Add Script Functions
		foreach (var method in project.GetType().GetMethods()) {
			var attr = method.GetCustomAttributes(typeof(ScriptFunction), false);
			if (attr.Length > 0) {
				var paramTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
				Type delegateType = paramTypes.Length switch {
					0 => typeof(Action),
					1 => typeof(Action<>).MakeGenericType(paramTypes),
					2 => typeof(Action<,>).MakeGenericType(paramTypes),
					3 => typeof(Action<,,>).MakeGenericType(paramTypes),
					4 => typeof(Action<,,,>).MakeGenericType(paramTypes),
					_ => throw new NotSupportedException($"Methods with {paramTypes.Length} parameters are not supported")
				};

				engine.AddHostObject(method.Name, Delegate.CreateDelegate(delegateType, project, method));
			}
		}
	}

	public void execute() {
		engine.Execute(System.IO.File.ReadAllText(path));
	}
}
