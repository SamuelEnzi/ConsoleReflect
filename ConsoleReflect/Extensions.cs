using ConsoleReflect.Attributes;
using System.Collections.Generic;
using System.Reflection;

namespace ConsoleReflect
{
    public static class Extensions
    {
        public static IEnumerable<(PropertyInfo prop, CommandPropertyAttribute attribute)> GetCommandProperies(this object target)
        {
            foreach (var prop in target.GetType().GetProperties())
                foreach (var attr in prop.GetCustomAttributes(true))
                    if (attr.GetType() == typeof(CommandPropertyAttribute))
                        yield return (prop, (CommandPropertyAttribute)attr);
        }
    }
}
