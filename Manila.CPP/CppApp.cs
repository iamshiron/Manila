
using Shiron.Manila.API;
using Shiron.Manila.Ext;
using Shiron.Manila.ManilaCPP;
using Shiron.Manila.Utils;

public abstract class CppApp : ProjectApplicable {
	protected ManilaCPP instance = ManilaCPP.instance;

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
	public string objDir { get; protected set; }
	public string runDir { get; protected set; }
	public ToolChain? toolChain1 { get; protected set; } = null;


	[ScriptFunction]
	public void toolChain(ToolChain toolChain) {
		Console.WriteLine(instance.getQualifier());

		instance.debug("Setting tool chain to: " + toolChain);
		toolChain1 = toolChain;
	}

	[ScriptFunction]
	public void define(string define) {
		instance.debug("Adding define: " + define);
		defines.Add(define);
	}

	[ScriptFunction]
	public void build() {
		instance.debug("Building CPP...");
		instance.debug(this.project.name);

		foreach (var define in defines) {
			instance.debug("Defining: " + define);
		}
	}
}
