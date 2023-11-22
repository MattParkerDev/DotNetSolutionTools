using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace DotNetSolutionTools.Core.Infrastructure;

public static class NugetLookup
{
    public static async Task<NuGetVersion> FetchPackageMetadataAsync(string packageId)
    {
        var cache = new SourceCacheContext();
        var repositories = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
        var logger = NullLogger.Instance;

        var resource = await repositories.GetResourceAsync<FindPackageByIdResource>();
        var versions = await resource.GetAllVersionsAsync(
            packageId,
            cache,
            logger,
            CancellationToken.None
        );

        return versions.Last(s => s.IsPrerelease == false);
    }
}
