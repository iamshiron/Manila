using Shiron.Manila.Ext;

namespace Manila.Docker;

public class ManilaDocker : ManilaPlugin {
	[PluginInstance]
	public static ManilaDocker instance { get; set; }

	public ManilaDocker() : base("shiron.manila", "docker", "0.0.0") {
	}

	public override void init() {
		instance.release();
		debug("ManilaDocker.init()");
	}

	public override void release() {
		debug("ManilaDocker.release()");
	}
}
