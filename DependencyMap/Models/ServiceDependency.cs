using NuGet;

namespace DependencyMap.Models
{
    internal class ServiceDependency
    {
        public string ServiceId { get; set; }

        public string DependencyId { get; set; }

        public SemanticVersion DependencyVersion { get; set; }
    }
}
