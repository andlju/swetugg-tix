
$oldLocation = Get-Location 

Set-Location $PSScriptRoot\pulumi

pulumi stack select dev

pulumi up -y

#TODO Get names of frontpage and back-office apps + frontend resource group

#az webapp deployment source config-zip -g tix-dev-fe-group6025c16c -n tix-dev-backoffice477b2679 --src ..\dist\back-office.zip

#pulumi stack output activityAppSettings > ..\src\Swetugg.Tix.Activity.Funcs\local.settings.json
#pulumi stack output orderAppSettings > ..\src\Swetugg.Tix.Order.Funcs\local.settings.json
#pulumi stack output processAppSettings > ..\src\Swetugg.Tix.Process.Funcs\local.settings.json
#pulumi stack output apiAppSettings > ..\src\Swetugg.Tix.Api\local.settings.json

Set-Location $oldLocation
