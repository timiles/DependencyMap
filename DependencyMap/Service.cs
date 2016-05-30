using System;
using System.Collections.Generic;

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

            public Version Version { get; set; }

            public Version LatestKnownVersion { get; set; }

            public bool IsStale { get; set; }
        }
    }
}