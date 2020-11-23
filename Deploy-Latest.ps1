
$oldLocation = Get-Location 

Set-Location $PSScriptRoot\pulumi

pulumi stack select dev

pulumi up -y

#TODO Get names of frontpage and back-office apps + frontend resource group
$json = pulumi stack output frontendSettings | ConvertFrom-Json

$frontendResourceGroupName = $json.frontendResourceGroupName
$frontpageAppName = $json.frontpageAppName
$backOfficeAppName = $json.backOfficeAppName

az webapp deployment source config-zip -g $frontendResourceGroupName -n $backOfficeAppName --src ..\dist\back-office.zip
az webapp deployment source config-zip -g $frontendResourceGroupName -n $frontpageAppName --src ..\dist\frontpage.zip

Set-Location $oldLocation
