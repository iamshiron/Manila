using System.ComponentModel;
using Spectre.Console.Cli;

namespace Shiron.Manila.CLI.Commands;

[Description("Installs the current build")]
public class CommandInstall : Command<CommandInstall.Settings> {
	public class Settings : CommandSettings {
	}

	public override int Execute(CommandContext context, Settings settings) {
		Console.WriteLine("Installing...");

		return 0;
	}
}
