using System;
using DependencyMap.Analysis;
using NUnit.Framework;

namespace DependencyMap.Tests.Analysis
{
    [TestFixture]
    public class ServiceDependenciesAnalyserTests
    {
        [Test]
        public void NullList_ShouldThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => new ServiceDependenciesAnalyser(null));
        }
    }
}