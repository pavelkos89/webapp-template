$ErrorActionPreference = 'Stop'; # stop on all errors
# Depends on it is used during creating package or installing/uninstalling package
if(Test-Path $PSScriptRoot\Helpers.Arguments.Install.ps1) {
	. $PSScriptRoot\Helpers.Arguments.Install.ps1
}
else {
	# Hardcoded path. Root folder should contain Chocolatey, Shared.Chocolatey, solution folders and files
	$sourceFolder = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..\..')) 
	. $sourceFolder\Shared.Chocolatey\Helpers\Helpers.Arguments.Install.ps1
}
if($packageName -eq $null) {
	$packageName= '${{ values.component_id }}'
}
if($deployType -eq $null) {
	$deployType = 'WebApp'
}

$toolsDir   = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$projectBinPath = "${{ values.component_id }}/bin/Release/PublishOutput/WebDeploy/${{ values.component_id }}"
$nameOfExecutable = "${{ values.component_id }}.dll"

$webAppSite  =         GetInstallParameter "IIS_WebSite"     "Default Web Site"
$webAppName  =         GetInstallParameter "IIS_Application" "${{ values.component_id }}"
$webDeployParamFile =  GetInstallParameter "IIS_WebDeployParamFile" $null
$appPoolName =         GetInstallParameter "AppPool_Name"    "Reserved for $webAppName"
$appPoolUserName =     GetInstallParameter "AppPool_Username"    ""
$appPoolUserPassword = GetInstallParameter "AppPool_Password"    ""