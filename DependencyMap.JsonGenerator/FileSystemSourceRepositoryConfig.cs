using DependencyMap.SourceRepositories;

namespace DependencyMap.JsonGenerator
{
    internal class FileSystemSourceRepositoryConfig : IFileSystemSourceRepositoryConfig
    {
        internal FileSystemSourceRepositoryConfig(params string[] rootFolders)
        {
            RootFolders = rootFolders;
        }

        public string DependencyFileName => "packages.config";

        public string[] RootFolders { get; }
    }
}