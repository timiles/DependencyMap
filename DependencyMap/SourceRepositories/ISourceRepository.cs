using System.Collections.Generic;

namespace DependencyMap.SourceRepositories
{
    public interface ISourceRepository
    {
        IEnumerable<DependencyFile> GetDependencyFilesToScan();
    }
}