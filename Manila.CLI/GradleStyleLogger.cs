using Shiron.Manila.Utils;
using Spectre.Console;

public class GradleStyleLogger {
	private readonly DateTime _startTime = DateTime.Now;
	private bool _isRunning = true;
	private readonly object _consoleLock = new();
	private string _currentTask = "";
	private int _statusLine;
	private string _lastWrittenStatus = "";

	public void start() {
		Console.CursorVisible = false;
		_statusLine = Console.CursorTop;
		Task.Run(updateTimer);
	}


	public void stop(bool success = true, Exception? exception = null) {
		lock (_consoleLock) {
			// Complete the final task if one exists
			if (!string.IsNullOrEmpty(_currentTask)) {
				var finalTime = DateTime.Now - _startTime;
				Console.SetCursorPosition(0, _statusLine);
				clearLine();
				AnsiConsole.MarkupLine($"[grey]> {_currentTask} ... {formatTimeSpan(finalTime)}[/]");
			}

			_isRunning = false;
			_currentTask = "";
			Console.WriteLine();
			AnsiConsole.MarkupLine((success ? "[green]BUILD SUCCESSFUL[/]" : "[red]BUILD FAILED[/]") + $" in {formatTimeSpan(DateTime.Now - _startTime)}");
			Console.CursorVisible = true;

			if (exception != null) {
				AnsiConsole.MarkupLine($"\n[red]{exception.Message}[/]");
				AnsiConsole.MarkupLine($"[grey]{Markup.Escape(exception.StackTrace ?? "")}[/]");
			}
		}
	}

	public void subLog(string message) {
		lock (_consoleLock) {
			if (!string.IsNullOrEmpty(_currentTask)) {
				Console.SetCursorPosition(0, _statusLine);
				clearLine();
			}

			AnsiConsole.WriteLine(message);
			_statusLine = Console.CursorTop;

			if (!string.IsNullOrEmpty(_currentTask)) {
				writeStatus();
			}
		}
	}

	public void log(string taskName) {
		lock (_consoleLock) {
			if (!string.IsNullOrEmpty(_currentTask)) {
				var previousTime = DateTime.Now - _startTime;
				Console.SetCursorPosition(0, _statusLine);
				clearLine();
				AnsiConsole.MarkupLine($"[grey]> {_currentTask} ... {formatTimeSpan(previousTime)}[/]");
			}

			_currentTask = taskName;
			_statusLine = Console.CursorTop;
			_lastWrittenStatus = "";  // Reset last status to force update
			writeStatus();
		}
	}

	private void updateTimer() {
		while (_isRunning) {
			lock (_consoleLock) {
				if (!string.IsNullOrEmpty(_currentTask)) {
					writeStatus();
				}
			}
			Thread.Sleep(100);
		}
	}

	private void writeStatus() {
		var timeSinceStart = DateTime.Now - _startTime;
		var status = $"> {_currentTask} ... {formatTimeSpan(timeSinceStart)}";

		// Only update if the status has changed
		if (status != _lastWrittenStatus) {
			Console.SetCursorPosition(0, _statusLine);
			clearLine();
			AnsiConsole.Markup($"[yellow]{status}[/]");
			_lastWrittenStatus = status;
		}
	}

	private void clearLine() {
		Console.Write(new string(' ', Console.WindowWidth - 1));
		Console.SetCursorPosition(0, Console.CursorTop);
	}

	private static string formatTimeSpan(TimeSpan span) {
		if (span.TotalMinutes < 1) {
			return $"{span.TotalSeconds:F1}s";
		}
		return $"{(int) span.TotalMinutes}m {span.Seconds}s";
	}
}
