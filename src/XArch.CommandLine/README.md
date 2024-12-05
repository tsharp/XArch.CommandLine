# XArch.CommandLine

XArch.CommandLine is a library that provides a basic framework for building command-line applications. It is built on top of the `System.CommandLine` library and provides a more structured way to build command-line applications.

## Features
- Dependency injection
- Module-based architecture
- REPL support
- Command-line parsing and completion support from `System.CommandLine`
- Context-aware commands and ability to save state between commands

## Example
```CSharp
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
```