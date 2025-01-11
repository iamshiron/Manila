
using Shiron.Manila.API;
using Shiron.Manila.Ext;

public abstract class CppApp : ProjectApplicable {
	[ScriptEnum]
	public enum ToolChain {
		MSVC,
		GCC,
		Clang,
	}

	public CppApp(Project project) : base(project) {
	}

	public List<string> defines { get; protected set; } = new();
	public string binDir { get; protected set; }
	public string objDir { get; private set; }
	public string runDir { get; private set; }

	[ScriptFunction]
	public void toolChain(ToolChain toolChain) {
		Console.WriteLine("Setting tool chain to: " + toolChain);
	}

	[ScriptFunction]
	public void define(string define) {
		Console.WriteLine("Adding define: " + define);
		defines.Add(define);
	}

	[ScriptFunction]
	public void build() {
		Console.WriteLine("Building CPP...");
		Console.WriteLine(this.project.name);

		foreach (var define in defines) {
			Console.WriteLine("Defining: " + define);
		}
	}
}
