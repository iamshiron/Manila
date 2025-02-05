﻿using Shiron.Manila.Utils;
using Spectre.Console;

namespace Shiron.Manila;

public class Manila {
	private static Manila? instance;
	public AbstractActivityLogger activityLogger { get; private set; }

	public static string VERSION = "1.0.0";
	public string root { get; private set; }

	public API.Workspace workspace { get; }
	public API.Project? currentProject { get; private set; } = null;

	public bool initialized { get; private set; } = false;

	private Manila() {
		root = Directory.GetCurrentDirectory();
		workspace = new API.Workspace(root);
	}

	public void init(Action<object[]>? scriptLogger, AbstractActivityLogger activityLogger) {
		this.activityLogger = activityLogger;
		if (!System.IO.File.Exists("Manila.js")) {
			throw new Exception("No root build script found");
		}
		initialized = true;

		runScript("Manila.js", scriptLogger ?? Logger.scriptLog, true);
		var files = Directory.GetFiles(".", "Manila.js", SearchOption.AllDirectories)
			.Where(f => !Path.GetFullPath(f).Equals(Path.GetFullPath("Manila.js")))
			.ToList();

		foreach (var file in files) {
			runScript(file, scriptLogger ?? Logger.scriptLog);
		}
	}

	public static Manila getInstance() {
		if (instance == null) instance = new Manila();
		return instance;
	}

	public void runScript(string path, Action<object[]> scriptLogger, bool root = false) {
		Logger.debug("Running script: " + path);

		if (root) {
			currentProject = workspace;
		} else {
			string projectPath = Path.GetDirectoryName(Path.GetRelativePath(this.root, path));
			string name = projectPath.ToLower().Replace("/", ":").Replace("\\", ":");
			currentProject = new API.Project(name, Path.Combine(this.root, projectPath));
			workspace.projects.Add(name, currentProject);
		}

		ScriptContext context = new ScriptContext(currentProject, this, path, scriptLogger);
		context.init();
		context.execute();
		currentProject = null;
	}
}
