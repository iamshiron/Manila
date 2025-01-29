using System.Text;
using Shiron.Manila.Utils;

namespace Shiron.Manila.API;

public class Workspace : Project {
	public BuildConfig buildConfig = new();
	public Dictionary<string, Project> projects { get; } = new();

	/// <summary>
	/// Creates a new workspace
	/// </summary>
	/// <param name="path">The absolute path</param>
	public Workspace(string path) : base("", path) {
		// Will be changed to a more dynamic way of setting the build config
		buildConfig.config = "Debug";
		buildConfig.platform = EPlatform.windows;
		buildConfig.architecture = EArchitecture.x64;
	}

	public override void runTask(string name) {
		Logger.debug("Running task: " + name);
		string[] parts = name.Split(':');

		if (parts.Length < 1) throw new Exception($"Invalid task name '{name}'.");

		if (parts.Length < 2) {
			if (!tasks.ContainsKey(name)) throw new Exception($"Task '{name}' not found.");
			tasks[name].run();
			return;
		}

		string project = string.Join(":", parts.Take(parts.Length - 1));
		if (!projects.ContainsKey(project)) throw new Exception($"Project '{project}' not found.");
		projects[project].runTask(parts[parts.Length - 1]);
	}


	public List<Task> getSchedule(string name) {
		var schedule = new List<Task>();
		var visited = new HashSet<string>();

		void AddTaskToSchedule(string taskName) {
			if (visited.Contains(taskName)) return;
			visited.Add(taskName);

			string[] parts = taskName.Split(':');
			Task task;

			if (parts.Length < 2) {
				if (!tasks.ContainsKey(taskName)) throw new Exception($"Task '{taskName}' not found.");
				task = tasks[taskName];
			} else {
				string project = string.Join(":", parts.Take(parts.Length - 1));
				if (!projects.ContainsKey(project)) throw new Exception($"Project '{project}' not found.");
				task = projects[project].tasks[parts[parts.Length - 1]];
			}

			foreach (var dep in task.dependencies) {
				AddTaskToSchedule(dep);
			}

			schedule.Add(task);
		}

		AddTaskToSchedule(name);
		return schedule.Distinct().ToList();
	}
}
