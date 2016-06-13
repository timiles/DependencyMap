using DependencyMap.SourceRepositories;
using FluentAssertions;
using NUnit.Framework;

namespace DependencyMap.Tests.SourceRepositories
{
    [TestFixture]
    public class GitHubSourceRepositoryTests
    {
        [Test, Ignore("When anonymous, limitied to 60 requests per hour")]
        public void Test()
        {
            var gitHubSourceRepository = new GitHubSourceRepository("packages.config", "timiles");
            var files = gitHubSourceRepository.GetDependencyFilesToScan();

            files.Should().NotBeEmpty();
        }
    }
}