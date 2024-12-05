using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Threading.Tasks;

namespace XArch.CommandLine
{
    internal sealed class CommandRuntime : ICommandRuntime
    {
        private sealed class SwitchOption : Option<bool>
        {
            public SwitchOption(string alias, string description)
                : base(new[] { alias }, description)
            {
            }
        }

        internal readonly RootCommand RootCommand = new RootCommand();

        private readonly SwitchOption Interactive = new SwitchOption("--repl", "Run in REPL mode")
        {
            IsRequired = false
        };

        private readonly bool enableRepl = false;
        private readonly bool useRepl = false;

        public CommandRuntime(bool useRepl = false, bool enableRepl = false)
        {
            if (!useRepl && enableRepl)
            {
                RootCommand.AddGlobalOption(Interactive);
            }

            this.enableRepl = enableRepl;
            this.useRepl = useRepl;
        }

        public async Task RunAsync(params string[] args)
        {
            // NOTE: You can parse the command line arguments here instead of invoking
            var result = RootCommand.Parse(args);

            if (!useRepl && !result.GetValueForOption(Interactive))
            {
                await RootCommand.InvokeAsync(args);

                return;
            }

            Console.WriteLine(">> Interactive Mode <<");

            while (true)
            {
                string? input = string.Empty;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(":>> ");
                Console.ResetColor();

                input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid Input");
                    Console.ResetColor();

                    continue;
                }

                if (string.Equals(input, "exit", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }

                if (string.Equals(input, "clear", StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.Clear();
                    continue;
                }

                try
                {
                    await RootCommand.InvokeAsync(input);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                }
            }
        }
    }
}
