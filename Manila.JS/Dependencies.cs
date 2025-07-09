namespace Shiron.Manila.JS;

using Shiron.Manila.API;
using Shiron.Manila.Utils;

public class DependencyNPM : Dependency {
    public string Name { get; private set; } = string.Empty;
    public string Version { get; private set; } = string.Empty;

    public DependencyNPM() : base("npm") {
    }

    public override void Create(params object[] args) {
        if (args.Length == 0) throw new Exception("NPM dependency requires at least one argument");

        if (args.Length == 1) {
            if (args[0] is not string packageArg) throw new Exception("NPM dependency requires a string argument");

            // Handle 'package' or 'package@version' format
            if (packageArg.Contains('@')) {
                var parts = packageArg.Split('@', 2);
                this.Name = parts[0];
                this.Version = parts[1];
            } else {
                this.Name = packageArg;
            }
        } else if (args.Length == 2) {
            // Handle 'package', 'version' format
            if (args[0] is not string packageName) throw new Exception("NPM dependency first argument must be a string");
            if (args[1] is not string version) throw new Exception("NPM dependency second argument must be a string");

            this.Name = packageName;
            this.Version = version;
        } else {
            throw new Exception("NPM dependency requires one or two arguments");
        }
    }

    public override void Resolve(Project project) {
        ManilaJS.Instance.Debug("Resolving NPM dependency: " + this.Name + "@" + this.Version);
    }
}
