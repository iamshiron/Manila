
using Shiron.Manila.API;
using Shiron.Manila.Artifacts;
using Shiron.Manila.Ext;

namespace Shiron.Manila.JS;

public class BunComponent() : JavaScriptComponent("bun", typeof(BunBuildConfig)) {
    public override IBuildExitCode Build(Workspace workspace, Project project, BuildConfig config, Artifact artifact, IArtifactManager artifactManager) {
        ManilaJS.Instance?.Info($"Building {project.Name} with Bun...");
        return new BuildExitCodeSuccess();
    }
    public override void Run(Project project, Artifact artifact) {
        ManilaJS.Instance?.Info($"Running {project.Name} with Bun...");
    }
}
