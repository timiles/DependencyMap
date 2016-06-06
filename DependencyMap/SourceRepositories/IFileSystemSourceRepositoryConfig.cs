namespace DependencyMap.SourceRepositories
{
    public interface IFileSystemSourceRepositoryConfig
    {
        string DependencyFileName { get; }
        string[] RootFolders { get; }
    }
}