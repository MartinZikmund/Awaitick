# Licensing

Plain-English guide to how Awaitick is licensed. The authoritative texts are
[`LICENSE`](LICENSE) (GNU AGPLv3) and [`COPYING.iOS`](COPYING.iOS) (the App Store
exception) — where this page and those files disagree, those files win.

## The short version

Awaitick is free software under the **GNU Affero General Public License, version 3 or
later** (`AGPL-3.0-or-later`). You may use it, study it, change it and share it. If you
share a changed version — or run one as a network service — you must give its users the
same freedoms, including the source code.

Two things sit outside the AGPL: an **exception** that makes App Store distribution
possible, and the **Awaitick brand**, which is not licensed at all.

## What you may do

- **Use the app** for anything, including commercially. No conditions.
- **Read and study the source.** That is the whole point of publishing it.
- **Modify it** for your own use. Private changes never have to be published.
- **Share it**, modified or not, provided you follow the two rules below.

## What the AGPL asks in return

**1. Pass on the source.** If you give someone a copy of Awaitick — modified or not — you
must also offer them the complete corresponding source code of *that* copy, under this
same licence. Shipping a binary without the source is the one thing the licence does not
allow.

**2. Keep it under the AGPL.** You cannot relicense Awaitick, or a work built from it,
under a more restrictive licence. This is what "copyleft" means: the freedoms travel with
the code.

## The network clause (section 13)

This is the difference between the AGPL and the ordinary GPL, and it is why it was chosen
here.

Under a normal GPL, obligations only trigger when you *distribute* a copy. Run a modified
version on your own server and give people access over a network, and you never hand out a
copy — so you never have to publish anything. The AGPL closes that gap. **Section 13 says
that letting users interact with a modified version remotely over a network counts, and you
must offer those users its source code.**

For Awaitick today this mostly matters for the WebAssembly build. **There is currently no
Awaitick backend server.** If one is ever added, section 13 is what makes sure a modified,
self-hosted copy of it stays open — which is exactly why the licence is AGPL rather than
GPL, even before that server exists.

If you only run Awaitick on your own device, section 13 asks nothing of you.

## The Apple App Store exception

Apple's App Store terms restrict what people may do with an app they download in ways that
sections 6 and 10 of the AGPL do not permit. Read strictly, an AGPL app cannot be on the
App Store at all.

[`COPYING.iOS`](COPYING.iOS) is an **additional permission under AGPLv3 section 7** that
resolves this. It allows Awaitick to be distributed through the Apple App Store and the Mac
App Store under Apple's terms — and that permission is granted not only for the official
builds, but to **anyone** shipping an unmodified or modified version through those stores.

Two things to note:

- **Every other AGPL obligation still applies.** You must still make your source available.
  The exception waives the conflict with Apple's distribution terms, nothing more.
- **You may drop the exception.** Section 7 lets anyone remove an additional permission from
  a copy they pass on. A fork shipped without `COPYING.iOS` is plain AGPLv3.

The exception is kept in its own file, not merged into `LICENSE`, so that `LICENSE` stays
byte-for-byte the FSF text and licence scanners keep identifying the project correctly.

## Trademarks

**The code is free. The name is not.** "Awaitick", the logo, the app icon and the brand
artwork are not covered by the AGPL — trademark and copyright are separate things, and a
free-software licence does not hand over a brand.

If you publish a fork, **rebrand it**. Full details and a checklist are in
[`TRADEMARKS.md`](TRADEMARKS.md).

## Contributing: the CLA

Contributions require signing a short [Contributor Licence Agreement](CLA.md). A bot will
ask you to sign it on your first pull request — one comment, once, and it remembers you.

**You keep the copyright in your own work.** The CLA grants a licence broad enough to keep
shipping Awaitick on app stores whose terms the AGPL alone cannot satisfy, and to relicense
if that ever becomes necessary. It takes a minute to read; please do read it rather than
signing blind.

## Third-party code and assets

Awaitick depends on other people's work, all of it under permissive licences compatible
with the AGPL. The full list, with licences, is in
[`THIRD-PARTY-NOTICES.md`](THIRD-PARTY-NOTICES.md), and the same information is shown inside
the app under **Settings → About → Open-source licenses**.

## Questions

Anything unclear, or a use case you are not sure about? Open a discussion or email
<martin@zikmund.dev>. Asking first is always cheaper than guessing.
