using NuGet;

namespace DependencyMap.Scanning
{
    public class ServiceDependency
    {
        public string ServiceId { get; set; }

        public string DependencyId { get; set; }

        public SemanticVersion DependencyVersion { get; set; }

        public string DependencyFilePath { get; set; }
    }
}
