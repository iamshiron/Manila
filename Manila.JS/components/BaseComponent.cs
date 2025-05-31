namespace Shiron.Manila.JS;

using Shiron.Manila.API;
using Shiron.Manila.Ext;

public class BaseComponent : LanguageComponent {
    public BaseComponent(string name) : base(name) {
    }

    public override void Build(Workspace workspace, Project project, BuildConfig config) {
        throw new Exception("Build logic not implemented for JavaScript/TypeScript projects.");
    }

    public override void Run(Project project) {
        throw new Exception("A BaseComponent cannot be started directly. Please use a TypeScriptComponent or JavaScriptComponent.");
    }
}
