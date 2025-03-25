param (
    [ValidateSet("pwsh", "powershell")]
    [string]$Runtime = "pwsh", # Default to PowerShell Core
    [string]$ModuleDirectory, # Optional: Custom module directory
    [string]$InlineScript, # Inline script to execute
    [string]$ScriptFile, # Path to a script file to execute
    [switch]$KeepSessionOpen # Flag to keep the session open
)

# Use the provided module directory or default to a temporary directory
if (-not $ModuleDirectory) {
    $ModuleDirectory = Join-Path -Path $env:TEMP -ChildPath "IsolatedModules"
}

# Create the module directory if it doesn't exist
if (-Not (Test-Path $ModuleDirectory)) {
    New-Item -ItemType Directory -Path $ModuleDirectory | Out-Null
}

# Validate the runtime
if ($Runtime -notin @("pwsh", "powershell")) {
    Write-Error "Invalid runtime specified. Use 'pwsh' for PowerShell Core or 'powershell' for Windows PowerShell."
    exit 1
}

# Validate that either InlineScript or ScriptFile is provided
if (-not $InlineScript -and -not $ScriptFile) {
    Write-Error "You must provide either an InlineScript or a ScriptFile to execute."
    exit 1
}

# Validate that ScriptFile exists if provided
if ($ScriptFile -and -not (Test-Path $ScriptFile)) {
    Write-Error "The specified ScriptFile does not exist: $ScriptFile"
    exit 1
}

# Start a new isolated PowerShell session
& $Runtime -NoProfile -Command {
    param ($ModuleDirectory, $InlineScript, $ScriptFile, $KeepSessionOpen)
    
    # Override the PSModulePath to isolate modules
    $env:PSModulePath = $ModuleDirectory

    # Confirm isolation
    Write-Host "Isolated PowerShell session started."
    Write-Host "PSModulePath: $env:PSModulePath"
    Write-Host "Loaded Modules: $(Get-Module -ListAvailable | Select-Object -ExpandProperty Name)"
    
    # Execute the provided script
    if ($InlineScript) {
        Write-Host "Executing inline script..."
        Invoke-Expression $InlineScript
    } elseif ($ScriptFile) {
        Write-Host "Executing script file: $ScriptFile"
        . $ScriptFile
    }

    # Keep the session open if the flag is set
    if ($KeepSessionOpen) {
        Write-Host "Keeping the session open. Type 'exit' to close."
        if ($env:PSModulePath -notlike "*$ModuleDirectory*") {
            $env:PSModulePath = "$env:PSModulePath;$ModuleDirectory"
        }
        $Host.EnterNestedPrompt()
    }
} -Args $ModuleDirectory, $InlineScript, $ScriptFile, $KeepSessionOpen