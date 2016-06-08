using System.Collections.Generic;
using System.IO;
using DependencyMap.Models;

namespace DependencyMap.SourceRepositories
{
    internal class FileSystemSourceRepository : ISourceRepository
    {
        private readonly string _dependencyFileName;
        private readonly string[] _rootFolders;

        private IEnumerable<DependencyFile> _dependencyFiles;

        public FileSystemSourceRepository(IFileSystemSourceRepositoryConfig config)
        {
            _dependencyFileName = config.DependencyFileName;
            _rootFolders = config.RootFolders;
        }

        public IEnumerable<DependencyFile> GetDependencyFilesToScan()
        {
            return _dependencyFiles ?? (_dependencyFiles = ReadFilesFromFileSystem());
        }

        private IEnumerable<DependencyFile> ReadFilesFromFileSystem()
        {
            foreach (var rootFolder in this._rootFolders)
            {
                foreach (var serviceDir in Directory.GetDirectories(rootFolder))
                {
                    var serviceId = new DirectoryInfo(serviceDir).Name;
                    var dependencyFilePaths = Directory.EnumerateFiles(serviceDir, _dependencyFileName,
                        SearchOption.AllDirectories);

                    foreach (var filePath in dependencyFilePaths)
                    {
                        yield return new DependencyFile
                        {
                            ServiceId = serviceId,
                            FileContents = File.ReadAllText(filePath)
                        };
                    }
                }
            }
        }
    }
}
