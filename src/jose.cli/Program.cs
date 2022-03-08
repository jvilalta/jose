using Scriban;
using System.CommandLine;

var rootCommand = new RootCommand();

rootCommand.SetHandler(() =>
{
    Console.WriteLine("Hello, World!");
});
var newCommand = new Command("new");
var newApplicationCommand = new Command("application");
newApplicationCommand.AddAlias("app");
newApplicationCommand.SetHandler(() =>
{
    Console.WriteLine("new application");
    Console.WriteLine("Company Name:");
    var companyName = Console.ReadLine();
    if (!Directory.Exists(companyName))
    {
        Directory.CreateDirectory(companyName);
    }
    else
    {
        Console.WriteLine("You've applied here before:");
        foreach (var dir in Directory.GetDirectories(companyName))
        {
            var info = Directory.GetCreationTime(dir);
            Console.WriteLine($"{dir} on {info}");
        }
    }
    Console.WriteLine("Position:");
    var position = Console.ReadLine();
    var positionDir = Path.Combine(companyName, position);
    if (Directory.Exists(positionDir))
    {
        Console.WriteLine("Already applied");
        Console.WriteLine("Overwrite (y/N)?");
        var over = Console.ReadLine();
        if (over.Equals("y"))
        {
            Directory.Delete(positionDir, true);
        }
        else
        {
            return;
        }
    }
    Directory.CreateDirectory(positionDir);
    var coverLetterTemplate = Template.Parse(File.ReadAllText("base_cover_letter.txt"));
    File.WriteAllText(Path.Join(positionDir, "cover_letter.txt"), coverLetterTemplate.Render(new { company = companyName, position = position }));
    File.Copy("base_resume.pdf", Path.Join(positionDir, "Jay Vilalta.pdf"));

});
newCommand.AddCommand(newApplicationCommand);

newCommand.SetHandler(() =>
{
    Console.WriteLine("new");
});

rootCommand.AddCommand(newCommand);
rootCommand.Invoke(args);