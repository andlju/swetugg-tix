Param(
    [string]$resourceGroupName = "swetugg-tix-new",
    [string]$deploymentName = "swetugg-tix",
    [string]$version = "latest"
)

[Net.ServicePointManager]::SecurityProtocol = "tls12, tls11, tls"

Write-Host "Downloading $version release"
if ($version -eq "latest") {
    $releaseInfo = (Invoke-RestMethod -uri "https://api.github.com/repos/andlju/swetugg-tix/releases/latest")
} else {
    $releaseInfo = (Invoke-RestMethod -uri "https://api.github.com/repos/andlju/swetugg-tix/releases/tags/swetugg-tix-build-v$version")
}

if (-not $releaseInfo.name)
{
    Write-Error "Unable to download latest relase"
    Write-Error $releaseInfo
    Exit 1
}

if (Test-Path -Path ".\deploytmp") {
    Remove-Item -Path ".\deploytmp" -Recurse -Force
}

New-Item -ItemType Directory -Path ".\deploytmp"

foreach ($asset in $releaseInfo.assets) {
    $assetName = $asset.name
    $downloadUrl = $asset.browser_download_url

    Write-Host "Downloading $assetName from $downloadUrl"

    Invoke-WebRequest -Uri $downloadUrl -OutFile ".\deploytmp\$assetName"
}

Write-Host "Finding the name of the Swetugg Client App"

$appName = az group deployment show -g $resourceGroupName -n $deploymentName --query properties.outputs.swetuggTixClientName.value -o tsv

Write-Host "AppName: $appName"

# get the deployment credentials
$user = az webapp deployment list-publishing-profiles -n $appName -g $resourceGroupName `
    --query "[?publishMethod=='MSDeploy'].userName" -o tsv

$pass = az webapp deployment list-publishing-profiles -n $appName -g $resourceGroupName `
    --query "[?publishMethod=='MSDeploy'].userPWD" -o tsv

$pair = "$($user):$($pass)"
$encodedCreds = [System.Convert]::ToBase64String([System.Text.Encoding]::ASCII.GetBytes($pair))
$basicAuthValue = "Basic $encodedCreds"

$Headers = @{
    Authorization = $basicAuthValue
}

$sourceFilePath = ".\deploytmp\Swetugg.Tix.Web.zip" # this is what you want to go into wwwroot

Write-Host "Deploying Swetugg Client"

# use kudu deploy from zip file
$deployResult = Invoke-WebRequest -Uri https://$appName.scm.azurewebsites.net/api/zipdeploy -Headers $Headers `
    -InFile $sourceFilePath -ContentType "multipart/form-data" -Method Post

if ($deployResult.StatusCode -ne 200) {
    Write-Error "Failed when uploading to Kudu"
    Write-Error $deployResult.Content

    Exit 1
}

$Headers = @{
    Authorization = $basicAuthValue;
    "Content-Disposition" = "attachement; filename=run.cmd"
}

$sourceFilePath = ".\deploytmp\Swetugg.Tix.Activity.Jobs.zip" # this is what you want to go into wwwroot

Write-Host "Deploying Activity Jobs"
# Now deploy the first web job
$deployResult = Invoke-WebRequest -Uri https://$appName.scm.azurewebsites.net/api/continuouswebjobs/ActivityJobs -Headers $Headers `
    -InFile $sourceFilePath -ContentType "application/zip" -Method Put

if ($deployResult.StatusCode -ne 200) {
    Write-Error "Failed when uploading to Kudu"
    Write-Error $deployResult.Content

    Exit 1
}

# Cleanup the temporary files
Remove-Item -Path ".\deploytmp" -Recurse -Force
