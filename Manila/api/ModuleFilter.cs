using System.Text.RegularExpressions;
using Microsoft.ClearScript;
using Shiron.Manila.Exceptions;
using Shiron.Manila.Logging;

namespace Shiron.Manila.API;

/// <summary>
/// Represents a filter for modules to create global configurations for specific modules.
/// </summary>
public abstract class ModuleFilter {
    public abstract bool Predicate(Module p);

    public static ModuleFilter From(object o) {
        Logger.Debug("From " + o.GetType());

        if (o is string) {
            var s = (string) o;
            if (s == "*") return new ModuleFilterAll();
            return new ModuleFilterName(s);
        }

        if (o is IList<object> list) {
            Logger.Debug("Array");

            var filters = new ModuleFilter[list.Count];
            for (var i = 0; i < list.Count; i++) filters[i] = From(list[i]);
            return new ModuleFilterArray(filters);
        }

        if (o is ScriptObject obj) {
            foreach (var key in obj.PropertyNames) {
                try {
                    var value = obj.GetProperty(key);
                    Logger.Debug($"Property: {key}, Value: {value}, Type: {value?.GetType()}");
                } catch (Exception ex) {
                    Logger.Debug($"Error accessing property {key}: {ex.Message}");
                }
            }

            string objString = o?.ToString() ?? string.Empty;
            if (objString.StartsWith("/") && objString.Contains("/")) {
                int lastSlashIndex = objString.LastIndexOf('/');
                string pattern = objString.Substring(1, lastSlashIndex - 1);
                string flags = lastSlashIndex < objString.Length - 1 ? objString.Substring(lastSlashIndex + 1) : "";

                Logger.Debug($"Detected regex pattern: '{pattern}', flags: '{flags}'");
                return new ModuleFilterRegex(new Regex(pattern));
            }

            try {
                dynamic dyn = obj;
                var constructorName = dyn.constructor.name;
                if (constructorName == "RegExp") {
                    string pattern = dyn.source;
                    string flags = dyn.flags;
                    Logger.Debug($"Detected RegExp object with pattern: '{pattern}', flags: '{flags}'");
                    return new ModuleFilterRegex(new Regex(pattern));
                }
            } catch (Exception ex) {
                Logger.Debug($"Error checking constructor: {ex.Message}");
            }
        }

        throw new ManilaException("Invalid module filter. " + o);
    }
}

public class ModuleFilterName : ModuleFilter {
    private readonly string _name;

    public ModuleFilterName(string name) {
        this._name = name;
    }

    public override bool Predicate(Module p) {
        return p.GetIdentifier() == _name;
    }
}

public class ModuleFilterAll : ModuleFilter {
    public override bool Predicate(Module p) {
        return true;
    }
}

public class ModuleFilterRegex : ModuleFilter {
    private readonly Regex _regex;

    public ModuleFilterRegex(Regex regex) {
        this._regex = regex;
    }

    public override bool Predicate(Module p) {
        return _regex.IsMatch(p.Name);
    }
}

public class ModuleFilterArray : ModuleFilter {
    private readonly ModuleFilter[] _filters;

    public ModuleFilterArray(ModuleFilter[] filters) {
        this._filters = filters;
    }

    public override bool Predicate(Module p) {
        foreach (var filter in _filters) if (filter.Predicate(p)) return true;
        return false;
    }
}
