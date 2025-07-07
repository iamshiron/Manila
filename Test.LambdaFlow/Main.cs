
using System.Reflection;
using Microsoft.ClearScript.V8;
using Shiron.Manila.LambdaFlow;

public interface ICalculator {
    int Value { get; }
    void Add(int number);
    void Subtract(int number);
    void Multiply(int number);
}

public class Calculator : ICalculator {
    public int Value { get; private set; }

    public void Add(int number) {
        Value += number;
        Console.WriteLine($"Implementation: Added {number}. Current Value: {Value}");
    }

    public void Subtract(int number) {
        Value -= number;
        Console.WriteLine($"Implementation: Subtracted {number}. Current Value: {Value}");
    }

    public void Multiply(int number) {
        Value *= number;
        Console.WriteLine($"Implementation: Multiplied by {number}. Current Value: {Value}");
    }
}

public static class LambdaFlowTest {
    public static int Main(string[] args) {
        Console.WriteLine("--- Step 1: Record a sequence of operations ---");

        // Create an instance of the real implementation
        var originalCalculator = new Calculator();

        // Wrap it in a Flow to start recording
        var recordingFlow = new Flow<ICalculator, Calculator>(originalCalculator);

        // Use the proxy to perform operations. These calls are recorded.
        // The output shows the implementation is being called underneath.
        recordingFlow.Proxy.Add(10);
        recordingFlow.Proxy.Add(5);
        recordingFlow.Proxy.Subtract(3);
        recordingFlow.Proxy.Multiply(2);

        Console.WriteLine($"\nFinal value after recording: {originalCalculator.Value}");

        Console.WriteLine("\n--- Step 2: Serialize the recorded flow ---");
        string serializedFlow = recordingFlow.Serialize();
        Console.WriteLine("Serialized JSON of the recorded calls:");
        Console.WriteLine(serializedFlow);

        Console.WriteLine("\n--- Step 3: Deserialize and Replay the flow ---");

        // Create a new, clean calculator instance. Its initial value is 0.
        var replayCalculator = new Calculator();
        Console.WriteLine($"Value of new calculator before replay: {replayCalculator.Value}");

        // Create a new flow and load the serialized data into it.
        // Note: We pass a dummy/null implementation here since we only need it for deserialization and replay.
        var replayFlow = new Flow<ICalculator, Calculator>(new Calculator()); // A placeholder implementation is fine here.
        replayFlow.Deserialize(serializedFlow);

        Console.WriteLine("\nReplaying the recorded calls on the new calculator instance...");
        // Replay the calls on our new calculator instance
        replayFlow.Replay(replayCalculator);

        Console.WriteLine($"\nFinal value of new calculator after replay: {replayCalculator.Value}");

        // Verification
        if (originalCalculator.Value == replayCalculator.Value) {
            Console.WriteLine("\nSuccess! The replayed state matches the original state.");
        } else {
            Console.WriteLine("\nError! The replayed state does not match the original state.");
        }

        return 0;
    }
}
