g_dependencies = [
  {
    "DependencyId": "FreshGlobally",
    "Score": 1,
    "ServiceUsageByVersion": {
      "1.0.0": [
        "SolutionA",
        "SolutionB"
      ]
    }
  },
  {
    "DependencyId": "MultipleVersions",
    "Score": 3,
    "ServiceUsageByVersion": {
      "1.0.1": [
        "SolutionA"
      ],
      "1.0.0": [
        "SolutionA"
      ],
      "0.0.99": [
        "SolutionB"
      ]
    }
  },
  {
    "DependencyId": "Package1",
    "Score": 1,
    "ServiceUsageByVersion": {
      "1.0.0": [
        "SolutionA"
      ]
    }
  },
  {
    "DependencyId": "Package2",
    "Score": 1,
    "ServiceUsageByVersion": {
      "1.0.0": [
        "SolutionA"
      ]
    }
  },
  {
    "DependencyId": "Package3",
    "Score": 1,
    "ServiceUsageByVersion": {
      "1.0.0": [
        "SolutionA"
      ]
    }
  }
];
g_services = [
  {
    "ServiceId": "SolutionA",
    "Score": 1,
    "Dependencies": [
      {
        "DependencyId": "FreshGlobally",
        "Version": "1.0.0",
        "LatestKnownVersion": "1.0.0",
        "IsStale": false
      },
      {
        "DependencyId": "MultipleVersions",
        "Version": "1.0.1",
        "LatestKnownVersion": "1.0.1",
        "IsStale": false
      },
      {
        "DependencyId": "Package1",
        "Version": "1.0.0",
        "LatestKnownVersion": "1.0.0",
        "IsStale": false
      },
      {
        "DependencyId": "Package2",
        "Version": "1.0.0",
        "LatestKnownVersion": "1.0.0",
        "IsStale": false
      },
      {
        "DependencyId": "Package3",
        "Version": "1.0.0",
        "LatestKnownVersion": "1.0.0",
        "IsStale": false
      },
      {
        "DependencyId": "MultipleVersions",
        "Version": "1.0.0",
        "LatestKnownVersion": "1.0.1",
        "IsStale": true
      }
    ]
  },
  {
    "ServiceId": "SolutionB",
    "Score": 2,
    "Dependencies": [
      {
        "DependencyId": "FreshGlobally",
        "Version": "1.0.0",
        "LatestKnownVersion": "1.0.0",
        "IsStale": false
      },
      {
        "DependencyId": "MultipleVersions",
        "Version": "0.0.99",
        "LatestKnownVersion": "1.0.1",
        "IsStale": true
      }
    ]
  }
];