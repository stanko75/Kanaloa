
$ErrorActionPreference = "Stop"

# Configuration (asked once, then stored next to the script)
$configFile = Join-Path $PSScriptRoot "publishRemote.config.json"

$settings = [ordered]@{
    project       = "Path to .csproj"
    publishFolder = "Local publish folder"
    server        = "Server address"
    user          = "Server user"
    password      = "Server password"
    remoteFolder  = "Remote folder"
    hostKey       = "SSH host key fingerprint"
    winscp        = "Path to WinSCP.com"
}

$config = @{}
if (Test-Path $configFile) {
    $loaded = Get-Content $configFile -Raw | ConvertFrom-Json
    foreach ($p in $loaded.PSObject.Properties) { $config[$p.Name] = $p.Value }
}

# The password is stored encrypted (Windows DPAPI, current user only)
$changed = $false
foreach ($key in $settings.Keys) {
    if ([string]::IsNullOrWhiteSpace($config[$key])) {
        if ($key -eq "password") {
            do {
                $secure = Read-Host $settings[$key] -AsSecureString
            } while ($secure.Length -eq 0)
            $config[$key] = ConvertFrom-SecureString $secure
        }
        else {
            do {
                $answer = Read-Host $settings[$key]
            } while ([string]::IsNullOrWhiteSpace($answer))
            $config[$key] = $answer
        }
        $changed = $true
    }
}

if ($changed) {
    $config | ConvertTo-Json | Set-Content $configFile -Encoding UTF8
}

$project = $config.project
$publishFolder = $config.publishFolder
$server = $config.server
$user = $config.user
$remoteFolder = $config.remoteFolder
$hostKey = $config.hostKey
$winscp = $config.winscp
$securePassword = ConvertTo-SecureString $config.password
$password = [System.Net.NetworkCredential]::new("", $securePassword).Password

# 1. Publish
Write-Host "Publishing .NET 10 application..."

dotnet publish $project `
    -c Release `
    -f net10.0 `
    -r linux-x64 `
    --self-contained false `
    -o $publishFolder

if ($LASTEXITCODE -ne 0) {
    throw "Publish failed!"
}

# 2. Upload via SFTP
Write-Host "Uploading to server..."

if (!(Test-Path $winscp)) {
    throw "WinSCP not found!"
}

if ($hostKey -like "HIER_*") {
    throw "Please configure the SSH host key fingerprint!"
}

# Authenticates with the stored user and password.
$scriptFile = Join-Path $env:TEMP "kanaloa-upload.txt"
$encUser = [uri]::EscapeDataString($user)
$encPassword = [uri]::EscapeDataString($password)
# sudo reads the password from stdin (-S) since there is no terminal
$shellPassword = $password.Replace("'", "'\''")
$restartCommand = "echo '$shellPassword' | sudo -S -p '' systemctl restart kanaloa"

@"
option batch abort
option confirm off
open sftp://${encUser}:${encPassword}@${server}/ -hostkey="$hostKey"
synchronize remote "$publishFolder" "$remoteFolder"
call $restartCommand
exit
"@ | Set-Content $scriptFile -Encoding UTF8

try {
    & $winscp "/script=$scriptFile"

    if ($LASTEXITCODE -ne 0) {
        throw "SFTP upload failed!"
    }

    Write-Host "Deployment completed successfully!"
}
finally {
    Remove-Item $scriptFile -ErrorAction SilentlyContinue
}
