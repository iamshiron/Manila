using Shiron.Manila.API;
using Shiron.Manila.Utils;

namespace Shiron.Manila.Ext;

/// <summary>
/// Represents a Manila plugin.
/// </summary>
public abstract class ManilaPlugin(string group, string name, string version) {
    public readonly string Group = group;
    public readonly string Name = name;
    public readonly string Version = version;
    public readonly Dictionary<string, PluginComponent> Components = [];
    public readonly List<Type> Enums = [];
    public readonly List<Type> Dependencies = [];

    /// <summary>
    /// Called upon initialization of the plugin.
    /// </summary>
    public abstract void Init();
    /// <summary>
    /// Called upon release of the plugin.
    /// </summary>
    public abstract void Release();

    /// <summary>
    /// Prints a debug severity message.
    /// </summary>
    /// <param name="args">The message</param>
    public void Debug(params object[] args) { Logger.PluginDebug(this, args); }
    /// <summary>
    /// Prints a information severity message.
    /// </summary>
    /// <param name="args">The message</param>
    public void Info(params object[] args) { Logger.PluginInfo(this, args); }
    /// <summary>
    /// Prints a warning severity message.
    /// </summary>
    /// <param name="args">The message</param>
    public void Warn(params object[] args) { Logger.PluginWarn(this, args); }
    /// <summary>
    /// Prints a error severity message.
    /// </summary>
    /// <param name="args">The message</param>
    public void Error(params object[] args) { Logger.PluginError(this, args); }

    /// <summary>
    /// Registers a component to the plugin.
    /// </summary>
    /// <param name="component">The instance the component</param>
    /// <exception cref="Exception">Component already registered to this plugin</exception>
    public void RegisterComponent(PluginComponent component) {
        if (Components.ContainsKey(component.Name)) throw new Exception("Component with name " + component.Name + " already registered");
        Components.Add(component.Name, component);
        component.plugin = this;
    }
    /// <summary>
    /// Registers an enum to the plugin. The class requires the <see cref="ScriptEnum"/> attribute.
    /// </summary>
    /// <typeparam name="T">The class type</typeparam>
    public void RegisterEnum<T>() {
        Enums.Add(typeof(T));
    }
    public void RegisterDependency<T>() {
        Dependencies.Add(typeof(T));
    }

    /// <summary>
    /// Returns a component by its name.
    /// </summary>
    /// <param name="name">The component name</param>
    /// <returns>The instance of the component</returns>
    /// <exception cref="Exception">Component was not found</exception>
    public PluginComponent GetComponent(string name) {
        if (!Components.ContainsKey(name)) throw new Exception("Component with name " + name + " not registered");
        return Components[name];
    }

    /// <summary>
    /// Returns a string representation of the plugin.
    /// </summary>
    /// <returns>Format: ManilaPlugin(Group:Name@Version)</returns>
    public override string ToString() {
        return $"ManilaPlugin({Group}:{Name}@{Version})";
    }
}
