namespace Shiron.Manila.CPP;

using Shiron.Manila.API;
using Shiron.Manila.CPP.Components;
using Shiron.Manila.Ext;
using Shiron.Manila.Attributes;

public class ManilaCPP : ManilaPlugin {
    public ManilaCPP() : base("shiron.manila", "cpp", "1.0.0", ["Shiron"], []) {
    }

    [PluginInstance]
    public static ManilaCPP Instance { get; set; }

    public override void Init() {
        Debug("Init");

        RegisterComponent(new StaticLibComponent());
        RegisterComponent(new ConsoleComponent());
        RegisterEnum<EToolChain>();
        RegisterDependency<DependencyLink>();
        RegisterDependency<DependencyModule>();
    }
    public override void Release() {
        Debug("Release");
    }

    [ScriptFunction]
    public static void Build(Workspace workspace, Module module, BuildConfig config) {
        if (module.HasComponent<StaticLibComponent>()) {
            Instance!.Debug("Building static library: " + module.Name);
            var comp = module.GetComponent<StaticLibComponent>();
            Instance.Debug("Building to: " + comp.BinDir!);
            return;
        }

        if (module.HasComponent<ConsoleComponent>()) {
            Instance!.Debug("Building console application: " + module.Name);
            var comp = module.GetComponent<ConsoleComponent>();
            Instance.Debug("Building to: " + comp.BinDir!);

            return;
        }
    }
}
