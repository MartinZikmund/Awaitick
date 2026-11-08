---
description: Branching, commit and versioning conventions
---

# Git and releases

## Commits
- Conventional Commits: `type: Subject`, where type is `feat`, `fix`, `chore`, `docs`, `refactor`, `test`, `ci` or `build`.
- Capitalize the subject, use the imperative mood, no trailing period.
- Split a bug fix and its test into two commits, fix first.

## Branches
- Work on `feature/<short-name>` or `fix/<short-name>` off `main`, and open a PR back to `main`.
- CI runs on PRs against any base, so stacked PRs get checks too.
- Releases happen on `release/v{major}.{minor}`. **Pushing to one publishes builds**, so never create one casually. Use the Prepare Release workflow. See [docs/versioning.md](../../docs/versioning.md).

## Versioning
- Nerdbank.GitVersioning computes versions from `version.json` and git height.
- **Don't hand-edit version numbers** in the csproj or manifest.
