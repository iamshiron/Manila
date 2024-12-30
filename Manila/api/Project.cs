using System.Dynamic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Microsoft.ClearScript;
using Shiron.Manila.Ext;
using Shiron.Manila.Utils;

namespace Shiron.Manila.API;

public class Project : DynamicObject, IScriptableObject {
	public string name { get; private set; }
	public Dictionary<string, Task> tasks { get; } = new();
	public List<string> appliedComponents { get; } = new();

	public Dictionary<string, Delegate> dynamicMethods { get; } = new();

	public Project(string name) {
		this.name = name;
	}

	public void addMethod(string name, Delegate impl) {
		if (dynamicMethods.ContainsKey(name)) throw new Exception($"Method '{name}' already exists.");
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
			result = method.DynamicInvoke(args);
			return true;
		}

		Logger.debug($"Method '{binder.Name}' not found.");
		return base.TryInvokeMember(binder, args, out result);
	}
}
