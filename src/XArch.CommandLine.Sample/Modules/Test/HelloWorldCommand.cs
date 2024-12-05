using XArch.CommandLine;

namespace XArch.CommandLine.Sample.Modules.Test
{
    [RegisterCommand(command: "hello")]
    internal class HelloWorldCommand : CommandBase
    {
        protected override async Task InvokeAsync(CommandExecutionContext executionContext)
        {
            Console.WriteLine("Hello, World!");
        }
    }
}
