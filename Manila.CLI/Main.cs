﻿using Shiron.Manila;
using Shiron.Manila.Utils;
using Shiron.Manila.Commands;
using Shiron.Manila.Ext;
using Shiron.Manila.CLI.Commands;

using Spectre.Console;
using Spectre.Console.Cli;
using WenceyWang.FIGlet;

#if DEBUG
if (!Directory.Exists("./run/")) Directory.CreateDirectory("./run/");
Directory.SetCurrentDirectory("./run/");
#endif
bool verbose = args.Contains("-v") || args.Contains("--verbose");
bool quiet = args.Contains("-q") || args.Contains("--quiet");

if (!quiet) {
	string[] lines = new AsciiArt("Manila").Result;
	lines[lines.Length - 2] = lines[lines.Length - 2] + $" [magenta]v{Manila.VERSION}[/]";
	AnsiConsole.MarkupLine($"[blue]{string.Join("\n", lines)}[/]");
}
Logger.init(verbose, quiet);

Logger.debug("Discovering plugins...");
ExtensionAPI.getInstance().init();

if (args.Length < 1 || !args[0].StartsWith(':')) {
	var app = new CommandApp();
	app.Configure(c => {
		c.SetApplicationName("manila");
		c.SetApplicationVersion(Manila.VERSION);

		c.AddCommand<CommandBuild>("build");
		c.AddCommand<CommandInit>("init");
		c.AddCommand<CommandInstall>("install");
		c.AddCommand<CommandTasks>("tasks");
		c.AddCommand<CommandRun>("run");
	});

	app.Run(args);

	return;
}

var task = args[0][1..];
Logger.debug("Executing task: " + task);

try {
	Manila instance = Manila.getInstance();
	instance.workspace.runTask(task);
} catch (Exception e) {
	Console.WriteLine(e.Message);
	Console.WriteLine(e.StackTrace);
}
