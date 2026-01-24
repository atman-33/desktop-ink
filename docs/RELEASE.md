# Release Process

This document describes the version management and release workflow for Desktop Ink.

## Overview

The release process is automated using GitHub Actions. When you push a version tag, the CI/CD pipeline automatically builds, tests, and creates a GitHub Release with distribution files.

## Prerequisites

- Git configured with write access to the repository
- PowerShell (for version bump script)
- All tests passing locally

## Version Numbering

Desktop Ink follows [Semantic Versioning](https://semver.org/):

- **Major version** (X.0.0): Breaking changes or major new features
- **Minor version** (0.X.0): New features, backward compatible
- **Patch version** (0.0.X): Bug fixes and minor improvements

## Release Workflow

### Step 1: Prepare Changes

1. Ensure all changes are committed and pushed to the `main` branch
2. All tests should be passing:
   ```cmd
   scripts\test.cmd
   ```

### Step 2: Bump Version

Use the version bump script to update the version number and create a git tag:

```cmd
# Update version to 1.1.0 (example)
scripts\bump-version.cmd 1.1.0
```

This script will:
- ✅ Update `Version` and `FileVersion` in `DesktopInk.csproj`
- ✅ Stage and commit the changes
- ✅ Create a git tag `v1.1.0`

**Optional: Auto-push**

To automatically push changes and tag:

```cmd
scripts\bump-version.cmd 1.1.0 -Push
```

### Step 3: Push to Remote

If you didn't use the `-Push` flag, manually push the changes:

```cmd
git push
git push origin v1.1.0
```

### Step 4: Automated Release

Once the tag is pushed, GitHub Actions automatically:

1. **Runs CI Tests** - Validates all unit tests pass
2. **Builds Two Distributions**:
   - Self-contained (includes .NET runtime, ~191MB)
   - Framework-dependent (requires .NET 10 installed, ~173MB)
3. **Creates GitHub Release** with:
   - Auto-generated release notes
   - Both distribution executables attached
   - Proper naming: `DesktopInk-v1.1.0-win-x64.exe`

Auto-generated release notes are displayed in the in-app update notification dialog, so keep them concise and user-facing.

### Step 5: Verify Release

1. Go to the [Releases page](https://github.com/atman-33/desktop-ink/releases)
2. Verify the new release appears
3. Check that both distribution files are attached:
   - `DesktopInk-vX.X.X-win-x64.exe` (self-contained)
   - `DesktopInk-vX.X.X-win-x64-framework.exe` (framework-dependent)
4. Test download and execution

## Distribution Files

### Self-Contained (`-win-x64.exe`)

- **Size**: ~191MB
- **Pros**: No .NET installation required, runs anywhere
- **Cons**: Larger file size
- **Recommended for**: End users who may not have .NET installed

### Framework-Dependent (`-win-x64-framework.exe`)

- **Size**: ~173MB
- **Pros**: Smaller file size
- **Cons**: Requires [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0)
- **Recommended for**: Developers or users with .NET already installed

## Manual Build (If Needed)

If you need to build releases locally without GitHub Actions:

```cmd
# Self-contained
scripts\publish.cmd

# Framework-dependent
scripts\publish-framework.cmd

# Both
scripts\publish-all.cmd
```

Output files will be in:
- `publish/win-x64-self-contained/DesktopInk.exe`
- `publish/win-x64-framework/DesktopInk.exe`

## Troubleshooting

### Tag Already Exists

If the tag already exists and you need to recreate it:

```powershell
# Delete local tag
git tag -d v1.1.0

# Delete remote tag
git push origin :refs/tags/v1.1.0

# Recreate tag
git tag -a v1.1.0 -m "Release version 1.1.0"
git push origin v1.1.0
```

The bump-version script will also prompt you to delete and recreate if a tag exists.

### GitHub Actions Failed

1. Check the [Actions tab](https://github.com/atman-33/desktop-ink/actions)
2. Review the failed workflow logs
3. Fix the issue and re-run the workflow or create a new tag

### Build Failed Locally

Ensure you have the correct .NET SDK:

```cmd
dotnet --version
# Should show 10.0.x or higher
```

## CI/CD Configuration

The release automation is configured in:

- **CI Workflow**: `.github/workflows/ci.yml`
  - Runs on every push/PR to main, develop, feature branches
  - Validates builds and tests

- **Release Workflow**: `.github/workflows/release.yml`
  - Triggers on `v*.*.*` tags
  - Builds distributions and creates GitHub Release

## Quick Reference

```cmd
# Full release process (automatic push)
scripts\bump-version.cmd 1.2.0 -Push

# Manual process
scripts\bump-version.cmd 1.2.0
git push
git push origin v1.2.0

# Local build only
scripts\publish-all.cmd
```

## Best Practices

1. ✅ Always run tests before bumping version
2. ✅ Update CHANGELOG or release notes if needed
3. ✅ Use semantic versioning consistently
4. ✅ Verify the GitHub Release after automated creation
5. ✅ Test downloaded executables before announcing release
6. ✅ Keep version numbers in sync across the project
