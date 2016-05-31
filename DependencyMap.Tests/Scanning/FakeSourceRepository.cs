using System.Collections.Generic;
using DependencyMap.Models;
using DependencyMap.SourceRepositories;

namespace DependencyMap.Tests.Scanning
{
    internal class FakeSourceRepository : ISourceRepository
    {
        private readonly IEnumerable<DependencyFile> _dependencyFiles;

        public FakeSourceRepository(params DependencyFile[] dependencyFiles)
        {
            _dependencyFiles = dependencyFiles;
        }

        public IEnumerable<DependencyFile> GetDependencyFilesToScan()
        {
            return _dependencyFiles;
        }
    }
}