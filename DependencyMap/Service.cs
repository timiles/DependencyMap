using System.Collections.Generic;
using NuGet;

namespace DependencyMap
{
    public class Service
    {
        public string ServiceId { get; set; }

        public int Score { get; set; }

        public IEnumerable<Dependency> Dependencies { get; set; }

        public class Dependency
        {
            public string DependencyId { get; set; }

            public SemanticVersion Version { get; set; }

            public SemanticVersion LatestKnownVersion { get; set; }

            public bool IsStale { get; set; }
        }
    }
}