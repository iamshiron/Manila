
using System.Runtime.InteropServices.Marshalling;
using Shiron.Manila.Exceptions;
using Shiron.Manila.Utils;

namespace Shiron.Manila.API.Toolchain;

public abstract class ToolChain {
	protected Project project { get; private set; }
	protected readonly string root = Shiron.Manila.Manila.getInstance().root;

	public ToolChain(Project project) {
		this.project = project;
	}

	public void compile() {
		throw new NotImplementedException();
		/*
				var al = Shiron.Manila.Manila.getInstance().activityLogger;

				Logger.debug("BinDir: " + project.binDir);
				Logger.debug("ObjDir: " + project.objDir);
				Logger.debug("RunDir: " + project.runDir);

				var root = project._sourceSets["main"].root;
				var set = project._sourceSets["main"];
				var objFiles = new List<string>();

				var lo = new LinkerOptions() { files = objFiles };
				var co = new CompilerOptions() { includePaths = new List<string>() { root } };

				foreach (var d in project._dependencies) {
					d.resolve(co, lo);
				}

				Logger.debug("Root: " + root);
				al.compileProject(project);

				foreach (var file in set.files()) {
					var realtiveToSrc = Path.GetRelativePath(root, file);
					var objFile = project.objDir + "/" + realtiveToSrc + ".o";
					var objFileDir = Path.GetDirectoryName(objFile);

					if (!Directory.Exists(objFileDir)) Directory.CreateDirectory(objFileDir);

					al.compileFile(file);
					compileFile(file, objFile, co);
					objFiles.Add(objFile);
				}

				Logger.debug("Object Files: " + string.Join(" ", objFiles));

				if (!Directory.Exists(project.binDir)) Directory.CreateDirectory(project.binDir);
				string outPath;
				if (project.appliedComponents.Contains("manila.console")) {
					outPath = linkConsole(lo);
				} else if (project.appliedComponents.Contains("manila.staticlib")) {
					outPath = linkStaticLib(lo);
				} else {
					throw new BuildException("No target specified");
				}

				al.compileProjectEnd(project);
				*/
	}

	public abstract void preBuild();
	public abstract void postBuild();
	public abstract void compileFile(string fileIn, string fileOut, CompilerOptions o);
	public abstract string linkConsole(LinkerOptions o);
	public abstract string linkStaticLib(LinkerOptions o);
}
