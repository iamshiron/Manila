namespace Shiron.Manila.JS;

using Shiron.Manila.API;
using Shiron.Manila.Ext;
using Shiron.Manila.Utils;

public class TSComponent : BaseComponent {
    public TSComponent() : base("ts") {
    }

    public override void Build(Workspace workspace, Project project, BuildConfig config) {
        ManilaJS.Instance.Debug("Building TypeScript project...");
    }

    public override void Run(Project project) {
        ManilaJS.Instance.Debug("Running TypeScript project...");
    }
}
