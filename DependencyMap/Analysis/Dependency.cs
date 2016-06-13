using System.Collections.Generic;
using NuGet;

namespace DependencyMap.Analysis
{
    public class Dependency
    {
        public string DependencyId { get; set; }

        public int Score { get; set; }

        public IDictionary<SemanticVersion, string[]> ServiceUsageByVersion { get; set; }
    }
}