using NuGet;

namespace DependencyMap.Models
{
    internal class DependencyStaleness
    {
        public string DependencyId { get; set; }

        public SemanticVersion Version { get; set; }

        public SemanticVersion LatestKnownVersion { get; set; }

        public int StalenessRating { get; set; }
    }
}