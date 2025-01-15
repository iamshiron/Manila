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
	public ELanguage language { get; private set; } = ELanguage.cpp;
	[ScriptAttribute]
	public string cppStandard { get; private set; } = "c++17";
	[ScriptAttribute]
	public string version { get; private set; } = "1.0.0";
	[ScriptAttribute]
	public string description { get; private set; } = "";
	[ScriptAttribute]
	public EToolChain toolchain { get; private set; } = EToolChain.clang;

	[ScriptAttribute]
	public Dir binDir { get; private set; }
	[ScriptAttribute]
	public Dir objDir { get; private set; }
	[ScriptAttribute]
	public Dir runDir { get; private set; }

	public List<IDependency> _dependencies { get; private set; } = new();
	public Dictionary<string, SourceSet> _sourceSets { get; private set; } = new();

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
		foreach (var pair in (IDictionary<string, object>) obj) {
			if (_sourceSets.ContainsKey(pair.Key)) throw new Exception($"SourceSet '{pair.Key}' already exists.");
			_sourceSets.Add(pair.Key, (SourceSet) pair.Value);
		}
	}

	[ScriptFunction]
	public void build() {
	}

	[ScriptFunction]
	public void dependencies(dynamic deps) {
		foreach (var dep in deps) _dependencies.Add(dep);
	}

	public string getIdentifier() {
		string relativeDir = Path.GetRelativePath(Shiron.Manila.Manila.getInstance().root, path.path);
		return ":" + relativeDir.Replace(Path.DirectorySeparatorChar, ':').ToLower();
	}

	public string getName() {
		return name;
	}
}
