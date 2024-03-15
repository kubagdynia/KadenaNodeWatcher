#
$RootPath = "./webapi"

$foldersToDelete = @(
    "$RootPath/DbConnectionExtensions"
    "$RootPath/KadenaNodeWatcher.Api"
    "$RootPath/KadenaNodeWatcher.Core"
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
    [pscustomobject]@{Source="../../DbConnectionExtensions";Target="$RootPath/DbConnectionExtensions"}
    [pscustomobject]@{Source="../../KadenaNodeWatcher.Api";Target="$RootPath/KadenaNodeWatcher.Api"}
    [pscustomobject]@{Source="../../KadenaNodeWatcher.Core";Target="$RootPath/KadenaNodeWatcher.Core"}
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

#### DELETE FILES - bin and obj

foreach($folder in $foldersToDelete)
{
    # Check if folder exists
    if (Test-Path $folder/bin)
    {
        # Folder exists, delete it!
        Remove-Item -Path $folder/bin -Recurse
    }

    if (Test-Path $folder/obj)
    {
        # Folder exists, delete it!
        Remove-Item -Path $folder/obj -Recurse
    }
}

### CREATE FOLDERS IF NOT EXISTS

if (-Not (Test-Path "../logs"))
{
    #powershell create directory
    New-Item -ItemType Directory -Path "../logs"
}