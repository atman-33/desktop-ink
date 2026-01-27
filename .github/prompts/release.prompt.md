---
name: release-desktop-ink
description: Execute the release process for Desktop Ink
argument-hint: Optionally provide the version number to release (e.g., 1.2.0). If not provided, the agent will prompt for it.
---

$ARGUMENTS

**Purpose**
Automate the release process following the documented workflow in `docs/RELEASE.md`.

**Pre-requisites Check**
Before proceeding, verify:
1. All changes are committed and pushed to the `main` branch
2. All tests are passing (run `scripts\test.cmd` to confirm)
3. The version number to release (if not provided, ask the user)

**Release Steps**
1. **Confirm Version**: If the version is not specified, ask the user for the target version number (e.g., 1.2.0)
2. **Bump Version**: Run `scripts\bump-version.cmd <version>` to update the version and create a git tag
3. **Push Changes**: Push the commit and tag to remote using `git push && git push origin v<version>`
4. **Monitor CI/CD**: GitHub Actions will automatically build, test, and create the release
5. **Verify Release**: Check the [Releases page](https://github.com/atman-33/desktop-ink/releases) to confirm the new release

**Important Notes**
- Follow Semantic Versioning: MAJOR.MINOR.PATCH
- The auto-generated release notes will appear in the in-app update notification
- For detailed workflow information, refer to `docs/RELEASE.md`
