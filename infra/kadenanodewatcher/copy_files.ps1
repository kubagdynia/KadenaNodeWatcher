#
$WebApiPath = "./webapi"

$foldersToDelete = @(
    "$WebApiPath/DbConnectionExtensions"
    "$WebApiPath/KadenaNodeWatcher.Api"
    "$WebApiPath/KadenaNodeWatcher.Core"
)

#### DELETE FILES

foreach($folder in $foldersToDelete)
{
    # Check if folder exists
    if (Test-Path $folder)
    {
        # Folder exists, delete it!
        Remove-Item -Path $folder -Recurse
        Write-host "Folder Deleted at" $dir -f Green
    }
    else {
        Write-host "Folder" $folder "does not exist!" -f Red
    }
}

#### COPY FILES

$foldersToCopy = @(
    [pscustomobject]@{Source="../../DbConnectionExtensions";Target="$WebApiPath/DbConnectionExtensions"}
    [pscustomobject]@{Source="../../KadenaNodeWatcher.Api";Target="$WebApiPath/KadenaNodeWatcher.Api"}
    [pscustomobject]@{Source="../../KadenaNodeWatcher.Core";Target="$WebApiPath/KadenaNodeWatcher.Core"}
)

foreach($data in $foldersToCopy)
{
    # Check if folder exists
    if (Test-Path $data.Source)
    {
        Copy-Item $data.Source -Destination $data.Target -Recurse
        Write-Host "Folder Copied from" $data.Source "To" $data.Target -f Green
    }
    else {
        Write-host "Folder" $data.Source "does not exist!" -f Red
    }
}

### CREATE FOLDERS IF NOT EXISTS

if (-Not (Test-Path "../logs"))
{
    #powershell create directory
    New-Item -ItemType Directory -Path "../logs"
}