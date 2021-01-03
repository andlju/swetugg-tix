
$oldLocation = Get-Location 

Set-Location $PSScriptRoot\pulumi

pulumi stack select local

pulumi up -y

pulumi stack output activityAppSettings --show-secrets > ..\src\Swetugg.Tix.Activity.Funcs\local.settings.json
pulumi stack output orderAppSettings --show-secrets > ..\src\Swetugg.Tix.Order.Funcs\local.settings.json
pulumi stack output processAppSettings --show-secrets > ..\src\Swetugg.Tix.Process.Funcs\local.settings.json
pulumi stack output apiAppSettings --show-secrets > ..\src\Swetugg.Tix.Api\local.settings.json

Set-Location $oldLocation
