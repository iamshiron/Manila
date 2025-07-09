namespace Shiron.Manila.JS;

using Shiron.Manila.Ext;
using Shiron.Manila.Attributes;
using System.Diagnostics.CodeAnalysis;

public class ManilaJS : ManilaPlugin {
    public ManilaJS() : base("shiron.manila", "javascript", "1.0.0", ["Shiron"], []) {
    }

    [PluginInstance]
    public static ManilaJS Instance { get; set; } = default!;

    public override void Init() {
        RegisterComponent(new TSComponent());

        RegisterDependency<DependencyNPM>();
    }

    public override void Release() {
    }
}
