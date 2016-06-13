﻿using System;
using System.Collections.Generic;
using System.Linq;
using DependencyMap.SourceRepositories;
using FluentAssertions;
using NUnit.Framework;

namespace DependencyMap.Tests.Generator
{
    [TestFixture]
    public class GetAllDependenciesTests
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
                var list = generator.GetAllDependencies().ToList();
            };
            a.ShouldThrow<DependencyFilesNotFoundException>();
        }
    }
}