using NuGet;

namespace DependencyMap.Analysis
{
    internal class DependencyStaleness
    {
        public string DependencyId { get; set; }

        public SemanticVersion Version { get; set; }

        public SemanticVersion LatestKnownVersion { get; set; }

        public int StalenessRating { get; set; }

        public bool IsPrerelease => !string.IsNullOrEmpty(this.Version.SpecialVersion);
    }
}