// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using Microsoft.Deployment.Compression.Cab;

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace AnyPackage.Provider.Msu;

[PackageProvider("Msu", PackageByName = false, FileExtensions = [".msu"])]
public sealed class MsuProvider : PackageProvider, IFindPackage, IGetPackage, IInstallPackage, IUninstallPackage
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
        Dictionary<string, object?> metadata = [];
        using var reader = file.OpenText();

        while ((line = reader.ReadLine()) is not null)
        {
            var values = line.Split(['='], 2);
            var key = values[0].Replace(" ", "");

            // Remove quotes around value
            var value = values[1].Substring(1, values[1].Length - 2);

            metadata.Add(key, value);
        }

        if (metadata.ContainsKey("KBArticleNumber"))
        {
            var kb = string.Format("KB{0}", metadata["KBArticleNumber"]);
            var source = new PackageSourceInfo(request.Path, request.Path, ProviderInfo);
            var package = new PackageInfo(kb, null, source, (string)metadata["PackageType"]!, null, metadata, ProviderInfo);
            request.WritePackage(package);
        }
    }

    public void GetPackage(PackageRequest request)
    {
        var quickFix = new ManagementObjectSearcher(@"root\cimv2", "select * from Win32_QuickFixEngineering");

        foreach (var hotFix in quickFix.Get())
        {
            if (request.IsMatch((string)hotFix["HotFixID"]) && (request.Version is null || request.Version.ToString() == "*"))
            {
                var package = new PackageInfo((string)hotFix["HotFixID"], null, (string)hotFix["Description"], ProviderInfo);
                request.WritePackage(package);
            }
        }
    }

    public void InstallPackage(PackageRequest request)
    {
        using var powershell = PowerShell.Create(RunspaceMode.CurrentRunspace);
        powershell.AddCommand("Find-Package").AddParameter("Path", request.Path);
        var package = powershell.Invoke<PackageInfo>().FirstOrDefault();

        if (package is null)
        {
            return;
        }

        using var process = new Process();
        process.StartInfo.FileName = "wusa.exe";
        process.StartInfo.Arguments = $"{request.Path} /quiet /norestart";
        process.StartInfo.UseShellExecute = true;
        process.Start();
        process.WaitForExit();

        if (!ValidateExitCode(process.ExitCode, request))
        {
            return;
        }

        request.WritePackage(package);
    }

    public void UninstallPackage(PackageRequest request)
    {
        var regex = new Regex(@"KB\d+", RegexOptions.IgnoreCase);

        if (!regex.Match(request.Name).Success)
        {
            return;
        }

        if (request.IsVersionFiltered)
        {
            return;
        }

        using var process = new Process();
        process.StartInfo.FileName = "wusa.exe";
        var kb = request.Name.Replace("KB", "");
        process.StartInfo.Arguments = $"/uninstall /kb:{kb} /norestart";
        process.StartInfo.UseShellExecute = true;
        process.Start();
        process.WaitForExit();

        if (!ValidateExitCode(process.ExitCode, request))
        {
            return;
        }

        var package = request.Package is not null ? request.Package : new PackageInfo(request.Name, ProviderInfo);
        request.WritePackage(package);
    }

    private bool ValidateExitCode(int exitCode, PackageRequest request)
    {
        // https://learn.microsoft.com/en-us/windows/win32/wua_sdk/wua-success-and-error-codes-
        // http://inetexplorer.mvps.org/archive/windows_update_codes.htm
        switch (exitCode)
        {
            case 0:
                request.WriteVerbose("The operation completed successfully.");
                return true;

            case 1223:
                request.WriteVerbose("The operation was canceled by the user.");
                return false;

            case 0x00240001:
                request.WriteVerbose("WUA was stopped successfully.");
                return true;

            case 0x00240002:
                request.WriteVerbose("WUA updated itself.");
                return true;

            case 0x00240003:
                request.WriteWarning("The operation completed successfully but errors occurred applying the updates.");
                return true;

            case 0x00240005:
            case 3010:
                request.WriteWarning("The system must be restarted to complete installation of the update.");
                return true;

            case 0x00240006:
                request.WriteVerbose("The update to be installed is already installed on the system.");
                return false;

            case 0x00240007:
                request.WriteVerbose("The update to be removed is not installed on the system.");
                return false;

            case 0x00240008:
                request.WriteVerbose("The update to be downloaded has already been downloaded.");
                return true;

            case 0x00242015:
                request.WriteVerbose("The installation operation for the update is still in progress.");
                return true;

            default:
                request.WriteVerbose($"The operation failed with exit code: {exitCode}");
                return false;
        }
    }
}
