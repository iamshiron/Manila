
using System.Runtime.InteropServices.Marshalling;
using Shiron.Manila.Exceptions;
using Shiron.Manila.Utils;

namespace Shiron.Manila.API.Toolchain;

public abstract class ToolChain {
	public class LinkerOptions {
		public string[] files;
		public string outPath;
		public string[] libs;
		public string[] libPaths;
		public string[] includePaths;
	}

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
			var realtiveToSrc = Path.GetRelativePath(root, file);
			var objFile = project.objDir + "/" + realtiveToSrc + ".o";
			var objFileDir = Path.GetDirectoryName(objFile);

			if (!Directory.Exists(objFileDir)) Directory.CreateDirectory(objFileDir);

			compileFile(file, objFile);
			objFiles.Add(objFile);

		}

		Logger.debug("Object Files: " + string.Join(" ", objFiles));

		if (!Directory.Exists(project.binDir)) Directory.CreateDirectory(project.binDir);

		var libFiles = new List<string>();
		foreach (var d in project._dependencies) {
			libFiles.Add(d.resolve());
		}

		string outPath;
		if (project.appliedComponents.Contains("manila.console")) {
			outPath = linkConsole(new LinkerOptions {
				files = objFiles.ToArray(),
				libs = libFiles.ToArray(),
			});
		} else if (project.appliedComponents.Contains("manila.staticlib")) {
			outPath = linkStaticLib(new LinkerOptions {
				files = objFiles.ToArray()
			});
		} else {
			throw new BuildException("No target specified");
		}
	}

	public abstract void preBuild();
	public abstract void postBuild();
	public abstract void compileFile(string fileIn, string fileOut);
	public abstract string linkConsole(LinkerOptions o);
	public abstract string linkStaticLib(LinkerOptions o);
}
