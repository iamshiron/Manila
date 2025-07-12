using Microsoft.ClearScript;
using Shiron.Manila.Ext;
using Shiron.Manila.Exceptions;
using Shiron.Manila.Utils;
using Shiron.Manila.Logging;
using Shiron.Manila.Profiling;
using System.Reflection;

namespace Shiron.Manila.API;

// As class is exposed to the scripting environment, use JavaScript naming conventions
#pragma warning disable IDE1006

/// <summary>
/// The main Manila API class. Used for global functions.
/// </summary>
public sealed class Manila(ScriptContext context) : ExposedDynamicObject {
    private readonly ScriptContext Context = context;
    private readonly BuildConfig BuildConfig = new();

    /// <summary>
    /// Gets the current module in the Manila engine.
    /// </summary>
    /// <returns>The current module.</returns>
    /// <exception cref="Exception">Thrown when not in a module context.</exception>
    public Module getModule() {
        if (ManilaEngine.GetInstance().CurrentModule == null) throw new ContextException(Exceptions.Context.WORKSPACE, Exceptions.Context.PROJECT);
        return ManilaEngine.GetInstance().CurrentModule!;
    }

    /// <summary>
    /// Gets an unresolved module with the specified name.
    /// </summary>
    /// <param name="name">The name of the module to get.</param>
    /// <returns>An unresolved module with the specified name.</returns>
    public UnresolvedModule getModule(string name) {
        return new UnresolvedModule(name);
    }

    /// <summary>
    /// Gets the workspace in the Manila engine.
    /// </summary>
    /// <returns>The workspace in the Manila engine.</returns>
    public Workspace getWorkspace() {
        return ManilaEngine.GetInstance().Workspace;
    }

    /// <summary>
    /// Gets the build configuration for this Manila instance.
    /// </summary>
    /// <returns>The build configuration for this Manila instance.</returns>
    public BuildConfig getConfig() {
        return BuildConfig;
    }

    /// <summary>
    /// Creates a new source set with the specified origin.
    /// </summary>
    /// <param name="origin">The origin of the source set.</param>
    /// <returns>A new source set with the specified origin.</returns>
    public SourceSet sourceSet(string origin) {
        return new SourceSet(origin);
    }

    public async void sleep(int milliseconds) {
        await System.Threading.Tasks.Task.Delay(milliseconds);
    }

    /// <summary>
    /// Creates a new task with the specified name.
    /// </summary>
    /// <param name="name">The name of the task to create.</param>
    /// <returns>A new task with the specified name, associated with the current module and script context.</returns>
    public Task task(string name) {
        try {
            return new Task(name, getModule(), Context, Context.ScriptPath);
        } catch (ContextException e) {
            if (e.Is != Exceptions.Context.WORKSPACE) throw;
            return new Task(name, getWorkspace(), Context, Context.ScriptPath);
        }
    }

    /// <summary>
    /// Creates a new directory reference with the specified path.
    /// </summary>
    /// <param name="path">The path of the directory.</param>
    /// <returns>A new directory reference with the specified path.</returns>
    public DirHandle dir(string path) {
        return new DirHandle(path);
    }

    /// <summary>
    /// Creates a new file reference with the specified path.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <returns>A new file reference with the specified path.</returns>
    public FileHandle file(string path) {
        return new FileHandle(path);
    }

    /// <summary>
    /// Applies the plugin component with the specified key to the current module.
    /// </summary>
    /// <param name="pluginComponentKey">The key of the plugin component to apply.</param>
    public void apply(string pluginComponentKey) {
        var component = ExtensionManager.GetInstance().GetPluginComponent(pluginComponentKey);
        apply(component);
    }

    /// <summary>
    /// Applies the plugin component specified by the script object to the current module.
    /// </summary>
    /// <param name="obj">A script object containing the group, name, component, and optional version of the plugin component to apply.</param>
    public void apply(ScriptObject obj) {
        var version = obj.GetProperty("version");
        var component = ExtensionManager.GetInstance().GetPluginComponent((string) obj["group"], (string) obj["name"], (string) obj["component"], version == Undefined.Value ? null : (string) version);
        apply(component);
    }

    /// <summary>
    /// Applies the specified plugin component to the current module.
    /// </summary>
    /// <param name="component">The plugin component to apply to the current module.</param>
    public void apply(PluginComponent component) {
        using (new ProfileScope(MethodBase.GetCurrentMethod()!)) {
            Logger.Debug("Applying: " + component);
            getModule().ApplyComponent(component);
        }
    }

    /// <summary>
    /// Used for filtering modules and running actions on them.
    /// </summary>
    /// <param name="o">The type of filter, a subclass of <see cref="ModuleFilter"/></param>
    /// <param name="a">The action to run</param>
    public void onModule(object o, dynamic a) {
        using (new ProfileScope(MethodBase.GetCurrentMethod()!)) {
            var filter = ModuleFilter.From(o);
            getWorkspace().ModuleFilters.Add(new Tuple<ModuleFilter, Action<Module>>(filter, (module) => a(module)));
        }
    }

    /// <summary>
    /// Runs a task with the specified key.
    /// </summary>
    /// <param name="key">The key</param>
    /// <exception cref="Exception">Thrown if task was not found</exception>
    public void runTask(string key) {
        var task = getWorkspace().GetTask(key);
        if (task == null) throw new Exception("Task not found: " + key);

        task.Execute();
    }

    /// <summary>
    /// Calls the underlying compiler to build the module
    /// </summary>
    /// <param name="workspace">The workspace</param>
    /// <param name="module">The module</param>
    /// <param name="config">The config</param>
    public void build(Workspace workspace, Module module, BuildConfig config) {
        using (new ProfileScope(MethodBase.GetCurrentMethod()!)) {
            module.GetLanguageComponent().Build(workspace, module, config);
        }
    }
    public void run(UnresolvedModule module) {
        run(module.Resolve());
    }
    public void run(Module module) {
        using (new ProfileScope(MethodBase.GetCurrentMethod()!)) {
            module.GetLanguageComponent().Run(module);
        }
    }
    public string getEnv(string key) {
        return Context.GetEnvironmentVariable(key);
    }
    public double getEnvNumber(string key) {
        var value = Context.GetEnvironmentVariable(key);
        if (string.IsNullOrEmpty(value)) return 0;
        if (double.TryParse(value, out var result)) return result;
        throw new Exception($"Environment variable {key} is not a number: {value}");
    }
    public bool getEnvBool(string key) {
        var value = Context.GetEnvironmentVariable(key);
        if (string.IsNullOrEmpty(value)) return false;
        if (bool.TryParse(value, out var result)) return result;
        throw new Exception($"Environment variable {key} is not a boolean: {value}");
    }
    public void setEnv(string key, string value) {
        Context.SetEnvironmentVariable(key, value);
    }

    public object import(string key) {
        using (new ProfileScope(MethodBase.GetCurrentMethod()!)) {
            var t = Activator.CreateInstance(ExtensionManager.GetInstance().GetAPIType(key));
            Logger.Debug($"Importing {key} as {t}");

            if (t == null)
                throw new Exception($"Failed to import API type for key: {key}");

            return t;
        }
    }

    // Task Actions
    public ITaskAction shell(string command) {
        return new TaskShellAction(new(
            "cmd.exe",
            ["/c", .. command.Split(" ")]
        ));
    }
    public ITaskAction execute(string command) {
        return new TaskShellAction(new(
            command.Split(" ")[0],
            command.Split(" ")[1..]
        ));
    }
}
