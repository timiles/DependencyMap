using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octokit;
using Octokit.Internal;

namespace DependencyMap.SourceRepositories
{
    internal class GitHubSourceRepository : ISourceRepository
    {
        private readonly string _dependencyFileName;
        private readonly string _repositoryOwner;
        private readonly bool _ownerIsOrganization;
        private readonly Credentials _credentials;
        private readonly Uri _apiBaseAddress;

        private IEnumerable<DependencyFile> _dependencyFiles;

        /// <summary>
        /// Reads files from all repositories owned by a GitHub user or organization
        /// </summary>
        public GitHubSourceRepository(IGitHubSourceRepositoryConfig config)
        {
            _dependencyFileName = config.DependencyFileName;
            _repositoryOwner = config.RepositoryOwner;
            _ownerIsOrganization = config.OwnerIsOrganization;
            _credentials = config.Login != null ? new Credentials(config.Login, config.Password) : Credentials.Anonymous;
            _apiBaseAddress = config.ApiBaseAddress != null ? new Uri(config.ApiBaseAddress) : GitHubClient.GitHubApiUrl;
        }

        public IEnumerable<DependencyFile> GetDependencyFilesToScan()
        {
            return _dependencyFiles ?? (_dependencyFiles = ReadFilesFromGitHubAsync().Result);
        }

        private async Task<IEnumerable<DependencyFile>> ReadFilesFromGitHubAsync()
        {
            var client = new GitHubClient(
                new ProductHeaderValue("DependencyMap"),
                new InMemoryCredentialStore(_credentials),
                _apiBaseAddress);

            var results = new List<DependencyFile>();

            var repositories = _ownerIsOrganization
                ? await client.Repository.GetAllForOrg(_repositoryOwner).ConfigureAwait(false)
                : await client.Repository.GetAllForUser(_repositoryOwner).ConfigureAwait(false);

            foreach (var repo in repositories)
            {
                var page = 1;
                var totalResultsFound = 0;
                var moreResultsExist = true;
                while (moreResultsExist)
                {
                    var searchCodeRequest = new SearchCodeRequest(_dependencyFileName, _repositoryOwner, repo.Name)
                    {
                        In = new[] { CodeInQualifier.Path },
                        Page = page
                    };
                    var files = await client.Search.SearchCode(searchCodeRequest).ConfigureAwait(false);

                    foreach (var file in files.Items)
                    {
                        var contents =
                            await client.Repository.Content.GetAllContents(_repositoryOwner, repo.Name, file.Path).ConfigureAwait(false);
                        if (contents.Any())
                        {
                            results.Add(new DependencyFile
                            {
                                ServiceId = repo.Name,
                                FilePath = file.Path,
                                FileContents = contents[0].Content
                            });
                        }
                        else
                        {
                            // TODO: log when we can't get a file for whatever reason?
                        }
                    }

                    // TODO: check IncompleteResults?
                    totalResultsFound += files.Items.Count;
                    moreResultsExist = files.TotalCount > totalResultsFound;
                    page++;
                }
            }

            return results;
        }
    }
}