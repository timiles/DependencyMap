using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DependencyMap.Models;
using Octokit;
using Octokit.Internal;

namespace DependencyMap.SourceRepositories
{
    internal class GitHubSourceRepository : ISourceRepository
    {
        internal enum OwnerType
        {
            User,
            Organization
        }

        private readonly string _dependencyFileName;
        private readonly string _repositoryOwner;
        private readonly OwnerType _ownerType;
        private readonly Credentials _credentials;
        private readonly Uri _apiBaseAddress;

        /// <summary>
        /// Reads files from all repositories owned by a GitHub user or organization
        /// </summary>
        /// <param name="dependencyFileName">e.g. packages.config</param>
        /// <param name="repositoryOwner">user or organization name</param>
        /// <param name="ownerType">user or organization</param>
        /// <param name="login">Needed to prevent rate limiting</param>
        /// <param name="password">Needed to prevent rate limiting</param>
        /// <param name="apiBaseAddress">override this for Enterprise instances</param>
        public GitHubSourceRepository(
            string dependencyFileName,
            string repositoryOwner,
            OwnerType ownerType,
            string login = null,
            string password = null,
            string apiBaseAddress = null)
        {
            _dependencyFileName = dependencyFileName;
            _repositoryOwner = repositoryOwner;
            _ownerType = ownerType;
            _credentials = login != null ? new Credentials(login, password) : Credentials.Anonymous;
            _apiBaseAddress = apiBaseAddress != null ? new Uri(apiBaseAddress) : GitHubClient.GitHubApiUrl;
        }

        public IEnumerable<DependencyFile> GetDependencyFilesToScan()
        {
            return ReadFilesFromGitHub().Result;
        }

        private async Task<IEnumerable<DependencyFile>> ReadFilesFromGitHub()
        {
            var client = new GitHubClient(
                new ProductHeaderValue("DependencyMap"),
                new InMemoryCredentialStore(_credentials),
                _apiBaseAddress);

            var results = new List<DependencyFile>();

            var repositories = _ownerType == OwnerType.User
                ? await client.Repository.GetAllForUser(_repositoryOwner)
                : await client.Repository.GetAllForOrg(_repositoryOwner);

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
                    var files = await client.Search.SearchCode(searchCodeRequest);

                    foreach (var file in files.Items)
                    {
                        var contents =
                            await client.Repository.Content.GetAllContents(_repositoryOwner, repo.Name, file.Path);
                        if (contents.Any())
                        {
                            results.Add(new DependencyFile
                            {
                                ServiceId = repo.Name,
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