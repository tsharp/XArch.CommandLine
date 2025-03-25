using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace XArch.CommandLine
{
    public sealed class CommandRuntimeBuilder
    {
        private readonly ServiceCollection serviceDescriptors = new ServiceCollection();
        private IServiceProvider serviceProvider;
        private readonly IList<Assembly> moduleAssemblies = new List<Assembly>();
        private readonly IList<CommandDescriptor> commandDescriptors = new List<CommandDescriptor>();
        private readonly string serviceDescription;
        private bool enableRepl = false;
        private bool useRepl = false;
        private CommandRuntimeBuilder(string serviceDescription = null)
        {
            this.serviceDescription = serviceDescription ?? "XArch Platform Management Command Line Interface";
        }

        public static CommandRuntimeBuilder Create(string serviceDescription = null)
        {
            return new CommandRuntimeBuilder(serviceDescription);
        }

        public CommandRuntimeBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            configureServices(serviceDescriptors);

            return this;
        }

        public CommandRuntimeBuilder RegisterModuleAssembly(Assembly assembly)
        {
            if (!moduleAssemblies.Contains(assembly))
            {
                moduleAssemblies.Add(assembly);
            }

            return this;
        }

        public CommandRuntimeBuilder EnableRepl(bool enable = true)
        {
            enableRepl = enable;

            return this;
        }

        public CommandRuntimeBuilder UseRepl()
        {
            useRepl = true;

            return this;
        }

        public ICommandRuntime Build()
        {
            CommandRuntime runtime = new CommandRuntime(useRepl, enableRepl);
            runtime.RootCommand.Description = serviceDescription;
            runtime.RootCommand.TreatUnmatchedTokensAsErrors = true;

            serviceDescriptors.AddScoped<ICommandContextDataProvider, CommandContextDataProvider>();

            foreach (var assembly in moduleAssemblies)
            {
                this.GenerateAssemblyCommands(runtime.RootCommand, assembly);
            }

            this.RegisterCommandHandlers();

            return runtime;
        }

        private struct CommandDescriptor
        {
            public Command ParentCommand { get; set; }
            public CommandBase CommandHandler { get; set; }
        }

        private Command GetOrCreateSubCommand(Command parentCommand, string command)
        {
            var subCommand = parentCommand.Children.Where(c => c.Name == command && c is Command).FirstOrDefault() as Command;

            if (subCommand == null)
            {
                subCommand = new Command(command)
                {
                    TreatUnmatchedTokensAsErrors = true
                };

                parentCommand.Add(subCommand);
            }

            return subCommand;
        }

        private void GenerateAssemblyCommands(RootCommand rootCommand, Assembly assembly)
        {
            IDictionary<string, Command> rootCommandMap = new Dictionary<string, Command>();

            var classes = assembly.GetTypes()
                .Where(t => typeof(CommandBase).IsAssignableFrom(t) && t.GetCustomAttribute<RegisterCommandAttribute>(false) != null)
                .ToList();

            foreach (var @class in classes)
            {
                RegisterCommandAttribute attribute = @class.GetCustomAttribute<RegisterCommandAttribute>()!;
                Command parentCommand = rootCommand;

                if (!attribute.IsEnabled)
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(attribute.Module))
                {
                    parentCommand = this.GetOrCreateSubCommand(parentCommand, attribute.Module);
                }

                if (!string.IsNullOrWhiteSpace(attribute.Verb))
                {
                    parentCommand = this.GetOrCreateSubCommand(parentCommand, attribute.Verb);
                }

                CommandBase instance = (Activator.CreateInstance(@class, new object[] { }) as CommandBase)!;
                instance.ConfigureServices(serviceDescriptors);
                commandDescriptors.Add(new CommandDescriptor()
                {
                    CommandHandler = instance,
                    ParentCommand = parentCommand
                });
            }
        }

        private void RegisterCommandHandlers()
        {
            // Services and configure commands now ...
            serviceProvider = serviceDescriptors.BuildServiceProvider();

            foreach (var commandDescriptor in commandDescriptors)
            {
                var command = commandDescriptor.CommandHandler.RegisterCommand(serviceProvider);

                if (command != null)
                {
                    commandDescriptor.ParentCommand.Add(command);
                }
            }
        }
    }
}
