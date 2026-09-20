# Security policy

## Reporting a vulnerability

**Please do not open a public issue for security problems.**

Report privately through GitHub's private vulnerability reporting:
[**Report a vulnerability**](https://github.com/MartinZikmund/Awaitick/security/advisories/new).
That opens a discussion visible only to you and the maintainer.

If you cannot use GitHub, email <martin@zikmund.dev> instead.

## What to include

Whatever you have — a rough report beats no report:

- What the problem is and what an attacker could do with it
- Steps to reproduce, or a proof of concept
- Affected platform (Windows, Android, iOS, desktop, web) and app version
- Anything you think should be handled carefully

## What happens next

Awaitick is maintained by one person in his spare time, so these are honest intentions
rather than a commercial SLA:

| Stage | Timeframe |
|---|---|
| Acknowledgement | within 5 days |
| Initial assessment | within 14 days |
| Fix and release | depends on severity and store review times |

Fixes ship through the app stores, so the last step is partly out of the maintainer's hands.

You will be credited in the advisory unless you would rather not be. Please give a
reasonable amount of time for a fix to reach users before disclosing publicly.

## Supported versions

Only the **latest released version** is supported. Store builds update automatically; a fix
means a new release rather than a patch to an older one.

## Scope

In scope: the Awaitick app and this repository.

Out of scope: vulnerabilities in the operating system, in Uno Platform or other
dependencies (report those upstream — tell us too if Awaitick is affected), and issues that
require an already-compromised device.
