using System;

namespace DependencyMap.Models
{
    internal class DependencyStaleness
    {
        public string DependencyId { get; set; }

        public Version Version { get; set; }

        public Version LatestKnownVersion { get; set; }

        public int StalenessRating { get; set; }
    }
}