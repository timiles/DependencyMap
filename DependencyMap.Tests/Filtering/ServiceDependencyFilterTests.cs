using System;
using System.Collections.Generic;
using System.Linq;
using DependencyMap.Filtering;
using DependencyMap.Scanning;
using FluentAssertions;
using NUnit.Framework;

namespace DependencyMap.Tests.Filtering
{
    [TestFixture]
    public class ServiceDependencyFilterTests
    {
        class ServiceDependencyConfig : IServiceDependencyFilterConfig
        {
            public ServiceDependencyConfig(
                IEnumerable<string> dependencyIdsToInclude = null,
                Func<string, bool> filePathFilter = null)
            {
                DependencyIdsToInclude = dependencyIdsToInclude;
                FilePathFilter = filePathFilter;
            }

            public IEnumerable<string> DependencyIdsToInclude { get; }

            public Func<string, bool> FilePathFilter { get; }
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
                    DependencyFilePath = @"Project0\packages.config",
                },
                new ServiceDependency
                {
                    ServiceId = "Service0",
                    DependencyId = "Dependency0",
                    DependencyFilePath = @"Project0.Tests\packages.config",
                },
                new ServiceDependency
                {
                    ServiceId = "Service0",
                    DependencyId = "Dependency1",
                    DependencyFilePath = @"Project0\packages.config",
                },
                new ServiceDependency
                {
                    ServiceId = "Service0",
                    DependencyId = "Dependency2",
                    DependencyFilePath = @"Project0\packages.config",
                },
                new ServiceDependency
                {
                    ServiceId = "Service1",
                    DependencyId = "Dependency0",
                    DependencyFilePath = @"Project1\packages.config",
                },
                new ServiceDependency
                {
                    ServiceId = "Service1",
                    DependencyId = "Dependency0",
                    DependencyFilePath = @"Project1.Tests\packages.config",
                },
                new ServiceDependency
                {
                    ServiceId = "Service2",
                    DependencyId = "Dependency0",
                    DependencyFilePath = @"Project2\packages.config",
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
        public void ConfigPropertiesAreNull_ShouldReturnAllServiceDependencies()
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
            filtered.ShouldBeEquivalentTo(
                this._serviceDependencies.Where(x => dependencyIdsToInclude.Contains(x.DependencyId)));
        }

        [Test]
        public void FilePathFilterIsSet_ShouldReturnFilteredServiceDependencies()
        {
            Func<string, bool> pathFilter = p => !p.ToLower().Contains(@".tests\");
            var filter = new ServiceDependencyFilter(new ServiceDependencyConfig(filePathFilter: pathFilter));

            var filtered = filter.Apply(this._serviceDependencies);
            filtered.ShouldBeEquivalentTo(
                this._serviceDependencies.Where(x => pathFilter(x.DependencyFilePath)));
        }
    }
}
