using System.Text;
using Microsoft.VisualBasic;
using Shiron.Manila.API;
using Shiron.Manila.Exceptions;
using Shiron.Manila.Utils;
using Spectre.Console;

namespace Shiron.Manila.CLI.Logger;

public class ActivityLog : AbstractActivityLogger {
	private bool verbose;
	private bool stackTrace;

	private DateTime startTime;

	public ActivityLog(bool verbose, bool stackTrace) {
		this.verbose = verbose;
		this.stackTrace = stackTrace;
	}

	public void start() {
		startTime = DateTime.Now;
	}

	public void subLog(string message) {
		AnsiConsole.WriteLine(message);
	}

	private static string formatTimeSpan(TimeSpan span) {
		if (span.TotalMinutes < 1) {
			return $"{span.TotalSeconds:F1}s";
		}
		return $"{(int) span.TotalMinutes}m {span.Seconds}s";
	}

	public void success() {
		var span = DateTime.Now - startTime;
		AnsiConsole.MarkupLine($"\n[green]BUILD SUCCESSFUL[/] in {formatTimeSpan(span)}");
	}

	public void error(Exception? e) {
		var span = DateTime.Now - startTime;
		AnsiConsole.MarkupLine($"\n[red]BUILD FAILED[/] in {formatTimeSpan(span)}\n");

		if (e != null) AnsiConsole.MarkupLine(format(e));
		else AnsiConsole.MarkupLine("[red]Unknown error occured![/]");
	}

	public string format(Exception e) {
		if (e is BuildException) return ((BuildException) e).format();
		var builder = new StringBuilder();

		builder.AppendLine($"{Markup.Escape(e.GetType().Name)}: [red]{Markup.Escape(e.Message)}[/]");
		if (stackTrace && e.StackTrace != null)
			foreach (var l in e.StackTrace.Split('\n'))
				builder.AppendLine($"[grey]{Markup.Escape(l)}[/]");

		return builder.ToString();
	}

	public override void task(API.Task t) {
		AnsiConsole.MarkupLine($"[grey]> {Markup.Escape(t.getQualifiedName())}[/]");
	}

	public override void taskEnd(API.Task t) { }

	public override void compileProject(Project p) {
		AnsiConsole.MarkupLine($"[skyblue1]> Compiling project {Markup.Escape(p.getIdentifier())}[/]");
	}

	public override void compileProjectEnd(Project p) {
		AnsiConsole.MarkupLine($"[green]> Finished compiling project {Markup.Escape(p.getIdentifier())}[/]");
	}

	public override void compileFile(string file) {
		AnsiConsole.MarkupLine(Path.GetFileName(file));
	}
}
