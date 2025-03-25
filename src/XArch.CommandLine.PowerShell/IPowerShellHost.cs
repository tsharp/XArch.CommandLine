using System;

namespace XArch.CommandLine.PowerShell
{
    public interface IPowerShellHost : IDisposable
    {
        string ExecuteCommand(string command);
    }
}
