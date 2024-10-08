# AnyPackage.Msu

[![gallery-image]][gallery-site]
[![build-image]][build-site]
[![cf-image]][cf-site]

[gallery-image]: https://img.shields.io/powershellgallery/dt/AnyPackage.Msu
[build-image]: https://img.shields.io/github/actions/workflow/status/anypackage/msu/ci.yml
[cf-image]: https://img.shields.io/codefactor/grade/github/anypackage/msu
[gallery-site]: https://www.powershellgallery.com/packages/AnyPackage.Msu
[build-site]: https://github.com/anypackage/msu/actions/workflows/ci.yml
[cf-site]: https://www.codefactor.io/repository/github/anypackage/msu

`AnyPackage.Msu` is a Windows Msu provider for AnyPackage.

## Install AnyPackage.Msu

```powershell
Install-PSResource AnyPackage.Msu
```

## Import AnyPackage.Msu

```powershell
Import-Module AnyPackage.Msu
```

## Sample usages

### Get list of installed packages

```powershell
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
