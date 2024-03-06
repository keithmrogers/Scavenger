param location string = resourceGroup().location
param uniqueSeed string = '${resourceGroup().id}-${deployment().name}'

////////////////////////////////////////////////////////////////////////////////
// Infrastructure
////////////////////////////////////////////////////////////////////////////////

module managedIdentity 'modules/infra/identity.bicep' = {
  name: '${deployment().name}-infra-managed-identity'
  params: {
    location: location
    uniqueSeed: uniqueSeed
  }
}

module acr 'modules/infra/container-registry.bicep' = {
  name: '${deployment().name}-infra-container-registry'
  params: {
    location: location
    uniqueSeed: uniqueSeed
    managedIdentityObjectId: managedIdentity.outputs.identityObjectId
  }
}

module containerAppsEnvironment 'modules/infra/container-apps-env.bicep' = {
  name: '${deployment().name}-infra-container-app-env'
  params: {
    location: location
    uniqueSeed: uniqueSeed
  }
}

module cosmos 'modules/infra/cosmos-db.bicep' = {
  name: '${deployment().name}-infra-cosmos-db'
  params: {
    location: location
    uniqueSeed: uniqueSeed
  }
}

module serviceBus 'modules/infra/service-bus.bicep' = {
  name: '${deployment().name}-infra-service-bus'
  params: {
    location: location
    uniqueSeed: uniqueSeed
  }
}

module keyVault 'modules/infra/keyvault.bicep' = {
  name: '${deployment().name}-infra-keyvault'
  params: {
    location: location
    uniqueSeed: uniqueSeed
    managedIdentityObjectId: managedIdentity.outputs.identityObjectId
  }
}

////////////////////////////////////////////////////////////////////////////////
// Dapr components
////////////////////////////////////////////////////////////////////////////////

module daprPubSub 'modules/dapr/pubsub.bicep' = {
  name: '${deployment().name}-dapr-pubsub'
  params: {
    containerAppsEnvironmentName: containerAppsEnvironment.outputs.name
    serviceBusConnectionString: serviceBus.outputs.connectionString
  }
}

module daprStateStore 'modules/dapr/statestore.bicep' = {
  name: '${deployment().name}-dapr-statestore'
  params: {
    containerAppsEnvironmentName: containerAppsEnvironment.outputs.name
    cosmosDbName: cosmos.outputs.cosmosDbName
    cosmosCollectionName: cosmos.outputs.cosmosCollectionName
    cosmosUrl: cosmos.outputs.cosmosUrl
    cosmosKey: cosmos.outputs.cosmosKey
  }
}

module daprSecretStore 'modules/dapr/secretstore.bicep' = {
  name: '${deployment().name}-dapr-secretstore'
  params: {
    containerAppsEnvironmentName: containerAppsEnvironment.outputs.name
    vaultName: keyVault.outputs.vaultName
    managedIdentityClientId: managedIdentity.outputs.identityClientId
  }
}

////////////////////////////////////////////////////////////////////////////////
// Container apps
////////////////////////////////////////////////////////////////////////////////

module scavengerActors 'modules/apps/actors.bicep' = {
  name: '${deployment().name}-app-actors'
  dependsOn: [
    daprPubSub
    daprStateStore
  ]
  params: {
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
    managedIdentityId: managedIdentity.outputs.identityId
    acrLoginServer: acr.outputs.acrLoginServer
    appInsightsConnectionString: containerAppsEnvironment.outputs.appInsightsConnectionString
  }
}

module scavengerApi 'modules/apps/api.bicep' = {
  name: '${deployment().name}-app-api'
  dependsOn: [
    daprPubSub
  ]
  params: {
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
    managedIdentityId: managedIdentity.outputs.identityId
    acrLoginServer: acr.outputs.acrLoginServer
    appInsightsConnectionString: containerAppsEnvironment.outputs.appInsightsConnectionString
  }
}
