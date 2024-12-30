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
		Logger.debug("Applying " + getID() + " to project " + project.name);

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
				var paramTypes = parameters.Select(p => p.ParameterType).ToArray();
				Type delegateType = paramTypes.Length switch {
					0 => typeof(Action),
					1 => typeof(Action<>).MakeGenericType(paramTypes),
					2 => typeof(Action<,>).MakeGenericType(paramTypes),
					3 => typeof(Action<,,>).MakeGenericType(paramTypes),
					4 => typeof(Action<,,,>).MakeGenericType(paramTypes),
					_ => throw new NotSupportedException($"Methods with {paramTypes.Length} parameters are not supported")
				};
				project.addMethod(method.Name, Delegate.CreateDelegate(delegateType, this, method));
			}
		}
	}
}
