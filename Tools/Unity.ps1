[CmdletBinding()]
param(
    [ValidateSet('Open', 'Validate')]
    [string]$Action = 'Open',
    [string]$EditorPath
)

$ErrorActionPreference = 'Stop'
$taskProjectRoot = Split-Path -Parent $PSScriptRoot
$taskVersionFile = Join-Path $taskProjectRoot 'ProjectSettings/ProjectVersion.txt'
$taskVersionMatch = Select-String -LiteralPath $taskVersionFile -Pattern '^m_EditorVersion: (.+)$'
if (-not $taskVersionMatch) {
    throw "Cannot read the Unity editor version from $taskVersionFile"
}
$taskEditorVersion = $taskVersionMatch.Matches[0].Groups[1].Value.Trim()

if (-not $EditorPath) {
    $EditorPath = Join-Path $env:ProgramFiles "Unity/Hub/Editor/$taskEditorVersion/Editor/Unity.exe"
}
if (-not (Test-Path -LiteralPath $EditorPath -PathType Leaf)) {
    throw "Unity $taskEditorVersion was not found. Supply -EditorPath with the full path to Unity.exe."
}

if ($Action -eq 'Open') {
    Start-Process -FilePath $EditorPath -WorkingDirectory $taskProjectRoot -ArgumentList @(
        '-projectPath', ('"{0}"' -f $taskProjectRoot)
    ) -WindowStyle Normal | Out-Null
    Write-Output "Opening $taskProjectRoot with Unity $taskEditorVersion."
    return
}

$taskLogsDirectory = Join-Path $taskProjectRoot 'Logs'
New-Item -ItemType Directory -Path $taskLogsDirectory -Force | Out-Null
$taskLogPath = Join-Path $taskLogsDirectory 'ProjectValidation.log'
$taskEditorProcess = Start-Process -FilePath $EditorPath -WorkingDirectory $taskProjectRoot -ArgumentList @(
    '-batchmode', '-nographics', '-quit',
    '-projectPath', ('"{0}"' -f $taskProjectRoot),
    '-executeMethod', 'TheWorld.Editor.ProjectSetup.ValidateProject',
    '-logFile', ('"{0}"' -f $taskLogPath)
) -WindowStyle Hidden -PassThru -Wait

if ($taskEditorProcess.ExitCode -ne 0) {
    throw "Unity validation failed (exit $($taskEditorProcess.ExitCode)). See $taskLogPath"
}
if (-not (Select-String -LiteralPath $taskLogPath -SimpleMatch '[The World] Project validation passed:')) {
    throw "Unity did not report successful validation. See $taskLogPath"
}
Write-Output "Unity project validation passed. Log: $taskLogPath"
