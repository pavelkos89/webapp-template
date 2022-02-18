# Introduction

// TODO

## Installation

Installed using Chocolatey or Web Deploy. See the _Web Deploy_ section in the system installation documentation for details.

## Chocolatey parameters

All Web Deploy parameters can be used. (See below.) In addition, these parameters may also be used:

| Parameter | Description |
| --------- | ----------- |
| IIS_WebSite | Optional. The existing web-site where the application should be created. Default: "Default Web Site". Note that if overridden, this parameter CAN NOT contain spaces. |
| IIS_Application | Optional. The "folder" name of the web-application. Default: "nCore" |
| IIS_WebDeployParamFile | Optional. Path to a custom web deploy parameter file that should be used when doing the deployment. |
| AppPool_Name | Optional. The name of a dedicated application pool for the web-application. |
| AppPool_Username | Optional. The user-name of the application pool. |
| AppPool_Password | Optional. The user-password of the application pool. |

## Web Deploy parameters

| Parameter | Description |
| --------- | ----------- |
| ConfigServer.BaseUrl | Configuration server base url. |
| ConfigServer.Password | Configuration server password. Must match the configured value in the configuration server. |
| ConfigServer.Scopes | Configuration scopes (comma separated) |
