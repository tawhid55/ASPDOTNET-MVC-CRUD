$ErrorActionPreference = 'Stop'
$projectPath = Join-Path $PSScriptRoot '../EmployeeManagement'
$adminCredential = Get-Credential -Message 'Enter a new administrator email and password'
if ($null -eq $adminCredential) { return }
$previousEnvironment = $env:ASPNETCORE_ENVIRONMENT
try {
    $env:ASPNETCORE_ENVIRONMENT = 'Development'
    $env:Admin__Email = $adminCredential.UserName
    $env:Admin__Password = $adminCredential.GetNetworkCredential().Password
    dotnet run --no-build --no-launch-profile --project $projectPath -- --create-admin
    if ($LASTEXITCODE -ne 0) { throw 'Administrator creation failed.' }
}
finally {
    Remove-Item Env:Admin__Email, Env:Admin__Password -ErrorAction SilentlyContinue
    $env:ASPNETCORE_ENVIRONMENT = $previousEnvironment
}
