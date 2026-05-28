# MakePay .NET SDK

Official .NET SDK for MakePay server-side integrations.

Use it to create payment links, send checkout emails, manage customers and
subscriptions, build hosted checkout URLs, work with MakePay bookkeeping, and
verify signed webhooks.

## Installation

The package is planned for NuGet as:

```bash
dotnet add package MakePay
```

Until the first NuGet release, reference the project or package generated from
this repository.

## Quick Start

```csharp
using MakePay;

var httpClient = new HttpClient();
var makePay = new MakePayClient(
    httpClient,
    new MakePayClientOptions
    {
        KeyId = Environment.GetEnvironmentVariable("MAKEPAY_KEY_ID")!,
        KeySecret = Environment.GetEnvironmentVariable("MAKEPAY_KEY_SECRET")!
    });

using var created = await makePay.CreatePaymentLinkAsync(
    new MakePayPaymentLinkPayload
    {
        Title = "Order #1042",
        Description = "Hosted checkout for order #1042",
        Amount = "129.99",
        Currency = "USDT",
        OrderId = "order_1042",
        CustomerEmail = "buyer@example.com",
        ReturnUrl = "https://merchant.example/orders/1042",
        SuccessUrl = "https://merchant.example/orders/1042/success",
        FailureUrl = "https://merchant.example/orders/1042/pay"
    });

Console.WriteLine(created.RootElement.GetProperty("paymentLink").GetProperty("publicUrl"));
```

## Webhook Verification

Read the exact raw body before parsing JSON.

```csharp
using var memory = new MemoryStream();
await Request.Body.CopyToAsync(memory);
var rawBody = memory.ToArray();
var signature = Request.Headers["X-MakePay-Signature"].ToString();

var isTrusted = MakePayWebhookVerifier.Verify(
    rawBody,
    signature,
    Environment.GetEnvironmentVariable("MAKEPAY_WEBHOOK_SECRET")!);

if (!isTrusted)
{
    return Results.Unauthorized();
}
```

## Checkout URLs

```csharp
var hosted = makePay.BuildHostedCheckoutUrl("01jsg7w9n8pz8l4x0dn26v2aqd");
var embedded = makePay.BuildEmbeddedCheckoutUrl(
    "01jsg7w9n8pz8l4x0dn26v2aqd",
    "https://merchant.example");
```

## Validation

```bash
dotnet test
dotnet pack src/MakePay/MakePay.csproj -c Release --no-build
```

Maintainer: Ethan Carter (`makepayio`).
