// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Management.Automation;
using static AnyPackage.Provider.PackageProviderManager;

namespace AnyPackage.Provider.Msu
{
    public sealed class Init : IModuleAssemblyInitializer, IModuleAssemblyCleanup
    {
        public void OnImport()
        {
            RegisterProvider(typeof(MsuProvider), "AnyPackage.Msu");
        }

        public void OnRemove(PSModuleInfo psModuleInfo)
        {
            UnregisterProvider(typeof(MsuProvider));
        }
    }
}
