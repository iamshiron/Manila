
using System.Diagnostics;

namespace Shiron.Manila.Utils;

public static class ProcessUtils {
	public static void runCommand(string command, string[] args, Action<string>? stdOut = null, Action<string>? stdErr = null) {
		Logger.debug("Running command: " + command + " " + string.Join(" ", args));

		var startInfo = new ProcessStartInfo() {
			FileName = command,
			Arguments = string.Join(" ", args),
			UseShellExecute = false,
			RedirectStandardOutput = stdOut != null,
			RedirectStandardError = stdErr != null,
		};

		using (Process process = Process.Start(startInfo)) {
			if (stdOut != null) {
				process.OutputDataReceived += (sender, e) => {
					if (e.Data != null) stdOut(e.Data);
				};
				process.BeginOutputReadLine();
			}

			if (stdOut != null) {
				process.ErrorDataReceived += (sender, e) => {
					if (e.Data != null) stdErr(e.Data);
				};
				process.BeginErrorReadLine();
			}

			process.WaitForExit();
		}
	}
}
