using System.Collections.Generic;
using DependencyMap.Models;

namespace DependencyMap.SourceRepositories
{
    internal interface ISourceRepository
    {
        IEnumerable<DependencyFile> GetDependencyFilesToScan();
    }
}