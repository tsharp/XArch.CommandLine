using System;
using System.Collections;
using System.Linq;
using System.Text.Json;

namespace XArch.CommandLine.PowerShell
{
    public class IsolatedPowershellHost : IDisposable
    {
        private readonly System.Management.Automation.PowerShell _powerShell;

        public IsolatedPowershellHost()
        {
            // Initialize a new PowerShell instance
            _powerShell = System.Management.Automation.PowerShell.Create();
        }

        /// <summary>
        /// Executes a PowerShell command and returns the results.
        /// </summary>
        /// <param name="command">The PowerShell command to execute.</param>
        /// <returns>The results of the command execution.</returns>
        public string ExecuteCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentException("Command cannot be null or empty.", nameof(command));

            _powerShell.Commands.Clear();
            _powerShell.AddScript(command);

            var results = _powerShell.Invoke<Hashtable>();

            if (_powerShell.HadErrors)
            {
                throw new InvalidOperationException("An error occurred while executing the PowerShell command.");
            }

            return JsonSerializer.Serialize(results.FirstOrDefault(), new JsonSerializerOptions()
            {
                WriteIndented = true,
            });
        }

        /// <summary>
        /// Disposes the PowerShell instance and unloads the isolated context.
        /// </summary>
        public void Dispose()
        {
            _powerShell?.Dispose();
        }
    }
}