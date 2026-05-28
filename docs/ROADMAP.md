# MakePay .NET SDK Roadmap

## Purpose

This repository is priority rank 2 in the MakePay integration roadmap. It fills
the major enterprise and backend gap for C# and .NET teams.

## Current Scope

- Authenticated partner API client.
- Payment links and donation links.
- Customers and customer portals.
- Subscriptions.
- Destination assets and webhook request logs.
- POS terminals.
- Products and Simple Shop.
- Branding and settings.
- Bookkeeping invoices, expenses, documents, OCR, and reconciliation links.
- Hosted checkout, embedded checkout, donation, modal script, and iframe URL
  helpers.
- HMAC-SHA256 webhook verification.

## Near-Term Milestones

1. Publish repository and protect `main`.
2. Fix any GitHub Actions build or test issues.
3. Add generated typed response models from `makepay-openapi`.
4. Publish prerelease NuGet package.
5. Add ASP.NET Core starter sample as the next repository in the roadmap.

## Acceptance Criteria

- GitHub Actions runs `dotnet test`.
- GitHub Actions runs `dotnet pack`.
- Webhook verification tests cover valid, tampered, malformed, and stale
  signatures.
- API key secrets never appear in client-side examples.
- Repository protection requires the validation workflow before merging.
