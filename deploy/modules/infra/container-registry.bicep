param location string = resourceGroup().location

param uniqueSeed string

param managedIdentityObjectId string

param registryName string = 'registry${uniqueString(uniqueSeed)}'

param acrPullDefinitionId string = '7f951dda-4ed3-4680-a7ca-43fe172d538d'

resource acr 'Microsoft.ContainerRegistry/registries@2021-09-01' = {
  name: registryName
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: false
  }
}

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, acr.name, 'AcrPullTestUserAssigned')
  scope: acr
  properties: {
    principalId: managedIdentityObjectId  
    principalType: 'ServicePrincipal'
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', acrPullDefinitionId)
  }
}

output acrLoginServer string = acr.properties.loginServer
output acrName string = acr.name
