namespace Shiron.Manila.CPP.Components;

using Shiron.Manila.API;
using Shiron.Manila.Ext;
using Shiron.Manila.Attributes;
using Shiron.Manila.CPP.Toolchain.Impl;
using Shiron.Manila.CPP.Toolchain;

/// <summary>
/// Represents a C++ module.
/// </summary>
public class CppComponent : LanguageComponent {
    public CppComponent(string name) : base(name) {
    }

    [ScriptProperty]
    public DirHandle? BinDir { get; set; }
    [ScriptProperty]
    public DirHandle? ObjDir { get; set; }
    [ScriptProperty]
    public EToolChain? ToolChain { get; set; }
    public List<string> IncludeDirs { get; set; } = [];
    public List<string> Links { get; set; } = [];

    public override void Build(Workspace workspace, Module module, BuildConfig config) {
        foreach (var dep in module._dependencies) {
            dep.Resolve(module);
        }

        Toolchain toolchain =
            ToolChain == EToolChain.Clang ? new ToolchainClang(workspace, module, config) :
            ToolChain == EToolChain.MSVC ? new ToolchainMSVC(workspace, module, config) :
            throw new Exception($"Toolchain '{ToolChain}' is not supported.");

        toolchain.Build(workspace, module, config);
    }

    public override void Run(Module module) {
        throw new Exception("A CppComponent cannot be started directly. Please use a ConsoleComponent.");
    }
}
