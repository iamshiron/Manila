namespace Shiron.Manila.API;

/// <summary>
/// Represents an unresolved module. Used for the configuration of modules in build scripts, where the module is not yet resolved.
/// </summary>
public class UnresolvedModule {
    public readonly string identifier;

    public UnresolvedModule(string identifier) { this.identifier = identifier; }

    /// <summary>
    /// Resolve the module from the identifier.
    /// </summary>
    /// <returns>The resolved module</returns>
    /// <exception cref="Exception">Module either does not exist or is unknown to the context</exception>
    public Module Resolve() {
        foreach (var pair in ManilaEngine.GetInstance().Workspace.Modules) {
            if (pair.Value.GetIdentifier() == identifier) {
                return pair.Value;
            }
        }
        throw new Exception("Module not found: " + identifier);
    }

    /// <summary>
    /// Implicitly convert an unresolved module to a module. Used for convenience in build scripts.
    /// </summary>
    /// <param name="unresolved">The unresolved poject</param>
    public static implicit operator Module(UnresolvedModule unresolved) {
        return unresolved.Resolve();
    }
}
