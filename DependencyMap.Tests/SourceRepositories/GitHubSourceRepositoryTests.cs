using DependencyMap.SourceRepositories;
using FluentAssertions;
using NUnit.Framework;

namespace DependencyMap.Tests.SourceRepositories
{
    [TestFixture]
    public class GitHubSourceRepositoryTests
    {
        internal class TestGitHubSourceRepositoryConfig : IGitHubSourceRepositoryConfig
        {
            internal TestGitHubSourceRepositoryConfig(string login = null, string password = null, string apiBaseAddress = null)
            {
                Login = login;
                Password = password;
                ApiBaseAddress = apiBaseAddress;
            }

            public string DependencyFileName => "packages.config";
            public string RepositoryOwner => "timiles";
            public bool OwnerIsOrganization => false;
            public string Login { get; }
            public string Password { get; }
            public string ApiBaseAddress { get; }
        }

        [Test, Ignore("When anonymous, limitied to 60 requests per hour")]
        public void Test()
        {
            var gitHubSourceRepository = new GitHubSourceRepository(new TestGitHubSourceRepositoryConfig());
            var files = gitHubSourceRepository.GetDependencyFilesToScan();

            files.Should().NotBeEmpty();
        }
    }
}