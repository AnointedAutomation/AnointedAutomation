# Copyright © Anointed Automation, LLC., 2025. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2025-06-08 13:27:40
# Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
#!/bin/bash

# Manual NuGet Package Publisher
# NOTE: Automatic publishing happens via GitHub Actions when merging to master with version changes

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo -e "${CYAN}============================================"
echo "Manual NuGet Package Publisher"
echo -e "============================================${NC}"
echo -e "${YELLOW}NOTE: Packages are automatically published when${NC}"
echo -e "${YELLOW}      merging to master with version changes${NC}"
echo

# Check if NuGet API key is provided
if [ -z "$NUGET_API_KEY" ]; then
    echo -e "${RED}Error: NUGET_API_KEY environment variable is not set${NC}"
    echo "Usage: export NUGET_API_KEY=your_api_key"
    echo "       ./publish-packages.sh"
    exit 1
fi

# Function to get latest version from NuGet
get_nuget_version() {
    local package_name=$1
    local response=$(curl -s "https://api.nuget.org/v3-flatcontainer/$package_name/index.json")
    
    # Check if package exists
    if [[ $response == *"\"versions\":[]"* ]] || [[ -z $response ]] || [[ $response == *"NotFound"* ]]; then
        echo "0.0.0"
    else
        # Extract versions and get the latest
        echo "$response" | grep -o '"[^"]*"' | sed 's/"//g' | sort -V | tail -n 1
    fi
}

# Function to extract version from csproj
get_version() {
    local csproj=$1
    grep -o '<Version>[^<]*</Version>' "$csproj" | sed 's/<Version>\(.*\)<\/Version>/\1/'
}

# Define packages to check
declare -A PACKAGES=(
    ["AnointedAutomation.Algorithms/AnointedAutomation.Algorithms.csproj"]="anointedautomation.algorithms"
    ["AnointedAutomation.Imaging/AnointedAutomation.Imaging.csproj"]="anointedautomation.imaging"
    ["AnointedAutomation.Logging/AnointedAutomation.Logging.csproj"]="anointedautomation.logging"
    ["AnointedAutomation.Memory/AnointedAutomation.Memory.csproj"]="anointedautomation.memory"
    ["AnointedAutomation.APIMiddlewares/AnointedAutomation.APIMiddlewares.csproj"]="anointedautomation.apimiddlewares"
    ["AnointedAutomation.Repository.Mongo/AnointedAutomation.Repository.Mongo.csproj"]="anointedautomation.repository.mongo"
    ["AnointedAutomation.Objects/AnointedAutomation.Objects.csproj"]="anointedautomation.objects"
    ["AnointedAutomation.Objects.API/AnointedAutomation.Objects.API.csproj"]="anointedautomation.objects.api"
    ["AnointedAutomation.Concepts/AnointedAutomation.Concepts.csproj"]="anointedautomation.concepts"
)

# Clean previous builds
echo -e "${YELLOW}Cleaning previous builds...${NC}"
rm -rf nupkgs

# Check current branch
CURRENT_BRANCH=$(git branch --show-current)
if [ "$CURRENT_BRANCH" != "master" ]; then
    echo -e "${YELLOW}WARNING: You are on branch '$CURRENT_BRANCH', not 'master'${NC}"
    echo -e "${YELLOW}         Automatic publishing only happens from master branch${NC}"
    echo
fi

# Restore dependencies
echo -e "${YELLOW}Restoring dependencies...${NC}"
dotnet restore > /dev/null 2>&1
if [ $? -ne 0 ]; then
    echo -e "${RED}ERROR: Failed to restore dependencies${NC}"
    exit 1
fi

# Build solution
echo -e "${YELLOW}Building solution in Release mode...${NC}"
dotnet build --configuration Release --no-restore > /dev/null 2>&1
if [ $? -ne 0 ]; then
    echo -e "${RED}ERROR: Build failed${NC}"
    exit 1
fi

# Run tests
echo -e "${YELLOW}Running all tests...${NC}"
dotnet test --configuration Release --no-build --verbosity minimal
if [ $? -ne 0 ]; then
    echo -e "${RED}ERROR: Tests failed! Fix failing tests before publishing.${NC}"
    exit 1
fi

# Create nupkgs directory
mkdir -p nupkgs

# Check packages for version changes
echo -e "${YELLOW}Checking packages for version changes...${NC}"
echo

PUBLISHED_COUNT=0
SKIPPED_COUNT=0

for PROJECT_PATH in "${!PACKAGES[@]}"; do
    PACKAGE_NAME="${PACKAGES[$PROJECT_PATH]}"
    
    echo -e "Checking ${CYAN}$PACKAGE_NAME${NC}..."
    
    # Get local version
    LOCAL_VERSION=$(get_version "$PROJECT_PATH")
    if [ -z "$LOCAL_VERSION" ]; then
        echo -e "  ${RED}No version found in project file - skipping${NC}"
        ((SKIPPED_COUNT++))
        continue
    fi
    
    # Get NuGet version
    NUGET_VERSION=$(get_nuget_version "$PACKAGE_NAME")
    
    echo "  Local version:  $LOCAL_VERSION"
    echo "  NuGet version:  $NUGET_VERSION"
    
    # Compare versions
    if [ "$LOCAL_VERSION" != "$NUGET_VERSION" ]; then
        echo -e "  ${GREEN}Version changed - will publish${NC}"
        
        # Pack the project
        echo -e "  ${YELLOW}Packing...${NC}"
        dotnet pack "$PROJECT_PATH" --configuration Release --no-build --output ./nupkgs
        
        # Find the package file
        PACKAGE_FILE=$(find "./nupkgs" -name "${PACKAGE_NAME}.${LOCAL_VERSION}.nupkg" -type f | head -1)
        
        if [ -f "$PACKAGE_FILE" ]; then
            # Push to NuGet
            echo -e "  ${YELLOW}Publishing to NuGet.org...${NC}"
            if dotnet nuget push "$PACKAGE_FILE" --api-key "$NUGET_API_KEY" --source https://api.nuget.org/v3/index.json --skip-duplicate; then
                echo -e "  ${GREEN}Successfully published!${NC}"
                ((PUBLISHED_COUNT++))
            else
                echo -e "  ${RED}Failed to publish${NC}"
            fi
        else
            echo -e "  ${RED}Package file not found${NC}"
        fi
    else
        echo -e "  ${YELLOW}Version unchanged - skipping${NC}"
        ((SKIPPED_COUNT++))
    fi
    
    echo
done

# Clean up
rm -rf ./nupkgs

# Summary
echo "============================================"
echo -e "${GREEN}Published: $PUBLISHED_COUNT packages${NC}"
echo -e "${YELLOW}Skipped:   $SKIPPED_COUNT packages${NC}"
echo "============================================"

if [ $PUBLISHED_COUNT -gt 0 ]; then
    echo -e "${GREEN}Publishing completed successfully!${NC}"
else
    echo -e "${YELLOW}No packages needed to be published.${NC}"
    echo -e "${YELLOW}To publish, increment version numbers in .csproj files${NC}"
fi
