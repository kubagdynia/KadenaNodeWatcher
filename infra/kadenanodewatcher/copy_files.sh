#!/bin/bash

GREEN='\033[0;32m'
RED='\033[0;31m'

WebApiPath="./webapi"

#### DELETE FILES

foldersToDeleted=(
    $WebApiPath/DbConnectionExtensions
    $WebApiPath/KadenaNodeWatcher.Api
    $WebApiPath/KadenaNodeWatcher.Core)

for folder in ${foldersToDeleted[@]}; do
    # Check if folder exists
    if [ -d $folder ]; then
        # Folder exists, delete it!
        rm -rf $folder
        echo -e "${GREEN}Folder Deleted at $folder"
    else
        echo -e "${RED}Folder $folder does not exist!"
    fi
done

#### COPY FILES

foldersToCopyFrom=(
    "../../DbConnectionExtensions"
    "../../KadenaNodeWatcher.Api"
    "../../KadenaNodeWatcher.Core"
)

foldersToCopyTo=(
    $WebApiPath/DbConnectionExtensions
    $WebApiPath/KadenaNodeWatcher.Api
    $WebApiPath/KadenaNodeWatcher.Core
)

for (( i = 0; i < ${#foldersToCopyFrom[@]}; ++i )); do
    # Check if folder exists
    if [ -d ${foldersToCopyFrom[i]} ]; then
        cp -r ${foldersToCopyFrom[i]} ${foldersToCopyTo[i]}
        echo -e "${GREEN}Folder Copied from ${foldersToCopyFrom[i]} to ${foldersToCopyTo[i]}"
    else
        echo -e "${RED}Folder ${foldersToCopyFrom[i]} does not exist!"
    fi
done

#### CREATE FOLDERS IF NOT EXISTS
if [ ! -d ../logs ]; then
    # Create folder
    mkdir -p ../logs
    echo -e "${GREEN}Folder Created at ../logs"
fi