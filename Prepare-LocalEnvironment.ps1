
$oldLocation = Get-Location 

Set-Location $PSScriptRoot\pulumi

pulumi stack select local

pulumi up -y

pulumi stack output activityAppSettings > ..\src\Swetugg.Tix.Activity.Funcs\local.settings.json
pulumi stack output orderAppSettings > ..\src\Swetugg.Tix.Order.Funcs\local.settings.json
pulumi stack output processAppSettings > ..\src\Swetugg.Tix.Process.Funcs\local.settings.json
pulumi stack output apiAppSettings > ..\src\Swetugg.Tix.Api\local.settings.json

Set-Location $oldLocation
