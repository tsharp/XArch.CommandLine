using System.Threading.Tasks;

namespace XArch.PlatformManagement.CommandLine
{
    public interface ICommandRuntime
    {
        public Task RunAsync(params string[] args);
    }
}
