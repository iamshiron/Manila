using System.Text;

public class CompileException : Exception {
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

	private static string format(string m) {
		var builder = new StringBuilder();
		foreach (var l in m.Split('\n')) {
			builder.Append("\u001B[0m" + l + "\n");
		}
		return builder.ToString();
	}
}
