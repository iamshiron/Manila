using System.Diagnostics;
using Shiron.Manila.Ext;
using Shiron.Manila.Utils;
using Spectre.Console.Cli;

namespace Shiron.Manila.CLI;

public class GlobalCommand : Command<GlobalCommand.Settings> {
	public class Settings : CommandSettings {
		[CommandOption("-v|--verbose")]
		public bool verbose { get; set; }

		[CommandOption("-q|--quiet")]
		public bool quiet { get; set; }
	}

	public override int Execute(CommandContext context, Settings settings) {
		return 0;
	}
}
