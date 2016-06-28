using DependencyMap.Analysis;
using DependencyMap.Scanning;
using FluentAssertions;
using NuGet;
using NUnit.Framework;

namespace DependencyMap.Tests.Analysis
{
    [TestFixture]
    public class StalenessCalculations
    {
        [Test]
        public void SimpleDependency_ShouldReturnStalenessAsExpected()
        {
            var input = new ServiceDependency
            {
                ServiceId = "Service0",
                DependencyId = "Dependency0",
                DependencyVersion = new SemanticVersion(1, 0, 0, 0)
            };
            var analyser = new ServiceDependenciesAnalyser(new[] { input });
            var output = analyser.CalculateDependencyStalenessesAcrossEntireSource();

            output.ShouldBeEquivalentTo(
                new[]
                {
                    new DependencyStaleness
                    {
                        DependencyId = "Dependency0",
                        Version = new SemanticVersion(1, 0, 0, 0),
                        StalenessRating = 0
                    }
                });
        }

        [Test]
        public void SameVersionInDifferentServices_ShouldReturnSingleStaleness()
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
                }
            };
            var analyser = new ServiceDependenciesAnalyser(input);
            var output = analyser.CalculateDependencyStalenessesAcrossEntireSource();

            output.ShouldBeEquivalentTo(
                new[]
                {
                    new DependencyStaleness
                    {
                        DependencyId = "Dependency0",
                        Version = new SemanticVersion(1, 0, 0, 0),
                        StalenessRating = 0
                    }
                });
        }

        [Test]
        public void DifferentVersionsInDifferentServices_ShouldReturnStalenessesAsExpected()
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
                    DependencyVersion = new SemanticVersion(2, 0, 0, 0)
                }
            };
            var analyser = new ServiceDependenciesAnalyser(input);
            var output = analyser.CalculateDependencyStalenessesAcrossEntireSource();

            output.ShouldBeEquivalentTo(
                new[]
                {
                    new DependencyStaleness
                    {
                        DependencyId = "Dependency0",
                        Version = new SemanticVersion(2, 0, 0, 0),
                        StalenessRating = 0
                    },
                    new DependencyStaleness
                    {
                        DependencyId = "Dependency0",
                        Version = new SemanticVersion(1, 0, 0, 0),
                        StalenessRating = 1
                    }
                });
        }

        [Test]
        public void LatestVersionIsPreRelease_DoesNotContributeToStaleness()
        {
            var input = new[]
            {
                new ServiceDependency
                {
                    ServiceId = "Service0",
                    DependencyId = "Dependency0",
                    DependencyVersion = new SemanticVersion(2, 0, 0, "alpha")
                },
                new ServiceDependency
                {
                    ServiceId = "Service0",
                    DependencyId = "Dependency0",
                    DependencyVersion = new SemanticVersion(1, 0, 0, 0)
                }
            };
            var analyser = new ServiceDependenciesAnalyser(input);
            var output = analyser.CalculateDependencyStalenessesAcrossEntireSource();

            output.ShouldBeEquivalentTo(
                new[]
                {
                    new DependencyStaleness
                    {
                        DependencyId = "Dependency0",
                        Version = new SemanticVersion(2, 0, 0, "alpha"),
                        StalenessRating = 0
                    },
                    new DependencyStaleness
                    {
                        DependencyId = "Dependency0",
                        Version = new SemanticVersion(1, 0, 0, 0),
                        StalenessRating = 0
                    }
                });
        }

        [Test]
        public void OlderVersionIsPreRelease_ContributesToStaleness()
        {
            var input = new[]
            {
                new ServiceDependency
                {
                    ServiceId = "Service0",
                    DependencyId = "Dependency0",
                    DependencyVersion = new SemanticVersion(2, 0, 0, 0)
                },
                new ServiceDependency
                {
                    ServiceId = "Service0",
                    DependencyId = "Dependency0",
                    DependencyVersion = new SemanticVersion(1, 1, 0, "alpha")
                },
                new ServiceDependency
                {
                    ServiceId = "Service0",
                    DependencyId = "Dependency0",
                    DependencyVersion = new SemanticVersion(1, 0, 0, 0)
                }
            };
            var analyser = new ServiceDependenciesAnalyser(input);
            var output = analyser.CalculateDependencyStalenessesAcrossEntireSource();

            output.ShouldBeEquivalentTo(
                new[]
                {
                    new DependencyStaleness
                    {
                        DependencyId = "Dependency0",
                        Version = new SemanticVersion(2, 0, 0, 0),
                        StalenessRating = 0
                    },
                    new DependencyStaleness
                    {
                        DependencyId = "Dependency0",
                        Version = new SemanticVersion(1, 1, 0, "alpha"),
                        StalenessRating = 1
                    },
                    new DependencyStaleness
                    {
                        DependencyId = "Dependency0",
                        Version = new SemanticVersion(1, 0, 0, 0),
                        StalenessRating = 1
                    }
                });
        }
    }
}