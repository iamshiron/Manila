
using System.Diagnostics;
using System.Text;
using Shiron.Manila.Exceptions;

namespace Shiron.Manila.Utils;

public static class ProcessUtils {
	public static void runCommand(string command, string[] args, Action<string>? stdOut = null, Action<string>? stdErr = null) {
		Logger.debug("Running command: " + command + " " + string.Join(" ", args));

		var startInfo = new ProcessStartInfo() {
			FileName = command,
			Arguments = string.Join(" ", args),
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			CreateNoWindow = true,
			WindowStyle = ProcessWindowStyle.Hidden,
			StandardOutputEncoding = Encoding.UTF8,
			StandardErrorEncoding = Encoding.UTF8
		};
		startInfo.Environment["TERM"] = "xterm-256color";
		startInfo.Environment["FORCE_COLOR"] = "true";

		using var process = new Process { StartInfo = startInfo };
		StringBuilder stdOutBuilder = new();
		StringBuilder stdErrBuilder = new();

		process.OutputDataReceived += (sender, e) => {
			if (e.Data == null) return;
			stdOutBuilder.AppendLine(e.Data);
			if (stdOut != null) stdOut(e.Data);
		};

		process.ErrorDataReceived += (sender, e) => {
			if (e.Data == null) return;
			stdErrBuilder.AppendLine(e.Data);
			if (stdErr != null) stdErr(e.Data);
		};

		process.Exited += (sender, e) => {
			if (process.ExitCode == 0) return;
			throw new CompileException("Compilation Failed!", command + " " + string.Join(" ", args), stdErrBuilder.ToString(), stdErrBuilder.ToString());
		};

		process.Start();
		process.BeginOutputReadLine();
		process.BeginErrorReadLine();
		process.WaitForExit();

		Logger.debug("Command exited with code: " + process.ExitCode);
	}
}
