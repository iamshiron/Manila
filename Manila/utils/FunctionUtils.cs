
using System.Reflection;

namespace Shiron.Manila.Utils;

public static class FunctionUtils {
	public static Delegate createDelegate(MethodInfo m, object instance) {
		var paramTypes = m.GetParameters().Select(p => p.ParameterType).ToArray();
		var delegateType = paramTypes.Length switch {
			0 => typeof(Action),
			1 => typeof(Action<>).MakeGenericType(paramTypes),
			2 => typeof(Action<,>).MakeGenericType(paramTypes),
			3 => typeof(Action<,,>).MakeGenericType(paramTypes),
			4 => typeof(Action<,,,>).MakeGenericType(paramTypes),
			_ => throw new NotSupportedException($"Methods with {paramTypes.Length} parameters are not supported")
		};

		return Delegate.CreateDelegate(delegateType, instance, m);
	}

	public static Delegate createSetter(PropertyInfo p, object instance) {
		var delegateType = typeof(Action<>).MakeGenericType(p.PropertyType);
		return Delegate.CreateDelegate(delegateType, instance, p.GetSetMethod());
	}
	public static Delegate createGetter(PropertyInfo p, object instance) {
		var delegateType = typeof(Func<>).MakeGenericType(p.PropertyType);
		return Delegate.CreateDelegate(delegateType, instance, p.GetGetMethod());
	}
}
