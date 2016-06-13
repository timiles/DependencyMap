using System.Collections.Generic;
using DependencyMap.SourceRepositories;

namespace DependencyMap.Scanning
{
    public interface IDependencyFileScanner
    {
        IEnumerable<ServiceDependency> GetAllServiceDependencies(IEnumerable<DependencyFile> dependencyFiles);
    }
}