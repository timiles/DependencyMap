using System.Collections.Generic;

namespace DependencyMap.Filtering
{
    public interface IServiceDependencyFilterConfig
    {
        IEnumerable<string> DependencyIdsToInclude { get; }
    }
}