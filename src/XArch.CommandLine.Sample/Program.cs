using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace XArch.CommandLine.Sample
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await CommandRuntimeBuilder
                .Create()
                .ConfigureServices(ConfigureServices)
                .RegisterModuleAssembly(Assembly.GetExecutingAssembly())
                .UseRepl()
                .Build()
                .RunAsync(args);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
        }
    }
}
