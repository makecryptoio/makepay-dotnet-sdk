using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MakePay;

public sealed class MakePayCreatePaymentLinkOptions
{
    public string Status { get; set; } = "active";

    public bool SendPaymentRequestEmail { get; set; }
}

public sealed class MakePayPaymentLinkPayload
{
    public string? Title { get; set; }

    public string? Label { get; set; }

    public string? Description { get; set; }

    public string? Amount { get; set; }

    public string? Currency { get; set; }

    public string? Asset { get; set; }

    public string? OrderId { get; set; }

    public string? MerchantOrderId { get; set; }

    public string? CustomerEmail { get; set; }

    public string? ReceiptEmail { get; set; }

    public string? ClientId { get; set; }

    public string? ReturnUrl { get; set; }

    public string? SuccessUrl { get; set; }

    public string? FailureUrl { get; set; }

    public string? ReturnRedirectUrl { get; set; }

    public string? SuccessRedirectUrl { get; set; }

    public string? FailureRedirectUrl { get; set; }

    public string? ExpirationTime { get; set; }

    public IDictionary<string, object?>? Metadata { get; set; }

    [JsonExtensionData]
    public IDictionary<string, object?> AdditionalProperties { get; } = new Dictionary<string, object?>();
}

public sealed class MakePayPaymentLinkStatusUpdate
{
    public string Status { get; set; } = "active";
}

public sealed class MakePayCustomerPayload
{
    public string? Email { get; set; }

    public string? CustomerEmail { get; set; }

    public string? Name { get; set; }

    public string? ClientId { get; set; }

    public IDictionary<string, object?>? Metadata { get; set; }
}

public sealed class MakePaySubscriptionPayload
{
    public string? AmountUsd { get; set; }

    public string? CustomerEmail { get; set; }

    public string? Label { get; set; }

    public string? Description { get; set; }

    public string? BillingIntervalUnit { get; set; }

    public int? BillingIntervalCount { get; set; }

    public string? StartAt { get; set; }

    public bool? SendPaymentRequestEmail { get; set; }

    public string? ClientId { get; set; }
}

public sealed class MakePayPosTerminalPayload
{
    public string? Name { get; set; }

    public string? Pin { get; set; }

    public string? Status { get; set; }

    public string[]? AllowedAssets { get; set; }

    public string? EmailCollectionMode { get; set; }

    public bool? CatalogEnabled { get; set; }

    public IDictionary<string, object?>? DisplaySettings { get; set; }

    public IDictionary<string, object?>? Metadata { get; set; }
}

public sealed class MakePayBookkeepingDocumentUpload
{
    public System.IO.Stream? File { get; set; }

    public string FileName { get; set; } = "document";

    public string? DocumentType { get; set; }

    public string? InvoiceId { get; set; }

    public string? ExpenseId { get; set; }
}
