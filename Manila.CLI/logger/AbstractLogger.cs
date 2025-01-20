
using System.Reflection;
using System.Text;
using Microsoft.ClearScript;
using Spectre.Console;

namespace Shiron.Manila.CLI.Logger;

public abstract class AbstractLogger {
	protected readonly bool verbose;
	protected readonly bool stackTrace;

	public AbstractLogger(bool verbose, bool stackTrace = false) {
		this.verbose = verbose;
		this.stackTrace = stackTrace;
	}

	protected readonly DateTime startTime = DateTime.Now;
	protected bool running = true;
	protected readonly object consoleLock = new();
	protected string currentTask = "";
	protected int statusLine;
	protected string lastWrittenStatus = "";

	public abstract void start();
	public abstract void stop(bool success = true, Exception? exception = null);
	public abstract void subLog(string message);
	public abstract void log(string taskName);

	protected abstract void updateTimer();
	protected abstract void writeStatus();
	protected abstract void clearLine();

	protected static string formatTimeSpan(TimeSpan span) {
		if (span.TotalMinutes < 1) {
			return $"{span.TotalSeconds:F1}s";
		}
		return $"{(int) span.TotalMinutes}m {span.Seconds}s";
	}

	public string formatException(Exception e) {
		StringBuilder res = new();

		if (!verbose && (e is TargetInvocationException || e is ScriptEngineException)) {
			return formatException(e.InnerException);
		}

		res.AppendLine($"{Markup.Escape(Markup.Escape(e.GetType().Name))}: [red]{Markup.Escape(e.Message)}[/]");
		if (stackTrace) {
			foreach (var line in e.StackTrace.Split('\n')) {
				res.Append($"\n[grey]{Markup.Escape(line)}[/]");
			}
			res.AppendLine();
		}

		if (e is CompileException) {
			var ce = (CompileException) e;
			res.AppendLine();
			res.AppendLine("-- Compile Output --");
			res.Append(Markup.Escape(ce.stdOut));
		} else if (e is BuildException) {
			var be = (BuildException) e;
		}

		if (e.InnerException != null) {
			res.Append("\nCaused by: ");
			res.Append(formatException(e.InnerException));
		}

		return res.ToString();
	}
}
