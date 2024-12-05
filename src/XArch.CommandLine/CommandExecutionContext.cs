using System;
using System.CommandLine.Invocation;

namespace XArch.CommandLine
{
    public class CommandExecutionContext : IDisposable
    {
        internal CommandExecutionContext(IServiceProvider serviceProvider, InvocationContext invocationContext)
        {
            Services = serviceProvider;
            InvocationContext = invocationContext;
        }

        public IServiceProvider Services { get; }

        public InvocationContext InvocationContext { get; }

        public void Dispose()
        {
        }
    }
}
