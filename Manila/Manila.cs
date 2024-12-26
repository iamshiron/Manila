using Shiron.Manila.Utils;
using Spectre.Console;

namespace Shiron.Manila;

public class Manila {
	private static Manila? instance;

	public static string VERSION = "1.0.0";
	public string root { get; private set; }

	public API.Workspace workspace { get; } = new();
	public API.Project? currentProject { get; private set; } = null;

	private Manila() {
		root = Directory.GetCurrentDirectory();

		if (!File.Exists("Manila.js")) {
			throw new Exception("No root build script found");
		}

		runScript("Manila.js", true);
		var files = Directory.GetFiles(".", "Manila.js", SearchOption.AllDirectories)
			.Where(f => !Path.GetFullPath(f).Equals(Path.GetFullPath("Manila.js")))
			.ToList();

		foreach (var file in files) {
			runScript(file);
		}

		instance = this;
	}

	public static Manila getInstance() {
		if (instance == null) instance = new Manila();
		return instance;
	}

	public void runScript(string path, bool root = false) {
		Logger.debug("Running script: " + path);

		if (root) {
			currentProject = workspace;
		} else {
			string name = Path.GetDirectoryName(Path.GetRelativePath(this.root, path)).ToLower().Replace("/", ":").Replace("\\", ":");
			currentProject = new API.Project(name);
			workspace.projects.Add(name, currentProject);
		}

		ScriptContext context = new ScriptContext(this, path);
		context.init();
		context.execute();
		currentProject = null;
	}
}
