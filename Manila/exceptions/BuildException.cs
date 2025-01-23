
using System.Text;
using Spectre.Console;

namespace Shiron.Manila.Exceptions;

public class BuildException : Exception {
	public readonly API.Task? task;

	public BuildException(string message) : base(message) {
		task = API.Task.currentTask;
	}
	public BuildException(string message, Exception innerException) : base(message, innerException) {
		task = API.Task.currentTask;
	}

	public string formatHeader() {
		return $"{Markup.Escape(GetType().Name)}: [red]{Markup.Escape(Message)}[/] in [blue]{(task == null ? "[[NONE]]" : $"[[{task.getQualifiedName()}]]")}[/]";
	}

	public virtual string format() {
		var builder = new StringBuilder();
		builder.AppendLine(formatHeader());

		if (StackTrace != null)
			foreach (var l in StackTrace.Split('\n'))
				builder.AppendLine($"[grey]{Markup.Escape(l)}[/]");

		return builder.ToString();
	}
}
