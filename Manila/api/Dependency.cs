using Shiron.Manila.Utils;
using Shiron.Manila.API.Toolchain;
using Shiron.Manila.Exceptions;

namespace Shiron.Manila.API;

public interface IDependency {
	public abstract void resolve(CompilerOptions co, LinkerOptions lo);
}

public class DependencyStaticCompile : IDependency {
	public readonly UnresolvedProject project;

	public DependencyStaticCompile(UnresolvedProject project) {
		this.project = project;
	}

	public void resolve(CompilerOptions co, LinkerOptions lo) {
		var project = this.project.resolve();
		Logger.debug("Resolving static compile dependency '" + project.name + "'");
		var buildTask = project.tasks["build"];
		if (buildTask == null) throw new BuildException("No build task found in project '" + project.name + "'");
		buildTask.run();
		co.includePaths.Add(project._sourceSets["main"].root);
		lo.libs.Add(project.binDir + "/" + project.name + ".lib");
	}
}

public class DependencyStaticLink : IDependency {
	public readonly string libFile;

	public DependencyStaticLink(string libFile) {
		this.libFile = libFile;
	}

	public void resolve(CompilerOptions co, LinkerOptions lo) {
		lo.libs.Add(libFile);
	}
}
