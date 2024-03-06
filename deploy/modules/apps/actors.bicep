param location string

param containerAppsEnvironmentId string

param managedIdentityId string

param acrLoginServer string

param appInsightsConnectionString string

resource containerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: 'scavenger-actors'
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
          name: 'scavenger-actors'
          image: '${acrLoginServer}/scavenger.actors:latest'
          env: [
            {
              name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
              value: appInsightsConnectionString
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
        appId: 'scavenger-actors'
        appPort: 8080
      }
      ingress: {
        external: true
        targetPort: 8080
      }
      registries: [
        {
          server: acrLoginServer
          identity:  managedIdentityId
        }
      ]
    }
  }
}
