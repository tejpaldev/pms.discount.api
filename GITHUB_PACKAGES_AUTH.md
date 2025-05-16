# GitHub Packages Authentication Setup

This document explains how to set up authentication for GitHub Packages in this project.

## The Issue

The project depends on private NuGet packages hosted on GitHub Packages:
- `Crypto.EventBus.Messages`
- `Crypto.Common.Logging`

These packages require proper authentication to access.

## Solution

We've updated the GitHub Actions workflow to use a Personal Access Token (PAT) with the appropriate permissions to access GitHub Packages.

### Required GitHub Secret

You need to add a secret to your GitHub repository:

- **GIT_PAT**: A GitHub Personal Access Token with the `read:packages` scope

### Creating a Personal Access Token (PAT)

1. Go to your GitHub account settings
2. Click on "Developer settings" in the left sidebar
3. Click on "Personal access tokens" and then "Tokens (classic)"
4. Click "Generate new token" and then "Generate new token (classic)"
5. Give your token a descriptive name (e.g., "NuGet Packages Access")
6. Select the following scopes:
   - `read:packages` (to download packages)
   - `repo` (if your packages are in private repositories)
7. Click "Generate token"
8. Copy the token immediately (you won't be able to see it again)

### Adding the Secret to Your Repository

1. Go to your GitHub repository
2. Click on "Settings"
3. Click on "Secrets and variables" in the left sidebar
4. Click on "Actions"
5. Click on "New repository secret"
6. Enter "GIT_PAT" as the name
7. Paste your Personal Access Token as the value
8. Click "Add secret"

## Changes Made

1. Updated the GitHub Actions workflow to use the GIT_PAT secret instead of the default GITHUB_TOKEN
2. Added explicit permissions to the build job
3. Improved the NuGet authentication setup in the workflow
4. Enhanced the package restore process to use direct token substitution

## Testing Locally

To test GitHub Packages authentication locally:

1. Create a Personal Access Token as described above
2. Update your local NuGet.config file:
   ```xml
   <?xml version="1.0" encoding="utf-8"?>
   <configuration>
     <packageSources>
       <clear />
       <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
       <add key="github" value="https://nuget.pkg.github.com/tejpaldev/index.json" />
     </packageSources>
     <packageSourceCredentials>
       <github>
         <add key="Username" value="YOUR_GITHUB_USERNAME" />
         <add key="ClearTextPassword" value="YOUR_GITHUB_PAT" />
       </github>
     </packageSourceCredentials>
   </configuration>
   ```
3. Run `dotnet restore` to test the authentication

## Troubleshooting

If you still encounter authentication issues:

1. Verify that your PAT has not expired
2. Ensure your PAT has the correct scopes
3. Check that your GitHub account has access to the repository hosting the packages
4. Try manually configuring the source:
   ```
   dotnet nuget add source https://nuget.pkg.github.com/tejpaldev/index.json --name github --username YOUR_GITHUB_USERNAME --password YOUR_GITHUB_PAT --store-password-in-clear-text
   ```
