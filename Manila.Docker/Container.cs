
using Shiron.Manila.API;
using Shiron.Manila.Ext;

namespace Shiron.Manila.Docker;

public class Container : ProjectApplicable {
	private ManilaDocker plugin = ManilaDocker.instance;

	public Container(Project project) : base(project) {
	}

	public override string getID() {
		return "container";
	}

	[ScriptFunction]
	public void docker() {
		plugin.debug("Docker Container");
	}
}
