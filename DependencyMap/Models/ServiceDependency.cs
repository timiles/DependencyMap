using System;

namespace DependencyMap.Models
{
    internal class ServiceDependency
    {
        public string ServiceId { get; set; }

        public string DependencyId { get; set; }

        public Version DependencyVersion { get; set; }
    }
}
