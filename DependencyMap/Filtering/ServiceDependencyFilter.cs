using System;
using System.Collections.Generic;
using System.Linq;
using DependencyMap.Scanning;

namespace DependencyMap.Filtering
{
    internal class ServiceDependencyFilter
    {
        private readonly IEnumerable<string> _dependencyIdsToInclude;
        private readonly Func<string, bool> _filePathFilter;

        public ServiceDependencyFilter(IServiceDependencyFilterConfig config)
        {
            _dependencyIdsToInclude = config?.DependencyIdsToInclude;
            _filePathFilter = config?.FilePathFilter;
        }

        public IEnumerable<ServiceDependency> Apply(IEnumerable<ServiceDependency> serviceDependencies)
        {
            if (this._dependencyIdsToInclude != null)
            {
                serviceDependencies = serviceDependencies
                    .Where(x => this._dependencyIdsToInclude.Contains(x.DependencyId));
            }

            if (this._filePathFilter != null)
            {
                serviceDependencies = serviceDependencies
                    .Where(x => this._filePathFilter(x.DependencyFilePath));
            }

            return serviceDependencies;
        }
    }
}
