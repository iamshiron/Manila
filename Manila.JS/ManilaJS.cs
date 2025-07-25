
using Shiron.Manila.Ext;

namespace Shiron.Manila.JS;

public class ManilaJS() : ManilaPlugin("shiron.manila", "js", "1.0.0", ["Shiron"], []) {
    public override void Init() {
        Debug("Init");
    }
    public override void Release() {
        Debug("Release");
    }
}
