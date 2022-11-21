param containerAppName string
param location string = resourceGroup().location

param enableIngress bool
param ingressTargetPort int = 80
param userAssignedIdentityId string

param containerRegistryName string
param containerName string
param containerVersion string
param containerResources object = {
  cpu: json('0.25')
  memory: '0.5Gi'
}
param environmentVariables array

param enableDapr bool
param daprAppName string = containerName
param daprPort int = ingressTargetPort

param containerAppEnvironmentResourceName string
param containerAppEnvironmentResourceGroupName string

param minimumReplicas int = 1
param maximumReplicas int = 6

param enableHttpTrafficBasedScaling bool

var httpScaling = enableHttpTrafficBasedScaling ? [
  {
    name: 'http-rule'
    http: {
      metadata: {
        concurrentRequests: '30'
      }
    }
  }
] : []

var ingress = enableIngress ? {
  external: true
  targetPort: ingressTargetPort
  transport: 'auto'
  allowInsecure: false
  traffic: [
    {
      weight: 100
      latestRevision: true
    }
  ]
} : {}

var dapr = enableDapr ? {
  enabled: enableDapr
  appPort: daprPort
  appId: daprAppName
} : {}

resource containerAppEnvironments 'Microsoft.App/managedEnvironments@2022-03-01' existing = {
  name: containerAppEnvironmentResourceName
  scope: resourceGroup(containerAppEnvironmentResourceGroupName)
}

resource apiContainerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: containerAppName
  location: location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${userAssignedIdentityId}': {}
    }
  }
  properties: {
    managedEnvironmentId: containerAppEnvironments.id

    configuration: {
      activeRevisionsMode: 'Single'
      ingress: ingress
      dapr: dapr
      registries: [
        {
          server: containerRegistryName
          identity: userAssignedIdentityId
        }
      ]
    }
    template: {
      containers: [
        {
          image: '${containerRegistryName}/${containerName}:${containerVersion}'
          name: containerName
          resources: containerResources
          env: environmentVariables
        }
      ]
      scale: {
        minReplicas: minimumReplicas
        maxReplicas: maximumReplicas
        rules: httpScaling
      }
    }
  }
}
