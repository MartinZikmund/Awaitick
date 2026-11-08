---
description: How to write and run tests
---

# Testing

Tests live in **`tests/Awaitick.Core.Tests`** (MSTest on **Microsoft.Testing.Platform**, `net10.0`). CI runs them on every PR.

## Running
```bash
dotnet test tests/Awaitick.Core.Tests/Awaitick.Core.Tests.csproj
```
The runner is MTP, not VSTest, so **don't pass VSTest-only flags** like `--logger`. Filter with `--filter "FullyQualifiedName~MyClass"`.

## Conventions
- **TDD:** write a failing test first, watch it fail, then implement.
- Name tests `Method_Scenario_ExpectedResult`, structured Arrange, Act, Assert.
- Use small hand-written fakes for collaborators (see `Fakes/`).
- Assert with plain MSTest `Assert` and `CollectionAssert`.
- `IoC` is static, so tests touching it use `[DoNotParallelize]` and call `IoC.Reset()` in cleanup.

## Limits of `net10.0`
WinRT types such as `UISettings` and `DisplayRequest` construct and run here without throwing, but no platform sits behind them, so a passing call proves nothing. Put such types behind an interface and test the logic with a fake, as `KeepScreenOnService` does, then check the real behaviour in the running app.

`Packaging/` tests read tracked files directly, which suits rules the build can't enforce, such as keeping identity out of the manifest.
