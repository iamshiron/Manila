using Shiron.Manila.Utils;

namespace Shiron.Manila.API;

public class BuildConfig {
	public string config { get; set; }
	public EPlatform platform { get; set; }
	public EArchitecture architecture { get; set; }

	// Functions that are mostly used by scripts\
	public string getConfig() {
		return config;
	}
	public string getPlatform() {
		return platform;
	}
	public string getArchitecture() {
		return architecture;
	}
}
