using System.Diagnostics.CodeAnalysis;
using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace DotNetSolutionTools.Core.Infrastructure;

public static class NugetLookup
{
    public static async Task<NuGetVersion> FetchPackageMetadataAsync(string packageId, bool isCurrentlyPrerelease)
    {
        var cache = new SourceCacheContext();
        var repositories = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
        var logger = NullLogger.Instance;

        var resource = await repositories.GetResourceAsync<FindPackageByIdResource>();
        var versions = await resource.GetAllVersionsAsync(packageId, cache, logger, CancellationToken.None);

        var latestPrereleaseVersion = versions.LastOrDefault(s => s.IsPrerelease == true);
        var latestStableVersion = versions.LastOrDefault(s => s.IsPrerelease == false);

        if (latestStableVersion is null && latestPrereleaseVersion is null)
        {
            Throw(packageId);
        }

        if (latestStableVersion is null)
        {
            if (isCurrentlyPrerelease && latestPrereleaseVersion is not null)
            {
                return latestPrereleaseVersion;
            }
            else
            {
                Throw(packageId);
            }
        }

        if (latestPrereleaseVersion is null)
        {
            return latestStableVersion;
        }

        if (latestStableVersion > latestPrereleaseVersion)
        {
            return latestStableVersion;
        }

        if (isCurrentlyPrerelease)
        {
            return latestPrereleaseVersion;
        }

        return latestStableVersion;
    }

    [DoesNotReturn]
    private static void Throw(string packageId)
    {
        throw new ArgumentNullException(
            "latestVersion",
            $"No latest stable or prerelease Nuget package version found for {packageId}"
        );
    }
}
