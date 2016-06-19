using System.Collections.Generic;
using DependencyMap.Analysis;
using DependencyMap.Scanning;
using FluentAssertions;
using NuGet;
using NUnit.Framework;

namespace DependencyMap.Tests.Analysis
{
    [TestFixture]
    public class GroupedByDependency
    {
        [Test]
        public void SimpleDependency_ShouldReturnDependenciesAsExpected()
        {
            var serviceDependency = new ServiceDependency
            {
                ServiceId = "Service0",
                DependencyId = "Dependency0",
                DependencyVersion = new SemanticVersion(1, 0, 0, 0)
            };
            var analyser = new ServiceDependenciesAnalyser(new[] {serviceDependency});
            var dependencies = analyser.GroupByDependency();

            dependencies.ShouldBeEquivalentTo(
                new Dictionary<string, Dictionary<SemanticVersion, string[]>>
                {
                    {
                        "Dependency0",
                        new Dictionary<SemanticVersion, string[]>
                        {
                            {
                                new SemanticVersion(1, 0, 0, 0),
                                new[] {"Service0"}
                            }
                        }
                    }
                });
        }

        [Test]
        public void DifferentVersions_ShouldReturnServicesGroupedByVersion()
        {
            var serviceDependencies = new[]
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
            var analyser = new ServiceDependenciesAnalyser(serviceDependencies);
            var dependencies = analyser.GroupByDependency();


            dependencies.ShouldBeEquivalentTo(
                new Dictionary<string, Dictionary<SemanticVersion, string[]>>
                {
                    {
                        "Dependency0",
                        new Dictionary<SemanticVersion, string[]>
                        {
                            {
                                new SemanticVersion(1, 0, 0, 0),
                                new[] {"Service0", "Service1"}
                            },
                            {
                                new SemanticVersion(1, 1, 0, 0),
                                new[] {"Service1"}
                            },
                            {
                                new SemanticVersion(2, 0, 0, 0),
                                new[] {"Service2"}
                            }
                        }
                    }
                });
        }
    }
}