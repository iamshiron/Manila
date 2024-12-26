using System.ComponentModel;
using Shiron.Manila.Utils;
using Spectre.Console.Cli;

namespace Shiron.Manila.CLI.Commands;

[Description("Builds the entire project")]
public class CommandBuild : Command<CommandBuild.Settings> {
	public class Settings : CommandSettings {
	}

	public override int Execute(CommandContext context, Settings settings) {
		Manila instance = Manila.getInstance();

		Logger.info("Building project...");

		return 0;
	}
}
