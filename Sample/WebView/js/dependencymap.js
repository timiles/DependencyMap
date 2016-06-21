var dependencymap = {
    buildDependencies: function (dependencies) {
        var divs = [];
        for (var i = 0; i < dependencies.length; i++) {
            var dependency = dependencies[i];

            var dependencyContainer = document.createElement('div');
            dependencyContainer.className = 'grid-item score-' + dependency.Score;

            var header = document.createElement('h2');
            header.innerHTML = dependency.DependencyId;
            dependencyContainer.appendChild(header);

            var versionList = document.createElement('ul');
            for (var version in dependency.ServiceUsageByVersion) {
                var serviceVersion = dependency.ServiceUsageByVersion[version];

                var serviceList = document.createElement('ul');
                for (var s = 0; s < serviceVersion.length; s++) {
                    var versionListItem = document.createElement('li');
                    versionListItem.innerText = serviceVersion[s];
                    serviceList.appendChild(versionListItem);
                }

                var serviceListItem = document.createElement('li');
                serviceListItem.innerHTML = version;
                serviceListItem.appendChild(serviceList);
                versionList.appendChild(serviceListItem);
            }

            dependencyContainer.appendChild(versionList);
            divs.push(dependencyContainer);
        }
        return divs;
    },

    buildServices: function (services) {
        var divs = [];
        for (var i = 0; i < services.length; i++) {
            var service = services[i];

            var serviceContainer = document.createElement('div');
            serviceContainer.className = 'grid-item score-' + service.Score;

            var header = document.createElement('h2');
            header.innerHTML = service.ServiceId;
            serviceContainer.appendChild(header);

            var dependenciesList = document.createElement('ul');
            dependenciesList.className = 'dependencies-list';
            for (var d = 0; d < service.Dependencies.length; d++) {
                var dependency = service.Dependencies[d];
                var dependencyListItem = document.createElement('li');
                dependencyListItem.className = 'dependency dependency-' + (dependency.IsStale ? 'stale' : 'fresh');
                dependencyListItem.innerHTML = dependency.DependencyId + ' <span class="version">' + dependency.Version + ' &lt; ' + dependency.LatestKnownVersion + '</span>';

                dependenciesList.appendChild(dependencyListItem);
            }

            serviceContainer.appendChild(dependenciesList);
            divs.push(serviceContainer);
        }
        return divs;
    }
}