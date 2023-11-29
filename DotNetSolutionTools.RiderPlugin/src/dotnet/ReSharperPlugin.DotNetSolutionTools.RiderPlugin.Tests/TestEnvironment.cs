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
    public class DotNetSolutionTools.RiderPluginTestEnvironmentZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>, IRequire<IDotNetSolutionTools.RiderPluginZone> { }

    [ZoneMarker]
    public class ZoneMarker : IRequire<ICodeEditingZone>, IRequire<ILanguageCSharpZone>, IRequire<DotNetSolutionTools.RiderPluginTestEnvironmentZone> { }

    [SetUpFixture]
    public class DotNetSolutionTools.RiderPluginTestsAssembly : ExtensionTestEnvironmentAssembly<DotNetSolutionTools.RiderPluginTestEnvironmentZone> { }
}
