using NuGet;

namespace DependencyMap.Analysis
{
    // Implement as value object
    internal class DependencyIdVersion
    {
        public string DependencyId { get; }
        public SemanticVersion Version { get; }

        public DependencyIdVersion(string dependencyId, SemanticVersion version)
        {
            DependencyId = dependencyId;
            Version = version;
        }
        
        public override bool Equals(object other)
        {
            var dependencyIdVersion = other as DependencyIdVersion;
            if (dependencyIdVersion == null)
            {
                return false;
            }
            return string.Equals(DependencyId, dependencyIdVersion.DependencyId) &&
                   Equals(Version, dependencyIdVersion.Version);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((DependencyId?.GetHashCode() ?? 0)*397) ^ (Version?.GetHashCode() ?? 0);
            }
        }
    }
}