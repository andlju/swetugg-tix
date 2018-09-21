Param(
    [string]$location = "northeurope",
    [string]$resourceGroupName = "swetugg-tix",
    [string]$deploymentName = "swetugg-tix"
)

# create resource group
az group create -n $resourceGroupName -l $location

# deploy using the resource manager template
az group deployment create -g $resourceGroupName --template-file .\azuredeploy.json -n $deploymentName

$appName = az group deployment show -g $resourceGroupName -n $deploymentName --query properties.outputs.swetuggTixClientName.value -o tsv

Write-Host "AppName: $appName"
