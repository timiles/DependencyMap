using System.Collections.Generic;
using System.Linq;
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
            var input = new ServiceDependency
            {
                ServiceId = "Service0",
                DependencyId = "Dependency0",
                DependencyVersion = new SemanticVersion(1, 0, 0, 0)
            };
            var analyser = new ServiceDependenciesAnalyser(new[] {input});
            var output = analyser.GroupByDependency();

            output.ShouldBeEquivalentTo(
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
            var input = new[]
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
            var analyser = new ServiceDependenciesAnalyser(input);
            var output = analyser.GroupByDependency();


            output.ShouldBeEquivalentTo(
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

        [Test]
        public void InputDependenciesNotOrdered_ShouldReturnOutputOrderedByDependencyId()
        {
            var input = new[]
            {
                new ServiceDependency
                {
                    ServiceId = "Service0",
                    DependencyId = "Charlie",
                    DependencyVersion = new SemanticVersion(1, 0, 0, 0)
                },
                new ServiceDependency
                {
                    ServiceId = "Service1",
                    DependencyId = "Bravo",
                    DependencyVersion = new SemanticVersion(1, 0, 0, 0)
                },
                new ServiceDependency
                {
                    ServiceId = "Service1",
                    DependencyId = "Alpha",
                    DependencyVersion = new SemanticVersion(1, 0, 0, 0)
                }
            };
            var analyser = new ServiceDependenciesAnalyser(input);
            var output = analyser.GroupByDependency();

            // Should.Equal also asserts order
            output.Select(x => x.Key).Should().Equal(
                input.Select(x => x.DependencyId).OrderBy(x => x));
        }

        [Test]
        public void InputServicesNotOrdered_ShouldReturnOutputOrderedByServiceId()
        {
            var version = new SemanticVersion(1, 0, 0, 0);
            var input = new[]
            {
                new ServiceDependency
                {
                    ServiceId = "Charlie",
                    DependencyId = "Dependency0",
                    DependencyVersion = version
                },
                new ServiceDependency
                {
                    ServiceId = "Bravo",
                    DependencyId = "Dependency0",
                    DependencyVersion = version
                },
                new ServiceDependency
                {
                    ServiceId = "Alpha",
                    DependencyId = "Dependency0",
                    DependencyVersion = version
                }
            };
            var analyser = new ServiceDependenciesAnalyser(input);
            var output = analyser.GroupByDependency();

            // Should.Equal also asserts order
            output["Dependency0"][version].Should().Equal(
                input.Select(x => x.ServiceId).OrderBy(x => x));
        }
    }
}