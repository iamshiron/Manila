
using System.Runtime.InteropServices.Marshalling;
using Shiron.Manila.Utils;

namespace Shiron.Manila.API.Toolchain;

public abstract class ToolChain {
	protected Project project { get; private set; }
	protected readonly string root = Shiron.Manila.Manila.getInstance().root;

	public ToolChain(Project project) {
		this.project = project;
	}

	public void compile() {
		Logger.debug("BinDir: " + project.binDir);
		Logger.debug("ObjDir: " + project.objDir);
		Logger.debug("RunDir: " + project.runDir);

		var root = project._sourceSets["main"].root;
		var set = project._sourceSets["main"];
		var objFiles = new List<string>();

		Logger.debug("Root: " + root);

		foreach (var file in set.files()) {
			Logger.debug("File: " + file);
			var objFile = compileFile(file);
			objFiles.Add(objFile);

			var objFileDir = Path.GetDirectoryName(objFile);
			if (!Directory.Exists(objFileDir)) Directory.CreateDirectory(objFileDir);
		}

		Logger.debug("Object Files: " + string.Join(" ", objFiles));

		if (!Directory.Exists(project.binDir)) Directory.CreateDirectory(project.binDir);
		var outPath = linkFiles(objFiles.ToArray());
	}

	public abstract void preBuild();
	public abstract void postBuild();
	public abstract string compileFile(string file);
	public abstract string linkFiles(string[] files);
}
