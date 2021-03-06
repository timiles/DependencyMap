﻿using System;
using System.Linq;
using DependencyMap.Scanning;
using DependencyMap.SourceRepositories;
using FluentAssertions;
using NuGet;
using NUnit.Framework;

namespace DependencyMap.Tests.Scanning
{
    [TestFixture]
    public class NuGetPackageConfigScannerTests
    {
        private NuGetPackageConfigScanner _scanner;

        [SetUp]
        public void GivenNewNuGetPackageConfigScanner()
        {
            _scanner = new NuGetPackageConfigScanner();
        }

        [Test]
        public void WhenSourceRepositoryReturnsNull_ThenExcepionIsThrown()
        {
            Action a = () =>
            {
                var list = _scanner.GetAllServiceDependencies(null).ToList();
            };
            a.ShouldThrow<DependencyFilesNotFoundException>();
        }

        [Test]
        public void WhenSourceRepositoryReturnsEmptyList_ThenExcepionIsThrown()
        {
            Action a = () =>
            {
                var list = _scanner.GetAllServiceDependencies(new DependencyFile[0]).ToList();
            };
            a.ShouldThrow<DependencyFilesNotFoundException>();
        }

        [Test]
        public void EmptyFile_ShouldYieldNothing()
        {
            var dependencyFile = new DependencyFile { ServiceId = @"MyService", FileContents = @"" };
            var serviceDependencies = _scanner.GetAllServiceDependencies(new[] {dependencyFile});
            serviceDependencies.Should().BeEmpty();
        }

        [Test]
        public void FileWithPackages_ShouldYieldCorrectIdsAndVersions()
        {
            var dependencyFile = new DependencyFile
            {
                ServiceId = @"MyService",
                FileContents = @"<?xml version=""1.0"" encoding=""utf-8""?>
<packages>
  <package id=""Package0"" version=""0.0.0.1"" targetFramework=""net451"" />
  <package id=""Package1"" version=""1.2.345"" targetFramework=""net451"" />
  <package id=""Package2"" version=""2.0"" targetFramework=""net451"" />
</packages>"
            };

            var serviceDependencies = _scanner.GetAllServiceDependencies(new[] {dependencyFile}).ToList();
            serviceDependencies.Count.Should().Be(3);

            foreach (var serviceDependency in serviceDependencies)
            {
                serviceDependency.ServiceId.Should().Be(dependencyFile.ServiceId);
            }

            serviceDependencies[0].DependencyId.Should().Be("Package0");
            serviceDependencies[0].DependencyVersion.Should().Be(new SemanticVersion(0, 0, 0, 1));

            serviceDependencies[1].DependencyId.Should().Be("Package1");
            serviceDependencies[1].DependencyVersion.Should().Be(new SemanticVersion(1, 2, 345, 0));

            serviceDependencies[2].DependencyId.Should().Be("Package2");
            serviceDependencies[2].DependencyVersion.Should().Be(new SemanticVersion(2, 0, 0, 0));
        }

        [Test]
        public void MultipleFiles_ShouldYieldCorrectServiceIdsWithDependencyIdsAndVersions()
        {
            var files = new []
            {
                new DependencyFile
                {
                    ServiceId = @"MyService1",
                    FileContents = @"<package id=""MyPackage"" version=""1.0.0"" targetFramework=""net451"" />"
                },
                new DependencyFile
                {
                    ServiceId = @"MyService2",
                    FileContents = @"<package id=""MyPackage"" version=""2.0.0"" targetFramework=""net451"" />
<package id=""MyPackage2"" version=""3.0.0"" targetFramework=""net451"" />"
                },
                new DependencyFile
                {
                    ServiceId = @"MyService2",
                    FileContents = @"<package id=""MyPackage"" version=""2.0.0"" targetFramework=""net451"" />"
                }
            };

            var serviceDependencies = _scanner.GetAllServiceDependencies(files).ToList();
            serviceDependencies.Count.Should().Be(4);

            serviceDependencies[0].ServiceId.Should().Be("MyService1");
            serviceDependencies[0].DependencyId.Should().Be("MyPackage");
            serviceDependencies[0].DependencyVersion.Should().Be(new SemanticVersion(1, 0, 0, 0));

            serviceDependencies[1].ServiceId.Should().Be("MyService2");
            serviceDependencies[1].DependencyId.Should().Be("MyPackage");
            serviceDependencies[1].DependencyVersion.Should().Be(new SemanticVersion(2, 0, 0, 0));

            serviceDependencies[2].ServiceId.Should().Be("MyService2");
            serviceDependencies[2].DependencyId.Should().Be("MyPackage2");
            serviceDependencies[2].DependencyVersion.Should().Be(new SemanticVersion(3, 0, 0, 0));

            serviceDependencies[3].ServiceId.Should().Be("MyService2");
            serviceDependencies[3].DependencyId.Should().Be("MyPackage");
            serviceDependencies[3].DependencyVersion.Should().Be(new SemanticVersion(2, 0, 0, 0));
        }

        [Test]
        public void PrereleaseVersionValue_ShouldParseCorrectly()
        {
            var dependencyFile = new DependencyFile
            {
                ServiceId = @"MyService",
                FileContents = @"<package id=""AlphaPackage"" version=""0.0.1-alpha"" targetFramework=""net451"" />"
            };

            var serviceDependencies = _scanner.GetAllServiceDependencies(new[] {dependencyFile}).ToList();
            serviceDependencies.Count.Should().Be(1);

            serviceDependencies[0].ServiceId.Should().Be("MyService");
            serviceDependencies[0].DependencyId.Should().Be("AlphaPackage");
            serviceDependencies[0].DependencyVersion.Should().Be(new SemanticVersion(0, 0, 1, "alpha"));
        }

        [Test]
        public void InvalidVersionValue_ShouldYieldNullVersion()
        {
            var dependencyFile = new DependencyFile
            {
                ServiceId = @"MyService",
                FileContents = @"<package id=""InvalidPackage"" version=""invalid"" targetFramework=""net451"" />"
            };

            var serviceDependencies = _scanner.GetAllServiceDependencies(new[] {dependencyFile}).ToList();
            serviceDependencies.Count.Should().Be(1);

            serviceDependencies[0].ServiceId.Should().Be("MyService");
            serviceDependencies[0].DependencyId.Should().Be("InvalidPackage");
            serviceDependencies[0].DependencyVersion.Should().BeNull();
        }
    }
}
