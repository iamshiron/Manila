using Shiron.Manila.Ext;

namespace Shiron.Manila.CPP.Components;

public class ConsoleExtension : CppExtension {
	public override string getID() {
		return "console";
	}

	[ScriptAttribute]
	public Dir runDir { get; set; }
}
