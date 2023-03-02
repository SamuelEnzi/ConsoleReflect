using System.Collections.Generic;

namespace ConsoleReflect.Models
{
    public class CommandModel
    {
        public string Command { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

        public CommandModel()
        {

        }
    }
}
