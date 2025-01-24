
using System.Text;
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

	public override void compileFile(string fileIn, string fileOut, CompilerOptions o) {
		var args = new List<string>();
		foreach (var d in o.includePaths) {
			args.Add("-I" + d);
		}

		run(commandPrefix, "-c", fileIn, "-o", fileOut, string.Join(" ", args));
	}

	public override string linkConsole(LinkerOptions o) {
		var outPath = project.binDir + "/" + project.name + ".exe";

		var args = new List<string>();
		foreach (var f in o.files) {
			args.Add(f);
		}
		foreach (var l in o.libs) {
			args.Add("-l" + l);
		}
		foreach (var d in o.libPaths) {
			args.Add("-L" + d);
		}


		run(commandPrefix, "-o", outPath, string.Join(" ", args));
		return outPath;
	}
	public override string linkStaticLib(LinkerOptions o) {
		var outPath = project.binDir + "/" + project.name + ".lib";
		run("llvm-ar", "rcs", outPath, string.Join(" ", o.files));
		return outPath;
	}

	public static void run(string command, params string[] args) {
		ProcessUtils.runCommand(command, args, null, null);
	}
}
