using Shiron.Manila.CPP.Components;

namespace Shiron.Manila.CPP;

public class ManilaCPP : Ext.ManilaPlugin {
	public ManilaCPP() : base("shiron.manila", "cpp", "1.0.0") {
	}

	public override void init() {
		debug("ManilaCPP initialized");

		register(typeof(ConsoleExtension));
		register(typeof(StaticLibExtension));
	}

	public override void release() {
		debug("ManilaCPP released");
	}
}
