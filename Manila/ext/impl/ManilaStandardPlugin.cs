
using Shiron.Manila.Ext;
using Shiron.Manila.Utils;

public class ManilaStandardPlugin : ManilaPlugin {
	[PluginInstance]
	public static ManilaStandardPlugin? instance { get; private set; } = null;

	public ManilaStandardPlugin() : base("shiron.manila", "manila", "1.0.0") {
	}

	public override void init() {
		debug("ManilaStandardPlugin initialized");

		register(typeof(CppConsole));
	}

	public override void release() {
		debug("ManilaStandardPlugin released");
	}
}
