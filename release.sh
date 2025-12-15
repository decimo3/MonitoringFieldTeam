#!/bin/env bash

WORKDIR=$PWD

# Fetch tags and get the latest one
git fetch --tags
version=$(git describe --tags $(git rev-list --tags --max-count=1))
version_number="${version#v}"  # Remove 'v' prefix if the tag has it
echo "Version: $version_number"

# Extract major, minor, and patch versions
IFS='.' read -r MAJOR_VERSION MINOR_VERSION PATCH_VERSION <<< "$version_number"
export MAJOR_VERSION MINOR_VERSION PATCH_VERSION
echo "Major version: $MAJOR_VERSION"
echo "Minor version: $MINOR_VERSION"
echo "Patch version: $PATCH_VERSION"

# Write version file
envsubst < src/ofs.csproj > src/ofs.csproj.tmp
mv src/ofs.csproj.tmp src/ofs.csproj
cat src/ofs.csproj

# Restore configuration file
cp src/ofs.conf src/ofs.conf.bak
git restore src/ofs.conf

# Build release
dotnet publish -c release

# Compress executable and related files
cd src/bin/Release/net7.0/win-x64/publish/
zip -r ofs_bot.zip .
mv ofs_bot.zip $WORKDIR
cd $WORKDIR

# Create a release on GitHub
gh release create $version --verify-tag --notes-file release.md --title "OFS_BOT ${version} release" ofs_bot.zip

# Reverting placeholder files
git restore src/ofs.csproj
mv src/ofs.conf.bak src/ofs.conf
