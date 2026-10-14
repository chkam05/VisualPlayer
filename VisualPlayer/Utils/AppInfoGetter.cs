using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VisualPlayer.Models.App;

namespace VisualPlayer.Utils;

public static class AppInfoGetter
{
    #region CONST

    private const string APP_AUTHOR = "Kamil Karpiński";
    private const string APP_NAME = "VisualPlayer";
    private const string APP_VERSION = "7.1.1.0";
    private const string NUGET_PACKAGE_PREFIX = "NuGetPackage:";

    #endregion CONST

    private static readonly Assembly Assembly = typeof(AppInfoGetter).Assembly;


    public static string AppName
    {
        get => GetAttribute<AssemblyProductAttribute>()?.Product
            ?? Assembly.GetName().Name
            ?? APP_NAME;
    }

    public static string AppVersion
    {
        get => GetAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? Assembly.GetName().Version?.ToString()
            ?? APP_VERSION;
    }

    public static string AppAuthor
    {
        get => GetAttribute<AssemblyCompanyAttribute>()?.Company
            ?? APP_AUTHOR;
    }

    public static IReadOnlyList<NuGetPackageInfo> NuGetPackages { get; } = Assembly
        .GetCustomAttributes<AssemblyMetadataAttribute>()
        .Where(attribute => attribute.Key.StartsWith(NUGET_PACKAGE_PREFIX, StringComparison.Ordinal))
        .Select(attribute => new NuGetPackageInfo(
            attribute.Key[NUGET_PACKAGE_PREFIX.Length..],
            attribute.Value ?? string.Empty))
        .OrderBy(package => package.Name, StringComparer.OrdinalIgnoreCase)
        .ToArray();

    private static T? GetAttribute<T>() where T : Attribute
    {
        return Assembly.GetCustomAttribute<T>();
    }
}
