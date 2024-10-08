// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Management.Automation;

using static AnyPackage.Provider.PackageProviderManager;

namespace AnyPackage.Provider.Msu;

public sealed class Init : IModuleAssemblyInitializer, IModuleAssemblyCleanup
{
    private readonly Guid _id = new Guid("314633fe-c7e9-4eeb-824b-382a8a4e92b8");

    public void OnImport()
    {
        RegisterProvider(_id, typeof(MsuProvider), "AnyPackage.Msu");
    }

    public void OnRemove(PSModuleInfo psModuleInfo)
    {
        UnregisterProvider(_id);
    }
}
