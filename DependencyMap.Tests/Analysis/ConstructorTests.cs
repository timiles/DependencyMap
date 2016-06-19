using System;
using DependencyMap.Analysis;
using NUnit.Framework;

namespace DependencyMap.Tests.Analysis
{
    [TestFixture]
    public class ConstructorTests
    {
        [Test]
        public void NullList_ShouldThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => new ServiceDependenciesAnalyser(null));
        }
    }
}