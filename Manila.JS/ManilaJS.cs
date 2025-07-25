
using Shiron.Manila.Attributes;
using Shiron.Manila.Ext;

namespace Shiron.Manila.JS;

public class ManilaJS() : ManilaPlugin("shiron.manila", "js", "1.0.0", ["Shiron"], []) {
    [PluginInstance]
    public static ManilaJS? Instance { get; set; }

    public override void Init() {
        Debug("Init");

        RegisterComponent(new BunComponent());
    }
    public override void Release() {
        Debug("Release");
    }
}
