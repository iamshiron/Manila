using Shiron.Manila.API;
using Shiron.Manila.Ext;

namespace Shiron.Manila.ManilaCPP;

public class ConsoleApp : ProjectApplicable {
	public List<string> defines { get; protected set; } = new();
	public string binDir { get; protected set; }
	public string objDir { get; private set; }
	public string runDir { get; private set; }

	public ConsoleApp(Project project) : base(project) {
	}

	[ScriptFunction]
	public void build() {
		Console.WriteLine("Building CPP...");
		Console.WriteLine(this.project.name);

		foreach (var define in defines) {
			Console.WriteLine("Defining: " + define);
		}
	}

	[ScriptFunction]
	public void define(string define) {
		Console.WriteLine("Adding define: " + define);
		defines.Add(define);
	}

	public override string getID() {
		return "console";
	}
}
