using System.Text;
using Spectre.Console;

namespace Shiron.Manila.Exceptions;

public class CompileException : BuildException {
	public string command { get; private set; }
	public string stdOut { get; private set; }
	public string stdErr { get; private set; }

	public CompileException(string message, string command, string stdOut, string stdErr) : base(message) {
		this.command = command;
		this.stdOut = stdOut;
		this.stdErr = stdErr;
	}

	public CompileException(string message, string command, string stdOut, string stdErr, Exception innerException) : base(message, innerException) {
		this.command = command;
		this.stdOut = stdOut;
		this.stdErr = stdErr;
	}

	public override string format() {
		var builder = new StringBuilder();
		builder.AppendLine(formatHeader());
		builder.AppendLine($"Command: {Markup.Escape(command)}");
		builder.AppendLine("Compiler Output:");
		builder.AppendLine(Markup.Escape(stdOut));

		return builder.ToString();
	}
}
