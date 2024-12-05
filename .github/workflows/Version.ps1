echo "output_${version}=${version}" >> "$GITHUB_OUTPUT"

$workspace = Resolve-Path "$PSScriptRoot\..\.."
Write-Host $workspace

# Install GitVersion tool globally
dotnet tool install --global GitVersion.Tool --version 6.*

# Add the .dotnet/tools to the PATH
# $env:PATH += ":~/.dotnet/tools"
# ls -la ~/.dotnet/tools

# Execute GitVersion with specified configuration
# dotnet-gitversion -output json -nofetch -l console -config "${{ github.workspace }}/GitVersion.yml" -targetpath "${{ github.workspace }}" -nocache -nonormalize

# If you need to capture the output for further use, you can do something like this:
$gitVersionRaw = (dotnet gitversion -output json -nofetch -config "$workspace/GitVersion.yml" -targetpath "$workspace" -nocache -nonormalize) | Out-String
Write-Host $gitVersionRaw
$gitVersion = ConvertFrom-Json $gitVersionRaw
$gitVersion.AssemblySemFileVer
$gitVersion.AssemblySemVer
$gitVersion.MajorMinorPatch
$gitVersion.FullSemVer
$gitVersion.SemVer