using Shiron.Manila.Utils;

namespace Shiron.Manila.API;

public interface IDependency {
	public abstract void resolve();
}

public class DependencyStaticCompile : IDependency {
	public readonly UnresolvedProject project;

	public DependencyStaticCompile(UnresolvedProject project) {
		this.project = project;
	}

	public void resolve() {
		var project = this.project.resolve();
		Logger.debug("Resolving static compile dependency '" + project.name + "'");
	}
}

public class DependencyStaticLink : IDependency {
	public readonly string libFile;

	public DependencyStaticLink(string libFile) {
		this.libFile = libFile;
	}

	public void resolve() {
		Logger.debug("Resolving static link dependency '" + libFile + "'");
	}
}
