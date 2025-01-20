using Shiron.Manila.Utils;
using Spectre.Console;

namespace Shiron.Manila.CLI.Logger;

public class GradleStyleLogger : AbstractLogger {
	public GradleStyleLogger(bool verbose, bool stackTrace = false) : base(verbose, stackTrace) { }

	public override void start() {
		Console.CursorVisible = false;
		statusLine = Console.CursorTop;
		Task.Run(updateTimer);
	}

	public override void stop(bool success = true, Exception? exception = null) {
		lock (consoleLock) {
			if (!string.IsNullOrEmpty(currentTask)) {
				var finalTime = DateTime.Now - startTime;
				Console.SetCursorPosition(0, statusLine);
				clearLine();
				AnsiConsole.MarkupLine($"[grey]> {currentTask} ... {formatTimeSpan(finalTime)}[/]");
			}

			running = false;
			currentTask = "";
			Console.WriteLine();
			AnsiConsole.MarkupLine((success ? "[green]BUILD SUCCESSFUL[/]" : "[red]BUILD FAILED[/]") +
				$" in {formatTimeSpan(DateTime.Now - startTime)}");
			Console.CursorVisible = true;

			if (exception != null) {
				AnsiConsole.WriteLine();
				AnsiConsole.MarkupLine("[red]Build Failed with an Exception:[/]");
				AnsiConsole.MarkupLine(formatException(exception));
			}
		}
	}

	public override void subLog(string message) {
		lock (consoleLock) {
			if (!string.IsNullOrEmpty(currentTask)) {
				Console.SetCursorPosition(0, statusLine);
				clearLine();
			}

			AnsiConsole.WriteLine(message);
			statusLine = Console.CursorTop;

			if (!string.IsNullOrEmpty(currentTask)) {
				writeStatus();
			}
		}
	}

	public override void log(string taskName) {
		lock (consoleLock) {
			if (!string.IsNullOrEmpty(currentTask)) {
				var previousTime = DateTime.Now - startTime;
				Console.SetCursorPosition(0, statusLine);
				clearLine();
				AnsiConsole.MarkupLine($"[grey]> {currentTask} ... {formatTimeSpan(previousTime)}[/]");
			}

			currentTask = taskName;
			statusLine = Console.CursorTop;
			lastWrittenStatus = "";  // Reset last status to force update
			writeStatus();
		}
	}

	protected override void updateTimer() {
		while (running) {
			lock (consoleLock) {
				if (!string.IsNullOrEmpty(currentTask)) {
					writeStatus();
				}
			}
			Thread.Sleep(100);
		}
	}

	protected override void writeStatus() {
		var timeSinceStart = DateTime.Now - startTime;
		var status = $"> {currentTask} ... {formatTimeSpan(timeSinceStart)}";

		if (status != lastWrittenStatus) {
			Console.SetCursorPosition(0, statusLine);
			clearLine();
			AnsiConsole.Markup($"[yellow]{status}[/]");
			lastWrittenStatus = status;
		}
	}

	protected override void clearLine() {
		Console.Write(new string(' ', Console.WindowWidth - 1));
		Console.SetCursorPosition(0, Console.CursorTop);
	}
}
