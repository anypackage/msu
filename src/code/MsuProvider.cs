// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using Microsoft.Deployment.Compression.Cab;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;

namespace AnyPackage.Provider.Msu
{
    [PackageProvider("Msu", PackageByName = false, FileExtensions = new string[] { ".msu" })]
    public sealed class MsuProvider : PackageProvider, IFindPackage, IGetPackage
    {
        public void FindPackage(PackageRequest request)
        {
            var file = new CabInfo(request.Path).GetFiles()
                                                .Where(x => Path.GetExtension(x.Name) == ".txt")
                                                .FirstOrDefault();

            if (file is null)
            {
                return;
            }

            string line;
            Dictionary<string, object> metadata = new Dictionary<string, object>();
            using var reader = file.OpenText();

            while ((line = reader.ReadLine()) is not null)
            {
                var values = line.Split(new char[] { '=' }, 2);
                var key = values[0].Replace(" ", "");

                // Remove quotes around value
                var value = values[1].Substring(1, values[1].Length - 2);

                metadata.Add(key, value);
            }

            if (metadata.ContainsKey("KBArticleNumber"))
            {
                var kb = string.Format("KB{0}", metadata["KBArticleNumber"]);
                var source = new PackageSourceInfo(request.Path, request.Path, ProviderInfo);
                var package = new PackageInfo(kb, null, source, (string)metadata["PackageType"], null, metadata, ProviderInfo);
                request.WritePackage(package);
            }
        }

        public void GetPackage(PackageRequest request)
        {
            var quickFix = new ManagementObjectSearcher(@"root\cimv2", "select * from Win32_QuickFixEngineering");

            foreach (var hotFix in quickFix.Get())
            {
                if (request.IsMatch((string)hotFix["HotFixID"]))
                {
                    var package = new PackageInfo((string)hotFix["HotFixID"], null, (string)hotFix["Description"], ProviderInfo);
                    request.WritePackage(package);
                }
            }
        }
    }
}
