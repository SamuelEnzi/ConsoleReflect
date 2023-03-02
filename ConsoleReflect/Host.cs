using ConsoleReflect.Attributes;
using ConsoleReflect.Models;
using ConsoleReflect.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConsoleReflect
{
    /// <summary>
    /// once the constructor has been called this class will automatically load all required information from the commands.
    /// it uses reflections to get both the commandstring and help that is later used to identify the command
    /// </summary>
    public class Host
    {
        public delegate void ListCommandsEventHandler(object sender, List<(Type type, string command, string help)> commands);
        public delegate void HelpCommandEventHandler(object sender, (Type type, string command, string help) target, CommandModel cmd);
        
        private List<(Type type, string command, string help)> commandsCollection { get; set; } = new List<(Type type, string command, string help)>();

        /// <summary>
        /// this event will trigger when the user enters help or h as command.
        /// it should be used to display all commands.
        /// </summary>
        public event ListCommandsEventHandler ListCommands;

        /// <summary>
        /// this event will be triggerd when the --help or -h flag was set in a command.
        /// it contains all command information like properties and its help info
        /// </summary>
        public event HelpCommandEventHandler HelpCommand;


        public Host(IEnumerable<Type> commands)
        {
            foreach (Type c in commands)
                if (typeof(Types.ICommand).IsAssignableFrom(c))
                {
                    var obj = Activator.CreateInstance(c);
                    var cmd = c.GetProperty("CommandString").GetValue(obj).ToString();
                    var help = c.GetProperty("Help").GetValue(obj).ToString();
                    commandsCollection.Add((c, cmd, help));
                }
        }

        /// <summary>
        /// when called it will interpret the input parameter as command.
        /// it automatically parses every property and reflects it into a new instance of target command object.
        /// help and h are two reserved keywords. they will always trigger the help event
        /// </summary>
        /// <param name="input"></param>
        public void Propergate(string input)
        {
            if (input.Trim() == "help" || input.Trim() == "h")
            {
                ListCommands?.Invoke(this, commandsCollection);
                return;
            }
            try
            {
                var cmd = Interpreter.Interpret(input);
                var help = cmd.Parameters.ContainsKey("--help") || cmd.Parameters.ContainsKey("-h");

                foreach (var command in commandsCollection)
                    if (command.command == cmd.Command)
                    {
                        if (help)
                            HelpCommand?.Invoke(this, command, cmd);
                        else
                            Execute(command, cmd);
                        return;
                    }
            }
            catch (TargetInvocationException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{ex.InnerException.Source}] {ex.InnerException.Message}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{typeof(Host).Name}] {ex.Message}");
                Console.ResetColor();
            }
        }

        private void Execute((Type type, string command, string help) target, Models.CommandModel command)
        {
            var obj = Activator.CreateInstance(target.type);

            var properties = obj.GetCommandProperies().ToList();

            foreach (var arg in command.Parameters)
            {
                List<(PropertyInfo prop, CommandPropertyAttribute attribute)> targetProperties = properties.Where((prop) => prop.attribute.ShortName == arg.Key.Trim().Substring(1) || prop.attribute.LongName == arg.Key.Trim().Substring(2)).ToList();
                if (targetProperties.Count() <= 0)
                {
                    throw new Exception($"Unknown parameter '{arg.Key}'");
                };
                var targetPropertie = targetProperties.First();

                if (targetPropertie.prop.PropertyType == typeof(System.Boolean))
                    targetPropertie.prop.SetValue(obj, true);
                else if (targetPropertie.prop.PropertyType == typeof(System.String))
                    targetPropertie.prop.SetValue(obj, arg.Value);
                else if (targetPropertie.prop.PropertyType == typeof(System.Int32))
                    targetPropertie.prop.SetValue(obj, int.Parse(arg.Value));
                else if (targetPropertie.prop.PropertyType == typeof(System.Double))
                    targetPropertie.prop.SetValue(obj, double.Parse(arg.Value));
            }

            target.type.GetMethod("Execute").Invoke(obj, null);
        }
    }
}
