// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using Microsoft.Deployment.Compression.Cab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;

namespace AnyPackage.Provider.Msu
{
    [PackageProvider("Msu")]
    public sealed class MsuProvider : PackageProvider, IFindPackage, IGetPackage
    {
        private readonly static Guid s_id = new Guid("314633fe-c7e9-4eeb-824b-382a8a4e92b8");

        public MsuProvider() : base(s_id) { }

        public void FindPackage(PackageRequest request)
        {
            var file = new CabInfo(request.Name).GetFiles()
                                                .Where(x => Path.GetExtension(x.Name) == ".txt")
                                                .FirstOrDefault();

            if (file is null)
            {
                return;
            }

            string line;
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            using var reader = file.OpenText();

            while ((line = reader.ReadLine()) is not null)
            {
                var values = line.Split('=');
                var key = values[0].Replace(" ", "");

                // Remove quotes around value
                var value = values[1].Substring(1, values[1].Length - 2);

                metadata.Add(key, value);
            }

            if (metadata.ContainsKey("KBArticleNumber"))
            {
                var kb = string.Format("KB{0}", metadata["KBArticleNumber"]);
                request.WritePackage(kb,
                                     new PackageVersion("0"),
                                     metadata["PackageType"],
                                     request.NewSourceInfo(request.Name, request.Name));
            }
        }

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
