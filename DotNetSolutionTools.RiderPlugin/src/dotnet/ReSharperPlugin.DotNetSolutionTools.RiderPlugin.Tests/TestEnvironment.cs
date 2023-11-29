using System.Threading;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NUnit.Framework;

[assembly: Apartment(ApartmentState.STA)]

namespace ReSharperPlugin.DotNetSolutionTools.RiderPlugin.Tests
{
    [ZoneDefinition]
    public class DotNetSolutionToolsRiderPluginTestEnvironmentZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>, IRequire<IDotNetSolutionToolsRiderPluginZone> { }

    [ZoneMarker]
    public class ZoneMarker : IRequire<ICodeEditingZone>, IRequire<ILanguageCSharpZone>, IRequire<DotNetSolutionToolsRiderPluginTestEnvironmentZone> { }

    [SetUpFixture]
    public class DotNetSolutionToolsRiderPluginTestsAssembly : ExtensionTestEnvironmentAssembly<DotNetSolutionToolsRiderPluginTestEnvironmentZone> { }
}
