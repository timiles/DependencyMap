using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octokit;
using Octokit.Internal;

namespace DependencyMap.SourceRepositories
{
    public class GitHubSourceRepository : ISourceRepository
    {
        private readonly string _dependencyFileName;
        private readonly string _repositoryOwner;
        private readonly bool _ownerIsOrganization;
        private readonly Credentials _credentials;
        private readonly Uri _apiBaseAddress;
        private readonly int _searchApiRateLimitDelayMilliseconds;

        private IEnumerable<DependencyFile> _dependencyFiles;

        /// <summary>
        /// Reads files from all repositories owned by a GitHub user or organization
        /// </summary>
        /// <param name="dependencyFileName">e.g. packages.config</param>
        /// <param name="repositoryOwner">user or organization name</param>
        /// <param name="ownerIsOrganization">false: RepositoryOwner is user; true: RepositoryOwner is organization</param>
        /// <param name="login">Only needed if anonymous rate limiting applies (eg public GitHub API)</param>
        /// <param name="password">Only needed if anonymous rate limiting applies (eg public GitHub API)</param>
        /// <param name="apiBaseAddress">override this for Enterprise instances; defaults to public GitHub API</param>
        public GitHubSourceRepository(string dependencyFileName, string repositoryOwner,
            bool ownerIsOrganization = false, string login = null, string password = null, string apiBaseAddress = null)
        {
            _dependencyFileName = dependencyFileName;
            _repositoryOwner = repositoryOwner;
            _ownerIsOrganization = ownerIsOrganization;
            _credentials = login != null ? new Credentials(login, password) : Credentials.Anonymous;
            _apiBaseAddress = apiBaseAddress != null ? new Uri(apiBaseAddress) : GitHubClient.GitHubApiUrl;

            if (_apiBaseAddress == GitHubClient.GitHubApiUrl)
            {
                /*
                From https://developer.github.com/v3/search/#rate-limit:
                For requests using Basic Authentication, OAuth, or client ID and secret, you can make up to 30 requests per minute. 
                For unauthenticated requests, the rate limit allows you to make up to 10 requests per minute.
                */
                _searchApiRateLimitDelayMilliseconds = login != null ? 60000/30 : 60000/10;
            }
        }

        public IEnumerable<DependencyFile> GetDependencyFiles()
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
                    await Task.Delay(_searchApiRateLimitDelayMilliseconds).ConfigureAwait(false);

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