using System;
using System.CommandLine;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace XArch.CommandLine
{
    public abstract class CommandBase
    {
        public virtual void ConfigureServices(IServiceCollection serviceDescriptors)
        {
        }

        protected virtual void ConfigureCommand(Command command)
        {
        }

        protected abstract Task InvokeAsync(CommandExecutionContext executionContext);

        internal Command? RegisterCommand(IServiceProvider rootServiceProvider)
        {
            var attribute = this.GetType().GetCustomAttribute<RegisterCommandAttribute>(false);

            if (attribute == null)
            {
                return null;
            }

            Command command = new Command(attribute!.Command, attribute.Description);
            this.ConfigureCommand(command);
            command.SetHandler(async (context) =>
            {
                using (var scope = rootServiceProvider.CreateAsyncScope())
                {
                    using (var executionContext = new CommandExecutionContext(scope.ServiceProvider, context))
                    {
                        await this.InvokeAsync(executionContext);
                    }
                }
            });

            return command;
        }
    }
}
