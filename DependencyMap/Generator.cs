using System;
using System.Collections.Generic;
using System.Linq;
using DependencyMap.Analysis;
using DependencyMap.Scanning;
using DependencyMap.SourceRepositories;

namespace DependencyMap
{
    [Obsolete("Use individual components directly rather than through Generator")]
    public class Generator
    {
        private readonly ISourceRepository _sourceRepository;
        private readonly IDependencyFileScanner _dependencyFileScanner;

        public Generator(ISourceRepository sourceRepository,
            IDependencyFileScanner dependencyFileScanner = null)
        {
            _sourceRepository = sourceRepository;
            _dependencyFileScanner = dependencyFileScanner ?? new NuGetPackageConfigScanner();
        }

        public IEnumerable<Dependency> GetAllDependencies()
        {
            var dependencyFiles = _sourceRepository.GetDependencyFilesToScan();
            var serviceDependencies = _dependencyFileScanner.GetAllServiceDependencies(dependencyFiles);
            var analyser = new ServiceDependenciesAnalyser(serviceDependencies.ToList());
            return analyser.GetAllDependencies();
        }

        public IEnumerable<Service> GetAllServices()
        {
            var dependencyFiles = _sourceRepository.GetDependencyFilesToScan();
            var serviceDependencies = _dependencyFileScanner.GetAllServiceDependencies(dependencyFiles);
            var analyser = new ServiceDependenciesAnalyser(serviceDependencies.ToList());
            return analyser.GetAllServices();
        }
    }
}
