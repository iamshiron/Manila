using Shiron.Manila.API;
using Shiron.Manila.CPP.Components;
using Shiron.Manila.Ext;

namespace Shiron.Manila.CPP;

public static class Utils {
    public static string GetBinFile(Module module, CppComponent c) {
        string extension = c is StaticLibComponent ? ".lib" : module.HasComponent<ConsoleComponent>() ? ".exe" : ".o";
        string binFile = module.Name + extension;

        return Path.Join(c.BinDir!, binFile);
    }
}
