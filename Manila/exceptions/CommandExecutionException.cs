
using System.Text;
using Shiron.Manila.Exceptions;

public class CommandExecutionException : BuildException {
	public readonly string invokedCommand;
	public readonly string? stdOut;
	public readonly string? stdErr;
	public readonly int exitCode;

	public CommandExecutionException(string invokedCommand, string? stdOut, string? stdErr, int exitCode) : base("Command Invocation Resulted in non 0 exit code!") {
		this.invokedCommand = invokedCommand;
		this.stdOut = stdOut;
		this.stdErr = stdErr;
		this.exitCode = exitCode;
	}
	public CommandExecutionException(string invokedCommand, string? stdOut, string? stdErr, int exitCode, Exception inner) : base("Command Invocation Resulted in non 0 exit code!", inner) {
		this.invokedCommand = invokedCommand;
		this.stdOut = stdOut;
		this.stdErr = stdErr;
		this.exitCode = exitCode;
	}

	public override string format() {
		var builder = new StringBuilder();
		builder.AppendLine(formatHeader());

		builder.AppendLine($"[grey]Command: {invokedCommand}[/]");
		builder.AppendLine($"[grey]Exit Code: {exitCode}[/]");

		if (stdOut != null) {
			builder.AppendLine("[red]Standard Output:[/]");
			foreach (var l in stdOut.Split('\n')) builder.AppendLine($"[grey]{l}[/]");
		}
		if (stdErr != null && !stdErr.Equals(stdOut)) {
			builder.AppendLine("[red]Standard Error:[/]");
			foreach (var l in stdErr.Split('\n')) builder.AppendLine($"[grey]{l}[/]");
		}

		return builder.ToString();
	}
}
