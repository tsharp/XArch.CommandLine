$workspace = Resolve-Path "$PSScriptRoot\..\.."
$gitVersionRaw = (dotnet gitversion -output json -nofetch -config "$workspace/GitVersion.yml" -targetpath "$workspace" -nocache -nonormalize) | Out-String
$gitVersion = ConvertFrom-Json $gitVersionRaw
$gitVersionRaw
$version = "$($gitVersion.MajorMinorPatch)"
$semVer = "$($gitVersion.MajorMinorPatch)-rc.$($gitVersion.PreReleaseTag)"
Write-Host "Version: $version" -ForegroundColor Cyan

$projectXml = @"
<Project>
    <PropertyGroup>
        <AssemblyVersion>$version</AssemblyVersion>
        <AssemblyFileVersion>$semVer</AssemblyFileVersion>
        <PackageVersion>$semVer</PackageVersion>
    </PropertyGroup>
</Project>
"@

$projectXml | Out-File -FilePath "../../Directory.Version.props"