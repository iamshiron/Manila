using Shiron.Manila.API;

namespace Shiron.Manila.CPP.Toolchain.Impl;

public class ToolchainClang : Toolchain {
    public override void Build(Workspace workspace, Project project, BuildConfig config) {
        var instance = ManilaCPP.Instance;
        instance.Info($"Building {project.Name} with Clang toolchain.");
    }
}
