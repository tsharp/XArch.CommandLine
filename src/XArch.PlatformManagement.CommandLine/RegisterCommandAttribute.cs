using System;
using System.Text.RegularExpressions;

namespace XArch.PlatformManagement.CommandLine
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class RegisterCommandAttribute : Attribute
    {
        //private readonly string[] ApprovedVerbs = new string[]
        //{
        //    "list",
        //    "create",
        //    "update",
        //    "delete",
        //    "get",
        //    "set",
        //    "apply",
        //    "add",
        //    "remove"
        //};

        public string Module { get; } = string.Empty;

        public string Verb { get; }

        public string Command { get; }

        public string? Description { get; }

        public bool IsEnabled { get; } = true;

        public RegisterCommandAttribute(string command, string? description = null, string? module = null, string verb = null, bool isEnabled = true)
        {
            Command = command.ToLowerInvariant();
            Description = description;
            IsEnabled = isEnabled;

            if (string.IsNullOrWhiteSpace(command) || !IsValidCommandName(command))
            {
                throw new Exception("Command Names can only contain letters and numbers.");
            }

            //if (!ApprovedVerbs.Contains(verb))
            //{
            //    throw new Exception("Must be an approved verb.");
            //}

            if (module != null && (string.IsNullOrWhiteSpace(module) || !IsValidCommandName(module)))
            {
                throw new Exception("Module Names can only contain letters and numbers.");
            }

            if (verb != null && (string.IsNullOrWhiteSpace(verb) || !IsValidCommandName(verb)))
            {
                throw new Exception("Verb Names can only contain letters and numbers.");
            }

            Module = (module ?? string.Empty).ToLowerInvariant();
            Verb = (verb ?? string.Empty).ToLowerInvariant();
        }

        public static bool IsValidCommandName(string commandName)
            => Regex.IsMatch(commandName, "^[a-z0-9-]+$");
    }
}
