using System;

namespace ConsoleReflect.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CommandPropertyAttribute : Attribute
    {
        public string LongName { get; set; }
        public string ShortName { get; set; }
        public string Help { get; set; }

        public CommandPropertyAttribute(string longName, string shortName, string help = null)
        {
            LongName = longName;
            ShortName = shortName;
            Help = help;
        }
    }
}
