
using Shiron.Manila.API;
using Shiron.Manila.Utils;

namespace Shiron.Manila.API.Toolchain;

public class Clang : IToolChain {
	private static readonly string commandPrefix = "clang++";

	public Clang() { }

	public void compile(Project project) {
		Logger.debug("Compiling project with Clang");
		Logger.debug("BinDir: " + project.binDir);
		Logger.debug("ObjDir: " + project.objDir);
		Logger.debug("RunDir: " + project.runDir);

		var root = Shiron.Manila.Manila.getInstance().root;
		var set = project._sourceSets["main"];
		var objFiles = new List<string>();

		foreach (var file in set.files()) {
			var realtiveToSrc = Path.GetRelativePath(root, file);
			var objFile = project.objDir + "/" + realtiveToSrc + ".o";
			var objFileDir = Path.GetDirectoryName(objFile);
			objFiles.Add(objFile);

			if (!Directory.Exists(objFileDir)) Directory.CreateDirectory(objFileDir);

			run("-c", file, "-o", objFile);
		}

		Logger.debug("Object Files: " + string.Join(" ", objFiles));

		if (!Directory.Exists(project.binDir)) Directory.CreateDirectory(project.binDir);
		run("-o", project.binDir + "/" + project.name + ".exe", string.Join(" ", objFiles));
	}

	public void run(params string[] args) {
		runCommand(commandPrefix, args);
	}
	public void runCommand(string command, params string[] args) {
		Logger.debug("Running command: " + command + " " + string.Join(" ", args));

		var process = new System.Diagnostics.Process();
		process.StartInfo.FileName = command;
		process.StartInfo.Arguments = string.Join(" ", args);
		process.StartInfo.UseShellExecute = false;
		process.Start();
		process.WaitForExit();
	}
}
