namespace DependencyMap.SourceRepositories
{
    public interface IGitHubSourceRepositoryConfig
    {
        /// <summary>
        /// e.g. packages.config
        /// </summary>
        string DependencyFileName { get; }

        /// <summary>
        /// user or organization name
        /// </summary>
        string RepositoryOwner { get; }

        /// <summary>
        /// false: RepositoryOwner is user; true: RepositoryOwner is organization
        /// </summary>
        bool OwnerIsOrganization { get; }

        /// <summary>
        /// Needed to prevent rate limiting
        /// </summary>
        string Login { get; }

        /// <summary>
        /// Needed to prevent rate limiting
        /// </summary>
        string Password { get; }

        /// <summary>
        /// override this for Enterprise instances; defaults to public GitHub API
        /// </summary>
        string ApiBaseAddress { get; }
    }
}