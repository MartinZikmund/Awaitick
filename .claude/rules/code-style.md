---
description: C# language, naming and dependency conventions
---

# Code style

These mirror `.editorconfig` and `src/Directory.Build.props`.

## Language
- `LangVersion=preview`, `Nullable=enable`, `ImplicitUsings=enable`. Use modern C# where it reads more clearly.
- File-scoped namespaces. Tabs for indentation in C#, project files and JSON. XAML is formatted by XAML Styler.
- Always use braces, including for single-line statements.
- Expression bodies for single-line methods and properties.
- Explicit type with target-typed `new()` (`List<string> items = new();`). Use `var` when the type isn't on the left, such as a method result.
- Keep new code warning-clean. Fix the cause rather than suppressing it.

## Naming
- `_camelCase` for private and internal fields. PascalCase for constants, types and members.
- `I` prefix for interfaces, `T` prefix for type parameters.

## Dependencies
- Central Package Management: versions live only in `src/Directory.Packages.props`. Reference packages with no `Version=` attribute.
- `tests/` is outside that scope, so it sets its own versions.
- **Check the license before adding a package.** FluentAssertions 8+ needs a paid license for commercial use, and Awaitick sells a Pro tier.

## Comments
- Comment the why in a line or two. Let clear names carry the what. No walls of text.
