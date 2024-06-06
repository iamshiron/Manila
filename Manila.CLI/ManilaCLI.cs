using Spectre.Console.Cli;


#if DEBUG
if (!Directory.Exists("./run/")) Directory.CreateDirectory("./run/");
Directory.SetCurrentDirectory("./run/");
#endif

var app = new CommandApp();
app.Configure(c => {
	c.SetApplicationName("manila");
	c.SetApplicationVersion(Manila.Manila.VERSION);
});

app.Run(args);
