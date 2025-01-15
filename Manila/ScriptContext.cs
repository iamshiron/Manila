using System.Reflection;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using Microsoft.VisualBasic;
using Shiron.Manila.API;
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

		Dictionary<string, Type> enums = new();

		// Add Script Attributes
		foreach (var prop in project.GetType().GetProperties()) {
			var type = prop.PropertyType;

			var attr = prop.GetCustomAttributes(typeof(ScriptAttribute), false);
			if (attr.Length > 0) {
				Type delegateType = typeof(Action<>).MakeGenericType(type);
				var setValue = (Action<object>) ((value) => {
					prop.SetValue(this.project, value);
				});

				engine.AddHostObject(prop.Name, setValue);
			}

			var typeAttr = type.GetCustomAttributes(typeof(ScriptEnum), false);
			if (typeAttr.Length > 0) {
				if (!enums.ContainsKey(type.Name))
					enums.Add(type.Name, type);
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

				Logger.debug("Adding function: " + method.Name);
				engine.AddHostObject(method.Name, Delegate.CreateDelegate(delegateType, project, method));

				foreach (var param in method.GetParameters()) {
					if (param.ParameterType.IsEnum) {
						if (!enums.ContainsKey(param.ParameterType.Name))
							enums.Add(param.ParameterType.Name, param.ParameterType);
					}
				}
			}
		}

		// Add Enums
		foreach (var pair in enums) {
			Logger.info("Adding enum: " + pair.Key);
			engine.AddHostType(pair.Key[1..], pair.Value);
		}
	}

	public void execute() {
		engine.Execute(System.IO.File.ReadAllText(path));
	}
}
