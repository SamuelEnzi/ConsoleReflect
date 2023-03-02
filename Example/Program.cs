using ConsoleReflect;
using ConsoleReflect.Models;
using Example.Commands;

/*
 * here i created a new instance of the Host class.
 * this takes care of parsing and invoking commands.
 */
var host = new Host(
        new Type[]
        {
            //all commandtypes need to be passed over as an Type[]
            //you could get all matching classes by using reflection avoiding to write this manually
            //but that is up to you
            typeof(ExampleCommand)
        });

//this event will always trigger once a help flag was set in a specific command
//by hooking you can display help messages to the user
host.HelpCommand += DisplayHelpCommand;

//this event will trigger when the user types help as an command
//this can be used to enumerate all available commands
host.ListCommands += DisplayAllCommands;

//here we start looping and get user input
while (true)
{
    Console.Write(">> ");
    host.Propergate(Console.ReadLine());
}

#region display methods
void DisplayAllCommands(object sender, List<(Type type, string command, string help)> commands)
{
    foreach (var c in commands)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write($"{c.command}: ");
        Console.ResetColor();
        Console.WriteLine($"{c.help}");
    }
}

void DisplayHelpCommand(object sender, (Type type, string command, string help) target, CommandModel cmd)
{
    var obj = Activator.CreateInstance(target.type);
    var properties = obj.GetCommandProperies().ToList();
    Console.WriteLine();
    Console.WriteLine($"{target.command}:", Console.ForegroundColor = ConsoleColor.DarkYellow);
    Console.WriteLine($"{target.help}", Console.ForegroundColor = ConsoleColor.DarkGray);

    foreach (var prop in properties)
    {
        Console.ResetColor();
        Console.WriteLine($"{prop.prop.Name} \t -{prop.attribute.ShortName}, --{prop.attribute.LongName}");
        Console.WriteLine($"\t {prop.attribute.Help}\n", Console.ForegroundColor = ConsoleColor.DarkGray);
    }
    Console.ResetColor();
}
#endregion