
$oldLocation = Get-Location 

Set-Location $PSScriptRoot\pulumi

pulumi stack select dev

# pulumi up -y

$json = pulumi stack output backendSettings | ConvertFrom-Json

$backendResourceGroupName = $json.backendResourceGroupName
$activityAppName = $json.activityAppName
$orderAppName = $json.orderAppName
$processAppName = $json.processAppName
$apiAppName = $json.apiAppName

az webapp deployment source config-zip -g $backendResourceGroupName -n $activityAppName --src ..\dist\Swetugg.Tix.Activity.Funcs.zip
az webapp deployment source config-zip -g $backendResourceGroupName -n $orderAppName --src ..\dist\Swetugg.Tix.Order.Funcs.zip
az webapp deployment source config-zip -g $backendResourceGroupName -n $processAppName --src ..\dist\Swetugg.Tix.Process.Funcs.zip
az webapp deployment source config-zip -g $backendResourceGroupName -n $apiAppName --src ..\dist\Swetugg.Tix.Api.zip

Set-Location $oldLocation
