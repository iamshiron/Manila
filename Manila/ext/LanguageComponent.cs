using Shiron.Manila.API;

namespace Shiron.Manila.Ext;

public abstract class LanguageComponent : PluginComponent {
    public LanguageComponent(string name) : base(name) {
    }
    public override string ToString() {
        return $"LanguageComponent({Name})";
    }

    public abstract void Build(Workspace workspace, Module module, BuildConfig config);
    public abstract void Run(Module module);
}
