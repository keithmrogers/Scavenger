param location string

param containerAppsEnvironmentId string

param managedIdentityId string

resource containerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: 'scavenger-api'
  location: location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${managedIdentityId}': {}
    }
  }
  properties: {
    managedEnvironmentId: containerAppsEnvironmentId
    template: {
      containers: [
        {
          name: 'scavenger-api'
          image: 'scavenger/scavenger.api:latest'
          env: [
            {
              name: 'ASPNETCORE_URLS'
              value: 'http://0.0.0.0:80'
            }
          ]      
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }

    }
    configuration: {
      activeRevisionsMode: 'single'
      dapr: {
        enabled: true
        appId: 'scavenger-api'
        appPort: 80
      }
      ingress: {
        external: true
        targetPort: 80
      }
    }
  }
}