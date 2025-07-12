namespace Shiron.Manila.CPP;

using Shiron.Manila.API;
using Shiron.Manila.CPP.Components;

public class DependencyLink : Dependency {
    public string Path { get; private set; } = string.Empty;

    public DependencyLink() : base("link") {
    }

    public override void Create(params object[] args) {
        if (args.Length != 1) throw new Exception("Link dependency requires one argument");
        if (args[0] is not string) throw new Exception("Link dependency requires a string argument");
        this.Path = (string) args[0];
    }

    public override void Resolve(Module module) {
        throw new Exception("Link dependencies are not supported yet.");
    }
}

public class DependencyModule : Dependency {
    public UnresolvedModule Module { get; private set; } = null!;
    public string BuildTask { get; private set; } = string.Empty;

    public DependencyModule() : base("module") {
    }

    public override void Create(params object[] args) {
        if (args.Length != 2) throw new Exception("Module dependency requires two arguments");
        if (args[0] is not string || args[1] is not string) throw new Exception("Module dependency requires 2 string arguments");
        this.Module = new UnresolvedModule((string) args[0]);
        this.BuildTask = (string) args[1];
    }

    public override void Resolve(Module dependent) {
        ManilaCPP.Instance.Info(string.Join(", ", ManilaEngine.GetInstance().Workspace.Modules.Keys));

        var dependency = this.Module.Resolve();
        ManilaCPP.Instance.Info("Resolving Dependency Module '" + dependency.GetIdentifier() + "'...");

        var task = dependency.Workspace.GetTask(dependency, BuildTask);
        if (task == null) throw new Exception("Task not found: " + BuildTask);

        task.Execute(); // Still need to find a better way

        var depComp = dependency.GetComponent<CppComponent>();
        var comp = dependent.GetComponent<CppComponent>();
        comp.IncludeDirs.Add(Path.Join(dependency._sourceSets["main"].Root));
        comp.Links.AddRange([.. depComp.Links, Utils.GetBinFile(dependency, depComp)]);
    }
}
