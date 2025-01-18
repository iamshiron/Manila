
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

		var root = project._sourceSets["main"].root;
		var set = project._sourceSets["main"];
		var objFiles = new List<string>();

		Logger.debug("Root: " + root);

		foreach (var file in set.files()) {
			Logger.debug("File: " + file);

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
		ProcessUtils.runCommand(commandPrefix, args, null, Console.WriteLine);
	}
}
