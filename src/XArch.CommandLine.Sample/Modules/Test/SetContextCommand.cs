using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;

namespace XArch.CommandLine.Sample.Modules.Test
{
    [RegisterCommand(command: "context", verb: "set")]
    internal class SetContextCommand : CommandBase
    {
        private static Argument<string> ContextName = new Argument<string>("name", "Context Name");
        public const string CommandNamespace = "Sample";

        protected override void ConfigureCommand(Command command)
        {
            base.ConfigureCommand(command);
            command.AddArgument(ContextName);
        }

        public override void ConfigureServices(IServiceCollection serviceDescriptors)
        {
            base.ConfigureServices(serviceDescriptors);
        }

        protected override Task InvokeAsync(CommandExecutionContext executionContext)
        {
            ICommandContextDataProvider provider = executionContext.Services.GetRequiredService<ICommandContextDataProvider>();
            string contextName = executionContext.InvocationContext.ParseResult.GetValueForArgument<string>(ContextName);

            if (!string.IsNullOrEmpty(contextName))
            {
                provider.Init<SampleContextData>(CommandNamespace, contextName);
                provider.SaveContext();

                provider.SetCurrentContext(CommandNamespace, contextName);

                Console.WriteLine($">> Context Set: {CommandNamespace}:{contextName}");
            }

            return Task.CompletedTask;
        }
    }
}
