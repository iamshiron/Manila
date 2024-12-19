using Spectre.Console.Cli;


#if DEBUG
if (!Directory.Exists("./run/")) Directory.CreateDirectory("./run/");
Directory.SetCurrentDirectory("./run/");
#endif

if (args.Length == 0 || !args[0].StartsWith(':')) {
	var app = new CommandApp();
	app.Configure(c => {
		c.SetApplicationName("manila");
		c.SetApplicationVersion(Manila.Manila.VERSION);
	});

	app.Run(args);

	return;
}

// Treat execution as a task execution
var task = args[0][1..];

Console.WriteLine("Executing task: " + task);
