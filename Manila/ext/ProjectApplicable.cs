using Shiron.Manila.API;
using Shiron.Manila.Utils;

namespace Shiron.Manila.Ext;

public abstract class ProjectApplicable : PluginComponent {
	protected Project project { get; private set; }

	protected ProjectApplicable(Project project) {
		this.project = project;
	}

	public abstract string getID();

	public virtual void onApply(ScriptContext context, Project project) {
		var methods = GetType()
			.GetMethods()
			.Where(m => m.GetCustomAttributes(typeof(ScriptFunction), true).Length > 0)
			.Concat(GetType()
				.BaseType?
				.GetMethods()
				.Where(m => m.GetCustomAttributes(typeof(ScriptFunction), true).Length > 0)
				?? Array.Empty<System.Reflection.MethodInfo>());

		foreach (var method in methods) {
			Logger.debug("Checking method: " + method.Name);
			var parameters = method.GetParameters();
			if (method.ReturnType == typeof(void)) {
				Logger.debug("Adding method: " + method.Name);
				var delegateType = parameters.Length switch {
					0 => typeof(Action),
					1 => typeof(Action<>).MakeGenericType(parameters[0].ParameterType),
					2 => typeof(Action<,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType),
					_ => null
				};
				if (delegateType != null) {
					context.engine.AddHostObject(method.Name, method.CreateDelegate(delegateType, this));
				}
			}
		}
	}
}
