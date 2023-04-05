@{
    RootModule = 'MsuProvider.dll'
    ModuleVersion = '0.2.1'
    CompatiblePSEditions = @('Desktop', 'Core')
    GUID = '758cc272-d0a4-49e5-9a24-7d9e730f60e1'
    Author = 'Thomas Nieto'
    Copyright = '(c) 2023 Thomas Nieto. All rights reserved.'
    Description = 'Windows Msu provider for AnyPackage.'
    PowerShellVersion = '5.1'
    RequiredModules = @(
        @{ ModuleName = 'AnyPackage'; ModuleVersion = '0.4.0' })
    FunctionsToExport = @()
    CmdletsToExport = @()
    AliasesToExport = @()
    PrivateData = @{
        AnyPackage = @{
            Providers = 'Msu'
        }
        PSData = @{
            Tags = @('AnyPackage', 'Provider', 'MSU', 'Windows')
            LicenseUri = 'https://github.com/AnyPackage/AnyPackage.Msu/blob/main/LICENSE'
            ProjectUri = 'https://github.com/AnyPackage/AnyPackage.Msu'
        }
    }
    HelpInfoURI = 'https://go.anypackage.dev/help'
}
