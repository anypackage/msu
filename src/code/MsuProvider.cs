// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Management;

namespace AnyPackage.Provider.Msu
{
    [PackageProvider("Msu")]
    public sealed class MsuProvider : PackageProvider, IGetPackage
    {
        private readonly static Guid s_id = new Guid("314633fe-c7e9-4eeb-824b-382a8a4e92b8");

        public MsuProvider() : base(s_id) { }

        public void GetPackage(PackageRequest request)
        {
            var quickFix = new ManagementObjectSearcher(@"root\cimv2", "select * from Win32_QuickFixEngineering");

            foreach (var hotFix in quickFix.Get())
            {
                if (request.IsMatch((string)hotFix["HotFixID"]))
                {
                    request.WritePackage((string)hotFix["HotFixID"],
                                         new PackageVersion("0"),
                                         (string)hotFix["Description"]);
                }
            }
        }
    }
}
