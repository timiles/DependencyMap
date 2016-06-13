using System;
using System.Collections.Generic;
using System.Linq;
using DependencyMap.SourceRepositories;
using FluentAssertions;
using NUnit.Framework;

namespace DependencyMap.Tests.Generator
{
    [TestFixture]
    public class GetAllServicesTests
    {
        [Test]
        public void WhenSourceRepositoryReturnsNull_ThenExcepionIsThrown()
        {
            var sourceRepository = new Moq.Mock<ISourceRepository>();
            sourceRepository.Setup(x => x.GetDependencyFilesToScan())
                .Returns(null as IEnumerable<DependencyFile>);
            var generator = new DependencyMap.Generator(sourceRepository.Object);

            Action a = () =>
            {
                var list = generator.GetAllServices().ToList();
            };
            a.ShouldThrow<DependencyFilesNotFoundException>();
        }

        [Test]
        public void WhenSourceRepositoryReturnsEmptyList_ThenExcepionIsThrown()
        {
            var sourceRepository = new Moq.Mock<ISourceRepository>();
            sourceRepository.Setup(x => x.GetDependencyFilesToScan())
                .Returns(new DependencyFile[0]);
            var generator = new DependencyMap.Generator(sourceRepository.Object);

            Action a = () =>
            {
                var list = generator.GetAllServices().ToList();
            };
            a.ShouldThrow<DependencyFilesNotFoundException>();
        }
    }
}