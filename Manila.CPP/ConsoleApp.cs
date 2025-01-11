using Shiron.Manila.API;
using Shiron.Manila.Ext;

namespace Shiron.Manila.ManilaCPP;

public class ConsoleApp : CppApp {
	public ConsoleApp(Project project) : base(project) {
	}

	public override string getID() {
		return "console";
	}
}
