$oldLocation = Get-Location 

Set-Location $PSScriptRoot\pulumi

$config = Get-Content '.\.azure-ad.secret.json' | Out-String | ConvertFrom-Json

pulumi config set azure-ad-b2c-tenant-name $config.tenantName
pulumi config set azure-ad-b2c-tenant-id $config.tenantId
pulumi config set azure-ad-b2c-policy-name $config.policyName

pulumi config set azure-ad-b2c-backoffice-app $config.backofficeApp
pulumi config set --secret azure-ad-b2c-backoffice-app-secret $config.backofficeAppSecret
pulumi config set azure-ad-b2c-api-app $config.apiApp
pulumi config set --secret azure-ad-b2c-api-app-secret $config.apiAppSecret

Set-Location $oldLocation
