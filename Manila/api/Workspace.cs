using System.Text;

namespace Shiron.Manila.API;

public class Workspace : Project {
	public Workspace() : base("") {
	}

	public Dictionary<string, Project> projects { get; } = new();

	public override void runTask(string name) {
		string[] parts = name.Split(':');
		if (parts.Length < 1) {
			throw new Exception($"Invalid task name '{name}'.");
		}

		if (parts.Length < 2) {
			if (!projects.ContainsKey(parts[0])) throw new Exception($"Project '{parts[0]}' not found.");
			this.tasks[parts[0]].run();
			return;
		}

		string project = string.Join(":", parts.Take(parts.Length - 1));
		if (!projects.ContainsKey(project)) throw new Exception($"Project '{project}' not found.");
		projects[project].runTask(parts[parts.Length - 1]);
	}
}
