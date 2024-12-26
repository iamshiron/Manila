using System.ComponentModel;
using Shiron.Manila.Utils;
using Spectre.Console.Cli;

namespace Shiron.Manila.CLI.Commands;

[Description("Lists all available tasks")]
public class CommandTasks : Command<CommandTasks.Settings> {
	public class Settings : CommandSettings {
	}

	public override int Execute(CommandContext context, Settings settings) {
		Console.WriteLine("Tasks...");

		Manila instance = Manila.getInstance();

		return 0;
	}
}
