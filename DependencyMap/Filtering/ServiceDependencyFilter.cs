using System.Collections.Generic;
using System.Linq;
using DependencyMap.Models;

namespace DependencyMap.Filtering
{
    internal class ServiceDependencyFilter
    {
        private readonly IEnumerable<string> _dependencyIdsToInclude;

        public ServiceDependencyFilter(IServiceDependencyFilterConfig config)
        {
            _dependencyIdsToInclude = config?.DependencyIdsToInclude;
        }

        public IEnumerable<ServiceDependency> Apply(IEnumerable<ServiceDependency> serviceDependencies)
        {
            if (this._dependencyIdsToInclude != null)
            {
                serviceDependencies = serviceDependencies
                    .Where(x => this._dependencyIdsToInclude.Contains(x.DependencyId));
            }

            return serviceDependencies;
        }
    }
}
