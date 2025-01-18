
using System.Reflection;

public static class FuncUtils {
	public static Delegate toDelegate(MethodInfo func, object obj) {
		var paramTypes = func.GetParameters().Select(p => p.ParameterType).ToArray();
		Type delegateType;

		// If the method returns void, use Action
		if (func.ReturnType == typeof(void)) {
			delegateType = paramTypes.Length switch {
				0 => typeof(Action),
				1 => typeof(Action<>).MakeGenericType(paramTypes),
				2 => typeof(Action<,>).MakeGenericType(paramTypes),
				3 => typeof(Action<,,>).MakeGenericType(paramTypes),
				4 => typeof(Action<,,,>).MakeGenericType(paramTypes),
				_ => throw new NotSupportedException($"Methods with {paramTypes.Length} parameters are not supported")
			};
		}
		// If the method returns a value, use Func
		else {
			var typeList = paramTypes.ToList();
			typeList.Add(func.ReturnType); // Add return type as last type parameter

			delegateType = paramTypes.Length switch {
				0 => typeof(Func<>).MakeGenericType(func.ReturnType),
				1 => typeof(Func<,>).MakeGenericType(typeList.ToArray()),
				2 => typeof(Func<,,>).MakeGenericType(typeList.ToArray()),
				3 => typeof(Func<,,,>).MakeGenericType(typeList.ToArray()),
				4 => typeof(Func<,,,,>).MakeGenericType(typeList.ToArray()),
				_ => throw new NotSupportedException($"Methods with {paramTypes.Length} parameters are not supported")
			};
		}

		try {
			return Delegate.CreateDelegate(delegateType, obj, func, throwOnBindFailure: true);
		} catch (ArgumentException ex) {
			throw new ArgumentException(
				$"Failed to create delegate for method '{func.Name}'. " +
				$"Return type: {func.ReturnType.Name}, " +
				$"Parameter types: {string.Join(", ", paramTypes.Select(t => t.Name))}",
				ex);
		}
	}
}
