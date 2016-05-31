using System;
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
        [Test]
        public void NullList_ShouldThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => new ServiceDependenciesAnalyser(null));
        }

        [Test]
        public void SimpleDependency_GroupedByService_ShouldReturnServicesAsExpected()
        {
            var myServiceDependency = new ServiceDependency
            {
                ServiceId = "MyService",
                DependencyId = "MyDependency",
                DependencyVersion = new SemanticVersion(1, 0, 0, 0)
            };
            var analyser = new ServiceDependenciesAnalyser(new[] {myServiceDependency});
            var service = analyser.GroupByService();

            service.Keys.Count.Should().Be(1);
            service.Keys.Should().Contain(myServiceDependency.ServiceId);
            service[myServiceDependency.ServiceId].Length.Should().Be(1);

            var dependencyStaleness = service[myServiceDependency.ServiceId][0];
            dependencyStaleness.DependencyId.Should().Be(myServiceDependency.DependencyId);
            dependencyStaleness.Version.Should().Be(myServiceDependency.DependencyVersion);
            dependencyStaleness.LatestKnownVersion.Should().Be(myServiceDependency.DependencyVersion);
            dependencyStaleness.StalenessRating.Should().Be(0);
        }
    }
}