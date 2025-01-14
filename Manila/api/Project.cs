using System.Dynamic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Remoting;
using System.Text;
using Microsoft.ClearScript;
using Shiron.Manila.Ext;
using Shiron.Manila.Utils;

namespace Shiron.Manila.API;

public class Project : DynamicObject, IScriptableObject {
	public string name { get; private set; }
	public Dir path { get; private set; }

	public Dictionary<string, Task> tasks { get; } = new();
	public List<string> appliedComponents { get; } = new();
	public List<string> appliedPlugins { get; } = new();
	public Dictionary<string, Delegate> dynamicMethods { get; } = new();

	[ScriptAttribute]
	public string language { get; private set; } = "cpp";
	[ScriptAttribute]
	public string cppStandard { get; private set; } = "c++17";
	[ScriptAttribute]
	public string version { get; private set; } = "1.0.0";
	[ScriptAttribute]
	public string description { get; private set; } = "";

	/// <summary>
	/// Creates a new project
	/// </summary>
	/// <param name="name">The name</param>
	/// <param name="location">The absolute location</param>
	public Project(string name, string location) {
		path = new Dir(location);
		if (!path.exists()) throw new Exception($"Directory '{location}' does not exist.");
		if (!path.isAbsolute()) throw new Exception($"Directory '{location}' is not an absolute path.");

		this.name = name;
	}

	public Dir getPath() {
		return path;
	}

	/* Internal API that is used by Manila */
	public void addMethod(string name, Delegate impl) {
		if (dynamicMethods.ContainsKey(name)) { Logger.warn($"Method '{name}' already exists, overwriting."); return; } // Temporary to check if I can just ignore that a duplicate method has been added
		dynamicMethods.Add(name, impl);
	}

	public virtual void runTask(string name) {
		if (tasks.TryGetValue(name, out var task)) {
			task.run();
		} else {
			throw new Exception($"Task '{name}' not found.");
		}
	}

	public void OnExposedToScriptCode(ScriptEngine engine) {
	}

	public override IEnumerable<string> GetDynamicMemberNames() {
		Logger.debug("GetDynamicMemberNames");
		return dynamicMethods.Keys;
	}
	public override bool TryInvokeMember(InvokeMemberBinder binder, object?[] args, out object result) {
		Logger.debug($"TryInvokeMember: {binder.Name}");

		if (dynamicMethods.TryGetValue(binder.Name, out var method)) {
			Logger.debug($"Invoking method '{binder.Name}'");

			var methodParams = method.Method.GetParameters();
			for (int i = 0; i < methodParams.Length; ++i) {
				var param = methodParams[i];
				Logger.debug($"Parameter: {param.Name}");

				// Convert enum strings to enum values
				if (param.ParameterType.IsEnum) {
					var type = param.ParameterType;
					args[i] = Enum.Parse(type, args[i].ToString());
				}
			}

			result = method.DynamicInvoke(args);
			return true;
		}

		Logger.debug($"Method '{binder.Name}' not found.");
		return base.TryInvokeMember(binder, args, out result);
	}

	[ScriptFunction]
	public void sourceSets(object obj) {
		Logger.info("sourceSets");
		Console.WriteLine(obj.GetType());

		Dictionary<string, SourceSet> sets = new();
		foreach (var pair in (IDictionary<string, object>) obj) {
			sets.Add(pair.Key, (SourceSet) pair.Value);
		}

		foreach (var pair in sets) {
			Logger.info($"SourceSet: {pair.Key}");
			Logger.info(pair.Value.fileGlobs);

			foreach (var file in pair.Value.files()) {
				Logger.info($"File: {file.path}");
			}
		}
	}

	public Dir getLocation() {
		return path;
	}
}
