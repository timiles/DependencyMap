using DependencyMap.Models;
using DependencyMap.Scanning;
using FluentAssertions;
using NUnit.Framework;

namespace DependencyMap.Tests.Scanning
{
    [TestFixture]
    public class NugetPackageConfigScannerTests
    {
        [Test]
        public void EmptyFile_WhenScanned_ShouldYieldNothing()
        {
            var dependencyFile = new DependencyFile { ServiceId = @"MyService", FileContents = @"" };
            var sourceRepository = new FakeSourceRepository(dependencyFile);
            var scanner = new NugetPackageConfigScanner(sourceRepository);

            var serviceDependencies = scanner.GetAllServiceDependencies();
            serviceDependencies.Should().BeEmpty();
        }
    }
}
