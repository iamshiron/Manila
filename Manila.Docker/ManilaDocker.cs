using Shiron.Manila.Ext;

namespace Shiron.Manila.Docker;

public class ManilaDocker : ManilaPlugin {
	[PluginInstance]
	public static ManilaDocker instance { get; set; }

	public ManilaDocker() : base("shiron.manila", "docker", "0.0.0") {
	}

	public override void init() {
		debug("ManilaDocker.init()");

		register(typeof(Docker.Container));
	}

	public override void release() {
		debug("ManilaDocker.release()");
	}
}
