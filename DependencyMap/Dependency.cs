using System;
using System.Collections.Generic;

namespace DependencyMap
{
    public class Dependency
    {
        public string DependencyId { get; set; }

        public int Score { get; set; }

        public IDictionary<Version, string[]> ServiceUsageByVersion { get; set; }
    }
}