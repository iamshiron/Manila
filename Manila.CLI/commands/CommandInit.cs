using System.ComponentModel;
using Spectre.Console.Cli;

namespace Shiron.Manila.CLI.Commands;

[Description("Initialize a new project")]
public class CommandInit : Command<CommandInit.Settings> {
	public class Settings : CommandSettings {
	}

	public override int Execute(CommandContext context, Settings settings) {
		Console.WriteLine("Initializing a new Manila project...");

		return 0;
	}
}
