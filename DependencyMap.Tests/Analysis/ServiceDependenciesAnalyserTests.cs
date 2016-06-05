using System;
using System.Collections.Generic;
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

                services.ShouldBeEquivalentTo(
                    new Dictionary<string, DependencyStaleness[]>
                    {
                        {
                            "Service0", new[]
                            {
                                new DependencyStaleness
                                {
                                    DependencyId = "Dependency0",
                                    LatestKnownVersion = new SemanticVersion(1, 0, 0, 0),
                                    Version = new SemanticVersion(1, 0, 0, 0),
                                    StalenessRating = 0
                                }
                            }
                        }
                    });
            }

            [Test]
            public void DifferentVersions_ShouldReturnDifferentStalenesses()
            {
                var analyser = new ServiceDependenciesAnalyser(_serviceDependencies);
                var services = analyser.GroupByService();

                services.ShouldBeEquivalentTo(
                    new Dictionary<string, DependencyStaleness[]>
                    {
                        {
                            "Service0", new[]
                            {
                                new DependencyStaleness
                                {
                                    DependencyId = "Dependency0",
                                    LatestKnownVersion = new SemanticVersion(2, 0, 0, 0),
                                    Version = new SemanticVersion(1, 0, 0, 0),
                                    StalenessRating = 2
                                }
                            }
                        },
                        {
                            "Service1",
                            new[]
                            {
                                new DependencyStaleness
                                {
                                    DependencyId = "Dependency0",
                                    LatestKnownVersion = new SemanticVersion(2, 0, 0, 0),
                                    Version = new SemanticVersion(1, 0, 0, 0),
                                    StalenessRating = 2
                                },
                                new DependencyStaleness
                                {
                                    DependencyId = "Dependency0",
                                    LatestKnownVersion = new SemanticVersion(2, 0, 0, 0),
                                    Version = new SemanticVersion(1, 1, 0, 0),
                                    StalenessRating = 1
                                }
                            }
                        },
                        {
                            "Service2", new[]
                            {
                                new DependencyStaleness
                                {
                                    DependencyId = "Dependency0",
                                    LatestKnownVersion = new SemanticVersion(2, 0, 0, 0),
                                    Version = new SemanticVersion(2, 0, 0, 0),
                                    StalenessRating = 0
                                }
                            }
                        }
                    });
            }
        }


        [TestFixture]
        public class GroupedByDependency : ServiceDependenciesAnalyserTests
        {
            [Test]
            public void SimpleDependency_ShouldReturnDependenciesAsExpected()
            {
                var serviceDependency = _serviceDependencies[0];
                var analyser = new ServiceDependenciesAnalyser(new[] { serviceDependency });
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
                var analyser = new ServiceDependenciesAnalyser(_serviceDependencies);
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
}