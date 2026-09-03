# Trademarks and brand assets

Awaitick's **source code** is free software under the [AGPLv3](LICENSE). Its **brand is
not**. This page explains where the line is.

A free-software licence covers copyright in the code. It does not grant rights in a name or
a logo — those are trademarks, and they exist so that users can tell who actually made the
thing they installed. Keeping them reserved is what stops a modified build from being
mistaken for the official one.

## What is reserved

- The name **"Awaitick"**, and names confusingly similar to it.
- The **Awaitick logo and app icon**, in every form and every size
  (`src/Awaitick/Assets/Icons/`, `src/Awaitick/Platforms/*/`).
- The **splash screen artwork** and other Awaitick-branded graphics
  (`src/Awaitick/Assets/Splash/`).
- The visual identity these assets make up.

These files sit in the repository because the app cannot be built without them. Their
presence is not a licence to use them.

## What you may do

- Use, study, modify and share the **source code**, under the AGPL.
- **Say what your fork is based on** — factual, descriptive references are fine and always
  will be. "Built on Awaitick", "a fork of Awaitick", "compatible with Awaitick" are all
  fine.
- Use the name and icon in **reviews, articles, tutorials, talks and screenshots**. That is
  ordinary descriptive use and needs no permission.
- Build and run your own modified version **privately**, brand and all. Nothing here
  restricts what you do on your own machine.

## What you may not do

- **Publish** a modified version under the name "Awaitick", or under a name likely to be
  confused with it.
- Ship a modified version using the **Awaitick logo or icon**.
- Suggest your fork is **official, endorsed by, or affiliated with** Awaitick or Martin
  Zikmund.
- Use the brand in your **company, product, domain or social media name**.

## Rebranding a fork

If you publish a fork, change the following. This is the practical checklist, not extra
legal fine print:

| What | Where |
|---|---|
| App name | `<ApplicationTitle>` in `src/Awaitick/Awaitick.csproj` |
| App identifier | `<ApplicationId>` in `src/Awaitick/Awaitick.csproj` |
| Windows identity and display name | `src/Awaitick/Package.appxmanifest` |
| iOS bundle name and identifier | `src/Awaitick/Platforms/iOS/Info.plist` |
| Displayed app name | `ApplicationName` in `src/Awaitick/Strings/*/Resources.resw` |
| Icons and logo | `src/Awaitick/Assets/Icons/`, `src/Awaitick/Platforms/*/` |
| Splash artwork | `src/Awaitick/Assets/Splash/` |

You will have to change the identifiers anyway — the stores will not accept a second app
claiming the same ones.

## Asking for permission

Want to use the name or logo in a way that is not covered above? Just ask:
<martin@zikmund.dev>. Reasonable requests get reasonable answers, and permission for a
specific use is usually easy to give.
