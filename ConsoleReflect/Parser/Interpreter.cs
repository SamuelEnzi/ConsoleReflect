using ConsoleReflect.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ConsoleReflect.Parser
{
    internal static class Interpreter
    {
        private static int registerCounter = 0;

        public static CommandModel Interpret(string command)
        {
            registerCounter = 0;

            var filterdCommand = Sanitize(command);
            var chuncks = filterdCommand.sanitized.Split(' ');

            var result = new CommandModel();
            result.Command = chuncks[0].Trim();

            foreach (var shortParam in FindShortParameters(chuncks))
            {
                var key = shortParam.name;
                var value = "";
                if (chuncks.Length > shortParam.index + 1)
                {
                    if (!chuncks[shortParam.index + 1].StartsWith("<STR-[") && !chuncks[shortParam.index + 1].StartsWith("-") && !chuncks[shortParam.index + 1].StartsWith("--"))
                    {
                        value = chuncks[shortParam.index + 1].Trim();
                    }
                    else if (chuncks[shortParam.index + 1].StartsWith("<STR-["))
                    {
                        try
                        {
                            value = filterdCommand.stringRegister[chuncks[shortParam.index + 1].Trim()];
                        }
                        catch { }
                    }
                }

                result.Parameters.Add(key, value);
            }

            foreach (var longParam in FindLongParameters(chuncks))
            {
                var key = longParam.name;
                var value = "";
                if (chuncks.Length > longParam.index + 1)
                {
                    if (!chuncks[longParam.index + 1].StartsWith("<STR-[") && !chuncks[longParam.index + 1].StartsWith("-") && !chuncks[longParam.index + 1].StartsWith("--"))
                    {
                        value = chuncks[longParam.index + 1].Trim();
                    }
                    else if (chuncks[longParam.index + 1].StartsWith("<STR-["))
                    {
                        try
                        {
                            value = filterdCommand.stringRegister[chuncks[longParam.index + 1].Trim()];
                        }
                        catch { }
                    }
                }

                result.Parameters.Add(key, value);
            }

            return result;
        }

        private static IEnumerable<(int index, string name)> FindShortParameters(string[] chuncks)
        {
            for (int i = 0; i < chuncks.Length; i++)
                if (Regex.Match(chuncks[i].Trim(), "^-[0-9a-zA-Z]+").Success)
                {
                    var ps = chuncks[i].Substring(1);
                    foreach (var c in ps)
                        yield return (i, $"-{c}");
                }
        }

        private static IEnumerable<(int index, string name)> FindLongParameters(string[] chuncks)
        {
            for (int i = 0; i < chuncks.Length; i++)
                if (Regex.Match(chuncks[i].Trim(), "--[0-9a-zA-Z-]+").Success)
                    yield return (i, $"{chuncks[i].Trim()}");
        }

        private static (string sanitized, Dictionary<string, string> stringRegister) Sanitize(string rawCommand)
        {
            var stringRegister = new Dictionary<string, string>();
            var sanitized = rawCommand.Trim();

            while (true)
            {
                var res = FilterNext(sanitized);
                if (res == null) break;

                sanitized = res.Value.output;
                stringRegister.Add(res.Value.key, res.Value.value);
            }
            return (sanitized, stringRegister);
        }

        private static (string output, string key, string value)? FilterNext(string raw)
        {

            bool fold = false;
            int start = 0;

            for (int i = 0; i < raw.Length; i++)
            {
                if (raw[i] == '"')
                {
                    if (fold == false)
                    {
                        fold = true;
                        start = i;
                    }
                    else
                    {
                        registerCounter++;
                        var name = $"<STR-[{registerCounter}]>";
                        fold = false;
                        var length = i - start;
                        var value = raw.Substring(start + 1, length - 1);
                        var replaced = raw.Substring(0, start).Trim() + $" {name} " + raw.Substring(i + 1, raw.Length - i - 1).Trim();
                        return (replaced, name, value);
                    }
                }
            }

            return null;
        }
    }
}
