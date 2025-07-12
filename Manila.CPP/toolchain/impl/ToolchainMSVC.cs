using Shiron.Manila.API;

namespace Shiron.Manila.CPP.Toolchain.Impl;

public class ToolchainMSVC : Toolchain {
    public ToolchainMSVC(Workspace workspace, Module module, BuildConfig config) : base(workspace, module, config) {
    }

    public override void Build(Workspace workspace, Module module, BuildConfig config) {
        var instance = ManilaCPP.Instance;
        instance.Info($"Building {module.Name} with MSVC toolchain.");
    }
}
