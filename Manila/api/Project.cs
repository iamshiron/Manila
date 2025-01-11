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
}
