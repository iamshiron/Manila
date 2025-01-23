using Shiron.Manila;
using Shiron.Manila.Utils;
using Shiron.Manila.Commands;
using Shiron.Manila.Ext;
using Shiron.Manila.CLI.Commands;

using Spectre.Console;
using Spectre.Console.Cli;
using WenceyWang.FIGlet;
using Shiron.Manila.CLI.Logger;
using Microsoft.ClearScript;
using System.Diagnostics;
using Shiron.Manila.Exceptions;

#if DEBUG
if (!Directory.Exists("./run/")) Directory.CreateDirectory("./run/");
Directory.SetCurrentDirectory("./run/");
#endif
bool verbose = args.Contains("-v") || args.Contains("--verbose");
bool stackTrace = args.Contains("--stack-trace");
bool quiet = args.Contains("-q") || args.Contains("--quiet");

if (!quiet) {
	string[] lines = new AsciiArt("Manila").Result;
	lines[lines.Length - 2] = lines[lines.Length - 2] + $" [magenta]v{Manila.VERSION}[/]";
	AnsiConsole.MarkupLine($"[blue]{string.Join("\n", lines)}[/]");
}

var logger = new ActivityLog(verbose, stackTrace);
AnsiConsole.MarkupLine("[skyblue1]Build Started At " + DateTime.Now + "[/]\n");
logger.start();
logger.log("Configuring");

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

	return 0;
}

var task = args[0][1..];
Logger.debug("Executing task: " + task);

var error = true;
try {
	Manila instance = Manila.getInstance();
	instance.init((object[] message) => {
		logger.subLog(string.Join(" ", message));
	});

	List<Shiron.Manila.API.Task> tasks = instance.workspace.getSchedule(task);


	foreach (var t in tasks) {
		logger.log(t.name);
		t.run(false);
	}

	logger.success();
	error = false;
} catch (BuildException e) {
	logger.error(e);
} catch (Exception e) {
	Exception inner = e;
	while (inner.InnerException != null) inner = inner.InnerException;
	logger.error(inner);
}

AnsiConsole.MarkupLine("\n[skyblue1]Build Finished At " + DateTime.Now + "[/]");

return error ? 1 : 0;
