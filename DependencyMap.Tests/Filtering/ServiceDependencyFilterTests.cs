using System.Collections.Generic;
using System.Linq;
using DependencyMap.Filtering;
using DependencyMap.Models;
using FluentAssertions;
using NUnit.Framework;

namespace DependencyMap.Tests.Filtering
{
    [TestFixture]
    public class ServiceDependencyFilterTests
    {
        class ServiceDependencyConfig : IServiceDependencyFilterConfig
        {
            public ServiceDependencyConfig(IEnumerable<string> dependencyIdsToInclude = null)
            {
                DependencyIdsToInclude = dependencyIdsToInclude;
            }

            public IEnumerable<string> DependencyIdsToInclude { get; }
        }

        private readonly ServiceDependency[] _serviceDependencies;


        public ServiceDependencyFilterTests()
        {
            _serviceDependencies = new[]
            {
                new ServiceDependency
                {
                    ServiceId = "Service0",
                    DependencyId = "Dependency0",
                },
                new ServiceDependency
                {
                    ServiceId = "Service0",
                    DependencyId = "Dependency1",
                },
                new ServiceDependency
                {
                    ServiceId = "Service0",
                    DependencyId = "Dependency2",
                },
                new ServiceDependency
                {
                    ServiceId = "Service1",
                    DependencyId = "Dependency0",
                },
                new ServiceDependency
                {
                    ServiceId = "Service1",
                    DependencyId = "Dependency0",
                },
                new ServiceDependency
                {
                    ServiceId = "Service2",
                    DependencyId = "Dependency0",
                }
            };
        }

        [Test]
        public void ConfigIsNull_ShouldReturnAllServiceDependencies()
        {
            var filter = new ServiceDependencyFilter(null);

            var filtered = filter.Apply(this._serviceDependencies);
            filtered.ShouldBeEquivalentTo(this._serviceDependencies);
        }

        [Test]
        public void DependencyIdsFilterIsNull_ShouldReturnAllServiceDependencies()
        {
            var filter = new ServiceDependencyFilter(new ServiceDependencyConfig());

            var filtered = filter.Apply(this._serviceDependencies);
            filtered.ShouldBeEquivalentTo(this._serviceDependencies);
        }

        [TestCase("Dependency0")]
        [TestCase("Dependency1")]
        [TestCase("Dependency0", "Dependency1", "Dependency2")]
        [TestCase("UnknownDependencyId")]
        [TestCase("Dependency0", "UnknownDependencyId")]
        public void DependencyIdsFilterIsSet_ShouldReturnFilteredServiceDependencies(params string[] dependencyIdsToInclude)
        {
            var filter = new ServiceDependencyFilter(new ServiceDependencyConfig(dependencyIdsToInclude));

            var filtered = filter.Apply(this._serviceDependencies);
            filtered.ShouldBeEquivalentTo(this._serviceDependencies.Where(x => dependencyIdsToInclude.Contains(x.DependencyId)));
        }
    }
}
