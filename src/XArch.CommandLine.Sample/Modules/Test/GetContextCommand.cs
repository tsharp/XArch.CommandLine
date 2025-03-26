using System.CommandLine;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace XArch.CommandLine.Sample.Modules.Test
{
    [RegisterCommand(command: "context", verb: "get")]
    internal class GetContextCommand : CommandBase
    {
        public const string CommandNamespace = "Sample";

        protected override void ConfigureCommand(Command command)
        {
            base.ConfigureCommand(command);
        }

        protected override Task InvokeAsync(CommandExecutionContext executionContext)
        {
            ICommandContextDataProvider provider = executionContext.Services.GetRequiredService<ICommandContextDataProvider>();
            SampleContextData data = provider.LoadCurrentContext<SampleContextData>(CommandNamespace);
            Console.WriteLine(">> Context Data:");
            Console.WriteLine(JsonSerializer.Serialize(data, new JsonSerializerOptions()
            {
                WriteIndented = true
            }));

            return Task.CompletedTask;
        }
    }
}
