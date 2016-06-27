using System.Collections.Generic;
using DependencyMap.Analysis;

namespace WebView.DataUpdater.DataModels
{
    public class Anaylsis
    {
        public IEnumerable<Dependency> Dependencies { get; set; }
        public IEnumerable<Service> Services { get; set; }
    }
}