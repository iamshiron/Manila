using System.Text;
using Shiron.Manila.Ext;

namespace Shiron.Manila.API;

public class Project {
	public string name { get; private set; }
	public Dictionary<string, Task> tasks { get; } = new();
	public List<PluginComponent> appliedComponents { get; } = new();

	public Project(string name) {
		this.name = name;
	}

	public virtual void runTask(string name) {
		if (tasks.TryGetValue(name, out var task)) {
			task.run();
		} else {
			throw new Exception($"Task '{name}' not found.");
		}
	}
}
