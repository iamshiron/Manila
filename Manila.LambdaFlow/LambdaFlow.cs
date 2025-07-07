using System.Reflection;
using System.Text.Json;
using Shiron.Manila.Logging;

namespace Shiron.Manila.LambdaFlow;

/// <summary>
/// Represents a single recorded method call.
/// This interface defines the essential properties for a call that can be replayed.
/// </summary>
public interface ICall {
    /// <summary>
    /// The name of the method that was called.
    /// Storing the name is more robust for serialization than the MethodInfo object itself.
    /// </summary>
    string MethodName { get; }

    /// <summary>
    /// The arguments that were passed to the method.
    /// These will be used when replaying the call.
    /// </summary>
    object[] Arguments { get; }
}

/// <summary>
/// Defines the main contract for managing a recordable and replayable flow.
/// Users will primarily interact with this interface.
/// </summary>
/// <typeparam name="T">The interface that defines the API to be recorded.</typeparam>
public interface IFlow<T> where T : class {
    /// <summary>
    /// Gets the proxy instance of the interface T.
    /// All method calls made on this proxy will be recorded.
    /// </summary>
    T Proxy { get; }

    /// <summary>
    /// Gets the list of recorded calls.
    /// </summary>
    IReadOnlyList<ICall> RecordedCalls { get; }

    /// <summary>
    /// Replays the recorded sequence of calls on a given implementation.
    /// </summary>
    /// <param name="targetImplementation">The object to execute the recorded calls against.</param>
    void Replay(T targetImplementation);

    /// <summary>
    /// Serializes the recorded calls to a string format (e.g., JSON).
    /// </summary>
    /// <returns>A string representing the serialized call list.</returns>
    string Serialize();

    /// <summary>
    /// Deserializes a string into a list of calls and loads them into the flow.
    /// </summary>
    /// <param name="serializedData">The string containing the serialized call data.</param>
    void Deserialize(string serializedData);
}

/// <summary>
/// A concrete, serializable implementation of ICall.
/// This class stores the information for a single method invocation.
/// </summary>
[Serializable]
public class Call : ICall {
    /// <summary>
    /// The name of the method.
    /// </summary>
    public string MethodName { get; set; }

    /// <summary>
    /// The arguments passed to the method.
    /// </summary>
    public object[] Arguments { get; set; }

    // Parameterless constructor for serialization
    public Call() { }

    public Call(string func, object[] arguments) {
        MethodName = func;
        Arguments = arguments;
    }
}

/// <summary>
/// The main class that manages the recording and replaying of method calls for a given interface.
/// </summary>
/// <typeparam name="T">The interface to proxy and record.</typeparam>
/// <typeparam name="I">The implementation of the acutal API</typeparam>
public class Flow<T, I> : IFlow<T> where T : class {
    private readonly List<ICall> _calls = new List<ICall>();
    private readonly T _implementation;

    /// <summary>
    /// The proxy instance that records method calls.
    /// </summary>
    public T Proxy { get; }

    /// <summary>
    /// A read-only view of the recorded calls.
    /// </summary>
    public IReadOnlyList<ICall> RecordedCalls => _calls.AsReadOnly();

    /// <summary>
    /// Initializes a new Flow with a concrete implementation.
    /// </summary>
    /// <param name="implementation">The actual implementation of the interface T.</param>
    public Flow(T implementation) {
        if (!typeof(T).IsInterface) {
            throw new ArgumentException("T must be an interface type.");
        }
        _implementation = implementation;
        Proxy = RecordingProxy<T, I>.Create(_implementation, _calls);
    }

    /// <summary>
    /// Replays the recorded calls on a target implementation.
    /// </summary>
    /// <param name="targetImplementation">The object instance to replay the calls on.</param>
    public void Replay(T targetImplementation) {
        var type = typeof(I);
        foreach (var call in _calls) {
            // Find a method that matches the name and parameter types.
            // This is a simplified approach. For overloaded methods, a more robust
            // signature matching would be needed.
            var methodInfo = type.GetMethod(call.MethodName);

            if (methodInfo != null) {
                methodInfo.Invoke(targetImplementation, call.Arguments.Select((i) => { return ((JsonElement) i).GetInt32(); }).Select(v => (object) v).ToArray());
            } else {
                // Handle case where method is not found
                throw new InvalidOperationException($"Method '{call.MethodName}' not found on type '{type.FullName}' with matching argument types.");
            }
        }
    }

    /// <summary>
    /// Serializes the recorded calls to a JSON string.
    /// </summary>
    public string Serialize() {
        // We need to use JsonSerializerOptions to handle the interface type ICall.
        // A simple approach is to cast to the concrete type before serializing.
        var concreteCalls = _calls.Cast<Call>().ToList();
        return JsonSerializer.Serialize(concreteCalls, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Deserializes a JSON string and loads the calls.
    /// </summary>
    public void Deserialize(string serializedData) {
        var deserializedCalls = JsonSerializer.Deserialize<List<Call>>(serializedData);
        _calls.Clear();
        if (deserializedCalls != null) {
            _calls.AddRange(deserializedCalls);
        }
    }
}

/// <summary>
/// A dynamic proxy that intercepts method calls to an interface, records them,
/// and then forwards the call to the actual implementation.
/// </summary>
/// <typeparam name="T">The interface to proxy.</typeparam>
public class RecordingProxy<T, I> : DispatchProxy where T : class {
    private T _implementation;
    private List<ICall> _calls;

    /// <summary>
    /// Creates a new proxy instance.
    /// </summary>
    /// <param name="implementation">The concrete object that implements the interface.</param>
    /// <param name="calls">The list to which recorded calls will be added.</param>
    /// <returns>A proxy of type T.</returns>
    public static T Create(T implementation, List<ICall> calls) {
        object proxy = Create<T, RecordingProxy<T, I>>();
        ((RecordingProxy<T, I>) proxy).SetParameters(implementation, calls);
        return (T) proxy;
    }

    private void SetParameters(T implementation, List<ICall> calls) {
        _implementation = implementation;
        _calls = calls;
    }

    /// <summary>
    /// This method is invoked for every call on the proxy instance.
    /// </summary>
    /// <param name="targetMethod">The method being called.</param>
    /// <param name="args">The arguments passed to the method.</param>
    /// <returns>The result from the actual method invocation.</returns>
    protected override object Invoke(MethodInfo targetMethod, object[] args) {
        // Record the call
        _calls.Add(new Call(targetMethod.Name, args));

        // Forward the call to the real implementation
        return targetMethod.Invoke(_implementation, args);
    }
}
