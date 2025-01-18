
using System.ComponentModel;
using Shiron.Manila.Ext;
using Shiron.Manila.Utils;

public class CppConsole : ProjectExtension {
	public CppConsole() : base(ManilaStandardPlugin.instance, "console") {
	}

	[ScriptFunction]
	public void test() {
		ManilaStandardPlugin.instance.debug("Hello from CppConsole");
	}
}
