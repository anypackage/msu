# AnyPackage.Msu

AnyPackage.Msu is a Windows Msu provider for AnyPackage.

## Install AnyPackage.Msu

```PowerShell
Install-PSResource AnyPackage.Msu
```

## Import AnyPackage.Msu

```PowerShell
Import-Module AnyPackage.Msu
```

## Sample usages

### Get list of installed packages

```PowerShell
Get-Package -Name KB12456789
```

### Get Msu metadata

```powershell
Find-Package -Path C:\windows10.0-kb5028853-x64-ndp48_6d85da3883386e6e72037cca91eb745df82bbd86.msu
```

### Install Msu

```powershell
Install-Package -Path C:\windows10.0-kb5028853-x64-ndp48_6d85da3883386e6e72037cca91eb745df82bbd86.msu
```

### Uninstall Msu

> Note! Windows no longer allows uninstalling KBs silently.
A dialog box will appear to confirm removal.

```powershell
Uninstall-Package -Name KB5028853
```
