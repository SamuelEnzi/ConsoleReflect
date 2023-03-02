# ConsoleReflect

## Usage
This library can be used to create advanced command-based console applications.
It automatically parses all parameters that can be defined using models

Here is an example model of a command:
```csharp
public class ExampleCommand : ConsoleReflect.Types.ICommand
{
    //this property contains the command string. it is used to identify the main command
    public string CommandString { get; set; } = "example";

    //the help property is used to describe the current command
    public string Help { get; set; } = "this is an example command that contains a few simple parameters";

    /// <summary>
    /// this is a flag example.
    /// if not stated otherwise the default value is false
    /// </summary>
    [CommandProperty("long", "l", "if set print long message [false]")]
    public bool PrintLongMessage { get; set; }

    /// <summary>
    /// this is an example how to get an int as parameter
    /// the default value is 1
    /// </summary>
    [CommandProperty("amount", "n", "the amount of times the message will be repeated [1]")]
    public int Amount { get; set; } = 1;

    /// <summary>
    /// this property will get a string.
    /// it can be set with -m text --message text 
    /// if the text contains whitespaces you can use '"' to escape it
    /// -m "message with whitespaces"
    /// </summary>
    [CommandProperty("message", "m", "prints an additional message at the end [hello world]")]
    public string AdditionalMessage { get; set; } = "hello world";

    /// <summary>
    /// this will just get some double as parameter
    /// </summary>
    [CommandProperty("double", "d", "sets the double value that will be printed at the end of this module [0,5]")]
    public double SomeDoubleSetting { get; set; } = 0.5;

    /// <summary>
    /// every command has an execute method that is invoked one a instance has been created.
    /// when errors are thrown by an command they are forwarded to the host instance and displayed to the user
    /// </summary>
    public void Execute()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        var message = PrintLongMessage ? "This is the long message" : "This is a short message";
        for(int i = 0; i < Amount; i++)
            Console.WriteLine(message);

        Console.WriteLine($"Additional message: '{AdditionalMessage}'");
        Console.WriteLine($"Some double setting: '{SomeDoubleSetting}'");
        Console.ResetColor();

        throw new Exception("this is an example exception to show how errors propergate. if you are in debug mode just click on continue");
    }
}
```

Now you can create the command host.

This is a working example:
```csharp
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
```

Now you can run the application. if you type the `help` command to get a list of all available commands.

if you select one of the defined commands you can get advanced info using either the `--help` or `-h` flag.

this is the output of the following command `example --help`
```
>> example --help

example:
this is an example command that contains a few simple parameters
PrintLongMessage         -l, --long
         if set print long message [false]

Amount                   -n, --amount
         the amount of times the message will be repeated [1]

AdditionalMessage        -m, --message
         prints an additional message at the end [hello world]

SomeDoubleSetting        -d, --double
         sets the double value that will be printed at the end of this module [0,5]
```
