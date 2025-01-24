using Shiron.Manila.Utils;

namespace Shiron.Manila.API;

public interface IDependency {
	public abstract string resolve();
}

public class DependencyStaticCompile : IDependency {
	public readonly UnresolvedProject project;

	public DependencyStaticCompile(UnresolvedProject project) {
		this.project = project;
	}

	public string resolve() {
		var project = this.project.resolve();
		Logger.debug("Resolving static compile dependency '" + project.name + "'");
		return project.binDir + "/" + project.name + ".lib";
	}
}

public class DependencyStaticLink : IDependency {
	public readonly string libFile;

	public DependencyStaticLink(string libFile) {
		this.libFile = libFile;
	}

	public string resolve() {
		return libFile;
	}
}
