using Scriban;
using System.CommandLine;
using System.Diagnostics;

var rootCommand = new RootCommand();

rootCommand.SetHandler(() =>
{
    Console.WriteLine("Hello, World!");
});
var rejectionCommand = new Command("rejection");
rejectionCommand.AddAlias("rej");
rejectionCommand.SetHandler(() =>
{
    Console.WriteLine("Rejection");
    Console.WriteLine("Company Name:");
    var companyName = Console.ReadLine();
    if (!Directory.Exists(companyName))
    {
        Console.WriteLine("Company Not Found");
        return;
    }
    Console.WriteLine("Positions:");
    var positions = Directory.GetDirectories(companyName);
    int counter = 0;
    foreach (var position in positions)
    {
        var files = Directory.GetFiles(position);
        if (files.Any(f=>f.EndsWith("rejection.txt")))
        {
            continue;
        }
        Console.WriteLine($"{counter}: {position}");
        counter++;
    }
    if (counter==0)
    {
        Console.WriteLine("No positions found to reject.");
        return;
    }
    var positionNumber = Console.ReadLine();
    File.WriteAllText(Path.Join(positions[int.Parse(positionNumber)], "rejection.txt"), DateTime.UtcNow.ToFileTimeUtc().ToString());
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
    var coverLetterTemplate = Template.Parse(File.ReadAllText("base_cover_letter.html"));
    File.WriteAllText("temp_cover_letter.html", coverLetterTemplate.Render(new { company = companyName, position = position }));
    var processInfo = new ProcessStartInfo();
    processInfo.UseShellExecute = false;
    processInfo.WorkingDirectory = Directory.GetCurrentDirectory();
    processInfo.FileName = "wsl.exe";
    processInfo.ArgumentList.Add("wkhtmltopdf");
    processInfo.ArgumentList.Add("-L");
    processInfo.ArgumentList.Add("20mm");
    processInfo.ArgumentList.Add("-T");
    processInfo.ArgumentList.Add("20mm");
    processInfo.ArgumentList.Add("-R");
    processInfo.ArgumentList.Add("20mm");

    processInfo.ArgumentList.Add("temp_cover_letter.html");
    processInfo.ArgumentList.Add("temp_cover_letter.pdf");
    var process = new Process();
    process.StartInfo = processInfo;
    process.Start();
    process.WaitForExit();
    File.Copy("base_resume.pdf", Path.Join(positionDir, "Jay Vilalta.pdf"));
    File.Copy("temp_cover_letter.pdf", Path.Join(positionDir, "Cover Letter.pdf"));

});
newCommand.AddCommand(newApplicationCommand);

newCommand.SetHandler(() =>
{
    Console.WriteLine("new");
});

rootCommand.AddCommand(newCommand);
rootCommand.AddCommand(rejectionCommand);
rootCommand.Invoke(args);