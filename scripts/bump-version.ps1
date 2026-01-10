#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Bump version number in project file and create git tag
    
.DESCRIPTION
    Updates the version number in DesktopInk.csproj and creates a git tag.
    
.PARAMETER Version
    New version number (e.g., 1.0.0, 1.1.0, 2.0.0)
    
.PARAMETER Push
    Automatically push the tag to remote repository
    
.EXAMPLE
    .\bump-version.ps1 1.1.0
    .\bump-version.ps1 1.1.0 -Push
#>

param(
    [Parameter(Mandatory=$true)]
    [ValidatePattern('^\d+\.\d+\.\d+$')]
    [string]$Version,
    
    [Parameter(Mandatory=$false)]
    [switch]$Push
)

$ErrorActionPreference = "Stop"

# Get repository root
$repoRoot = Split-Path -Parent $PSScriptRoot
$projectFile = Join-Path $repoRoot "src\DesktopInk\DesktopInk.csproj"

if (-not (Test-Path $projectFile)) {
    Write-Error "Project file not found: $projectFile"
    exit 1
}

Write-Host "Updating version to $Version..." -ForegroundColor Cyan

# Read project file
$xml = [xml](Get-Content $projectFile)

# Update Version and FileVersion
$propertyGroup = $xml.Project.PropertyGroup | Where-Object { $_.Version -ne $null } | Select-Object -First 1
if ($null -eq $propertyGroup) {
    # If no Version element exists, find first PropertyGroup and add it
    $propertyGroup = $xml.Project.PropertyGroup | Select-Object -First 1
    $versionElement = $xml.CreateElement("Version")
    $versionElement.InnerText = $Version
    $propertyGroup.AppendChild($versionElement) | Out-Null
    
    $fileVersionElement = $xml.CreateElement("FileVersion")
    $fileVersionElement.InnerText = $Version
    $propertyGroup.AppendChild($fileVersionElement) | Out-Null
} else {
    $propertyGroup.Version = $Version
    if ($propertyGroup.FileVersion -ne $null) {
        $propertyGroup.FileVersion = $Version
    } else {
        $fileVersionElement = $xml.CreateElement("FileVersion")
        $fileVersionElement.InnerText = $Version
        $propertyGroup.AppendChild($fileVersionElement) | Out-Null
    }
}

# Save project file
$xml.Save($projectFile)
Write-Host "✓ Updated $projectFile" -ForegroundColor Green

# Git operations
$tagName = "v$Version"

# Check if tag already exists
$existingTag = git tag -l $tagName
if ($existingTag) {
    Write-Warning "Tag $tagName already exists!"
    $response = Read-Host "Do you want to delete and recreate it? (y/N)"
    if ($response -ne 'y' -and $response -ne 'Y') {
        Write-Host "Aborted." -ForegroundColor Yellow
        exit 0
    }
    git tag -d $tagName
    if ($Push) {
        git push origin ":refs/tags/$tagName"
    }
}

# Stage changes
git add $projectFile
Write-Host "✓ Staged changes" -ForegroundColor Green

# Commit
git commit -m "chore: bump version to $Version"
Write-Host "✓ Created commit" -ForegroundColor Green

# Create tag
git tag -a $tagName -m "Release version $Version"
Write-Host "✓ Created tag $tagName" -ForegroundColor Green

if ($Push) {
    # Push commit and tag
    Write-Host "`nPushing to remote..." -ForegroundColor Cyan
    git push
    git push origin $tagName
    Write-Host "✓ Pushed to remote" -ForegroundColor Green
    Write-Host "`n✓ GitHub Actions will automatically create a release for $tagName" -ForegroundColor Green
} else {
    Write-Host "`nTo push the changes and trigger release, run:" -ForegroundColor Yellow
    Write-Host "  git push" -ForegroundColor White
    Write-Host "  git push origin $tagName" -ForegroundColor White
}

Write-Host "`n✓ Version bumped to $Version successfully!" -ForegroundColor Green
