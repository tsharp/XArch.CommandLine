using System.Threading.Tasks;

namespace XArch.CommandLine
{
    public interface ICommandRuntime
    {
        public Task RunAsync(params string[] args);
    }
}
