
using Shiron.Manila.API;
using Shiron.Manila.Utils;

namespace Shiron.Manila.API.Toolchain;

public class Clang : ToolChain {
	private static readonly string commandPrefix = "clang++";

	public Clang(Project project) : base(project) { }

	public override void preBuild() {
		Logger.debug("Clang preBuild");
	}
	public override void postBuild() {
		Logger.debug("Clang postBuild");
	}

	public override string compileFile(string file) {
		var realtiveToSrc = Path.GetRelativePath(root, file);
		var objFile = project.objDir + "/" + realtiveToSrc + ".o";
		run("-c", file, "-o", objFile);

		return objFile;
	}

	public override string linkFiles(string[] files) {
		var outPath = project.binDir + "/" + project.name + ".exe";
		run("-o", outPath, string.Join(" ", objFiles));
		return outPath;
	}

	public void run(params string[] args) {
		ProcessUtils.runCommand(commandPrefix, args, null, Console.WriteLine);
	}
}
