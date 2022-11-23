param defaultResourceName string
param location string
param storageAccountTables array
param containerVersion string
param integrationResourceGroupName string
param containerAppEnvironmentResourceName string
param azureAppConfigurationName string

param containerRegistryResourceGroupName string = 'Containers'
param containerRegistryResourceName string = 'ekereg.azurecr.io'

param containerPort int = 7127
param containerName string = 'blackjack-users-functions'

resource appConfiguration 'Microsoft.AppConfiguration/configurationStores@2022-05-01' existing = {
  name: azureAppConfigurationName
  scope: resourceGroup(integrationResourceGroupName)
}

resource containerAppIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' = {
  name: defaultResourceName
  location: location
}

resource acrPullRole 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' existing = {
  scope: resourceGroup()
  name: '7f951dda-4ed3-4680-a7ca-43fe172d538d'
}
module acrPullRoleAssignment 'roleAssignment.bicep' = {
  name: 'acrPullRoleAssignment'
  scope: resourceGroup(containerRegistryResourceGroupName)
  params: {
    principalId: containerAppIdentity.properties.principalId
    roleDefinitionId: acrPullRole.id
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-09-01' = {
  name: uniqueString(defaultResourceName)
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}
resource storageAccountTableService 'Microsoft.Storage/storageAccounts/tableServices@2021-09-01' = {
  name: 'default'
  parent: storageAccount
}
resource storageAccountTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2021-09-01' = [for table in storageAccountTables: {
  name: table
  parent: storageAccountTableService
}]

var environmentVariables = [
  {
    name: 'AzureAppConfiguration'
    value: appConfiguration.properties.endpoint
  }
  {
    name: 'AzureWebJobsStorage'
    value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=${environment().suffixes.storage}'
  }
  {
    name: 'StorageAccountName'
    value: 'https://${storageAccount.name}.table.${environment().suffixes.storage}'
  }
  {
    name: 'FUNCTIONS_WORKER_RUNTIME'
    value: 'dotnet-isolated'
  }
]

// module azureContainerApp 'br/CoreModules:containerapp:0.1.9' = {
//   name: 'ContainerAppModule'
//   dependsOn: [
//     roleAssignmentsModule
//   ]
//   params: {
//     containerAppName: '${defaultResourceName}-aca'
//     containerAppEnvironmentResourceGroupName: integrationResourceGroupName
//     containerAppEnvironmentResourceName: containerAppEnvironmentResourceName
//     containerName: containerName
//     containerRegistryName: containerRegistryResourceName
//     containerVersion: containerVersion
//     enableDapr: true
//     daprPort: containerPort
//     enableHttpTrafficBasedScaling: true
//     enableIngress: true
//     userAssignedIdentityId: containerAppIdentity.id
//     environmentVariables: environmentVariables
//   }
// }
module containerAppModuleModule 'containerapp.bicep' = {
  name: 'ContainerAppModuleModule'
  dependsOn: [
    acrPullRoleAssignment
  ]
  params: {
    containerAppName: '${defaultResourceName}-aca'
    location: location
    containerAppEnvironmentResourceGroupName: integrationResourceGroupName
    containerAppEnvironmentResourceName: containerAppEnvironmentResourceName
    containerName: containerName
    containerRegistryName: containerRegistryResourceName
    containerVersion: containerVersion
    enableDapr: true
    daprPort: containerPort
    enableHttpTrafficBasedScaling: true
    enableIngress: true
    userAssignedIdentityId: containerAppIdentity.id
    environmentVariables: environmentVariables
  }
}

module roleAssignmentsModule 'all-role-assignments.bicep' = {
  name: 'roleAssignmentsModule'
  params: {
    containerAppPrincipalId: containerAppModuleModule.outputs.principalId
    integrationResourceGroupName: integrationResourceGroupName
  }
}
