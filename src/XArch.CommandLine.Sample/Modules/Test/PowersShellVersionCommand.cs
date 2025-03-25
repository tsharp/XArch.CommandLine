using Microsoft.Extensions.DependencyInjection;
using XArch.CommandLine.PowerShell;

namespace XArch.CommandLine.Sample.Modules.Test
{
    [RegisterCommand(command: "version", module: "ps", verb: "get")]
    internal class PowersShellVersionCommand : CommandBase
    {
        protected override Task InvokeAsync(CommandExecutionContext executionContext)
        {
            IsolatedPowershellHost host = executionContext.Services.GetRequiredService<IsolatedPowershellHost>();
            Console.WriteLine(host.ExecuteCommand("$PSVersionTable"));

            return Task.CompletedTask;
        }
    }
}
