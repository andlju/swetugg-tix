$resourceGroup = "testgroup"
$location = "northeurope"
$deploymentName = "testdeployment"

# create resource group
az group create -n $resourceGroup -l $location

# deploy using the resource manager template
az group deployment create -g $resourceGroup --template-file .\azuredeploy.json -n $deploymentName

$appName = az group deployment show -g $resourceGroup -n $deploymentName --query properties.outputs.swetuggTixClientName.value -o tsv

Write-Host "AppName: $appName"

# get the deployment credentials
$user = az webapp deployment list-publishing-profiles -n $appName -g $resourceGroup `
    --query "[?publishMethod=='MSDeploy'].userName" -o tsv

$pass = az webapp deployment list-publishing-profiles -n $appName -g $resourceGroup `
    --query "[?publishMethod=='MSDeploy'].userPWD" -o tsv

$pair = "$($user):$($pass)"
$encodedCreds = [System.Convert]::ToBase64String([System.Text.Encoding]::ASCII.GetBytes($pair))
$basicAuthValue = "Basic $encodedCreds"

$Headers = @{
    Authorization = $basicAuthValue
}

$sourceFilePath = ".\dist\Swetugg.Tix.Web.zip" # this is what you want to go into wwwroot

# use kudu deploy from zip file
Invoke-WebRequest -Uri https://$appName.scm.azurewebsites.net/api/zipdeploy -Headers $Headers `
    -InFile $sourceFilePath -ContentType "multipart/form-data" -Method Post