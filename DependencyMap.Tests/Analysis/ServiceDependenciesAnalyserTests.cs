using System;
using System.Linq;
using DependencyMap.Analysis;
using DependencyMap.Models;
using FluentAssertions;
using NuGet;
using NUnit.Framework;

namespace DependencyMap.Tests.Analysis
{
    [TestFixture]
    public class ServiceDependenciesAnalyserTests
    {
        private readonly ServiceDependency[] _serviceDependencies;

        public ServiceDependenciesAnalyserTests()
        {
            _serviceDependencies = new[]
            {
                new ServiceDependency
                {
                    ServiceId = "Service0",
                    DependencyId = "Dependency0",
                    DependencyVersion = new SemanticVersion(1, 0, 0, 0)
                },
                new ServiceDependency
                {
                    ServiceId = "Service1",
                    DependencyId = "Dependency0",
                    DependencyVersion = new SemanticVersion(1, 1, 0, 0)
                },
                new ServiceDependency
                {
                    ServiceId = "Service2",
                    DependencyId = "Dependency0",
                    DependencyVersion = new SemanticVersion(2, 0, 0, 0)
                }
            };
        }

        [Test]
        public void NullList_ShouldThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => new ServiceDependenciesAnalyser(null));
        }

        [TestFixture]
        public class GroupedByService : ServiceDependenciesAnalyserTests
        {
            [Test]
            public void SimpleDependency_ShouldReturnServicesAsExpected()
            {
                var serviceDependency = _serviceDependencies[0];
                var analyser = new ServiceDependenciesAnalyser(new[] {serviceDependency});
                var services = analyser.GroupByService();

                services.Keys.Count.Should().Be(1);
                services.Keys.Should().Contain(serviceDependency.ServiceId);
                services[serviceDependency.ServiceId].Length.Should().Be(1);

                var dependencyStaleness = services[serviceDependency.ServiceId][0];
                dependencyStaleness.DependencyId.Should().Be(serviceDependency.DependencyId);
                dependencyStaleness.Version.Should().Be(serviceDependency.DependencyVersion);
                dependencyStaleness.LatestKnownVersion.Should().Be(serviceDependency.DependencyVersion);
                dependencyStaleness.StalenessRating.Should().Be(0);
            }

            [Test]
            public void DifferentVersions_ShouldReturnDifferentStalenesses()
            {
                var analyser = new ServiceDependenciesAnalyser(_serviceDependencies);
                var services = analyser.GroupByService();

                services.Keys.Count.Should().Be(3);

                services["Service0"].First(x => x.DependencyId == "Dependency0").StalenessRating.Should().Be(2);
                services["Service1"].First(x => x.DependencyId == "Dependency0").StalenessRating.Should().Be(1);
                services["Service2"].First(x => x.DependencyId == "Dependency0").StalenessRating.Should().Be(0);
            }
        }
    }
}