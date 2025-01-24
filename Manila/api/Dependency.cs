using Shiron.Manila.Utils;
using Shiron.Manila.API.Toolchain;

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
