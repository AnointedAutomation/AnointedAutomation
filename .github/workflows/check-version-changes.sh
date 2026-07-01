# Copyright © Anointed Automation, LLC., 2025. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2025-06-08 15:20:26
# Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
#!/bin/bash

# Script to check if package version has changed compared to NuGet.org
# Usage: ./check-version-changes.sh <project-file> <package-name>

PROJECT_FILE="$1"
PACKAGE_NAME="$2"

# Extract version from project file
LOCAL_VERSION=$(grep -o '<Version>[^<]*</Version>' "$PROJECT_FILE" | sed 's/<Version>\(.*\)<\/Version>/\1/')

if [ -z "$LOCAL_VERSION" ]; then
    echo "No <Version> tag found in $PROJECT_FILE"
    exit 1
fi

# Get the latest version from NuGet.org
echo "Checking NuGet.org for $PACKAGE_NAME..."
NUGET_VERSION=$(curl -s "https://api.nuget.org/v3-flatcontainer/$PACKAGE_NAME/index.json" | grep -o '"[^"]*"' | sed 's/"//g' | sort -V | tail -n 1)

if [ -z "$NUGET_VERSION" ]; then
    echo "Package $PACKAGE_NAME not found on NuGet.org or error occurred"
    echo "LOCAL_VERSION=$LOCAL_VERSION"
    echo "SHOULD_PUBLISH=true"
    exit 0
fi

echo "Local version: $LOCAL_VERSION"
echo "NuGet version: $NUGET_VERSION"

# Compare versions
if [ "$LOCAL_VERSION" = "$NUGET_VERSION" ]; then
    echo "Version unchanged - skipping publish"
    echo "SHOULD_PUBLISH=false"
else
    echo "Version changed - will publish"
    echo "SHOULD_PUBLISH=true"
fi
