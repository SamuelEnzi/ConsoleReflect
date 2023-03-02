using ConsoleReflect.Attributes;

namespace Example.Commands;

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
