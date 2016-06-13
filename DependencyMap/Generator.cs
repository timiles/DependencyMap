using System.Collections.Generic;
using System.Linq;
using DependencyMap.Analysis;
using DependencyMap.Filtering;
using DependencyMap.Scanning;
using DependencyMap.SourceRepositories;

namespace DependencyMap
{
    public class Generator
    {
        private readonly ISourceRepository _sourceRepository;
        private readonly IDependencyFileScanner _dependencyFileScanner;
        private readonly ServiceDependencyFilter _serviceDependencyFilter;

        public Generator(ISourceRepository sourceRepository,
            IDependencyFileScanner dependencyFileScanner = null,
            IServiceDependencyFilterConfig serviceDependencyFilterConfig = null)
        {
            _sourceRepository = sourceRepository;
            _dependencyFileScanner = dependencyFileScanner ?? new NuGetPackageConfigScanner();
            _serviceDependencyFilter = new ServiceDependencyFilter(serviceDependencyFilterConfig);
        }

        public IEnumerable<Dependency> GetAllDependencies()
        {
            var dependencyFiles = _sourceRepository.GetDependencyFilesToScan();
            var serviceDependencies = _dependencyFileScanner.GetAllServiceDependencies(dependencyFiles);
            var filtered = _serviceDependencyFilter.Apply(serviceDependencies);
            var analyser = new ServiceDependenciesAnalyser(filtered.ToList());
            return analyser.GetAllDependencies();
        }

        public IEnumerable<Service> GetAllServices()
        {
            var dependencyFiles = _sourceRepository.GetDependencyFilesToScan();
            var serviceDependencies = _dependencyFileScanner.GetAllServiceDependencies(dependencyFiles);
            var filtered = _serviceDependencyFilter.Apply(serviceDependencies);
            var analyser = new ServiceDependenciesAnalyser(filtered.ToList());
            return analyser.GetAllServices();
        }
    }
}
