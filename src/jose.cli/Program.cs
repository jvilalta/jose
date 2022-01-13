using System.CommandLine;

var rootCommand = new RootCommand();

rootCommand.SetHandler(() =>
{
    Console.WriteLine("Hello, World!");
});
var newCommand = new Command("new");
var newApplicationCommand = new Command("application");
newApplicationCommand.AddAlias("app");
newApplicationCommand.SetHandler(() => {
    Console.WriteLine("application");
});
newCommand.AddCommand(newApplicationCommand);

newCommand.SetHandler(() => {
    Console.WriteLine("new");
});

rootCommand.AddCommand(newCommand);
rootCommand.Invoke(args);