using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace MakePay;

public sealed class MakePayClient
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient httpClient;
    private readonly MakePayClientOptions options;

    public MakePayClient(HttpClient httpClient, MakePayClientOptions options)
    {
        if (httpClient == null)
        {
            throw new ArgumentNullException(nameof(httpClient));
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (string.IsNullOrWhiteSpace(options.KeyId))
        {
            throw new MakePayException("MakePay KeyId is required.");
        }

        if (string.IsNullOrWhiteSpace(options.KeySecret))
        {
            throw new MakePayException("MakePay KeySecret is required.");
        }

        this.httpClient = httpClient;
        this.options = options;
    }

    public Task<JsonDocument> CreatePaymentLinkAsync(
        MakePayPaymentLinkPayload payload,
        MakePayCreatePaymentLinkOptions? createOptions = null,
        CancellationToken cancellationToken = default)
    {
        if (payload == null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        createOptions ??= new MakePayCreatePaymentLinkOptions();
        return SendJsonAsync(
            HttpMethod.Post,
            "/api/partner/v1/makepay/payment-links",
            new
            {
                status = createOptions.Status,
                sendPaymentRequestEmail = createOptions.SendPaymentRequestEmail,
                payload
            },
            null,
            cancellationToken);
    }

    public Task<JsonDocument> ListPaymentLinksAsync(
        IReadOnlyDictionary<string, string?>? query = null,
        CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, "/api/partner/v1/makepay/payment-links", null, query, cancellationToken);
    }

    public Task<JsonDocument> GetPaymentLinkAsync(string uid, CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, $"/api/partner/v1/makepay/payment-links/{Escape(uid, nameof(uid))}", null, null, cancellationToken);
    }

    public Task<JsonDocument> UpdatePaymentLinkAsync(
        string uid,
        MakePayPaymentLinkStatusUpdate update,
        CancellationToken cancellationToken = default)
    {
        if (update == null)
        {
            throw new ArgumentNullException(nameof(update));
        }

        return SendJsonAsync(Patch, $"/api/partner/v1/makepay/payment-links/{Escape(uid, nameof(uid))}", update, null, cancellationToken);
    }

    public Task<JsonDocument> SendPaymentRequestEmailAsync(
        string uid,
        string? email = null,
        CancellationToken cancellationToken = default)
    {
        object body = email == null ? new { } : new { email };
        return SendJsonAsync(HttpMethod.Post, $"/api/partner/v1/makepay/payment-links/{Escape(uid, nameof(uid))}/send-request-email", body, null, cancellationToken);
    }

    public Task<JsonDocument> CreateDonationLinkAsync(
        MakePayPaymentLinkPayload payload,
        MakePayCreatePaymentLinkOptions? createOptions = null,
        CancellationToken cancellationToken = default)
    {
        if (payload == null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        createOptions ??= new MakePayCreatePaymentLinkOptions();
        payload.AdditionalProperties["type"] = "donation";
        return SendJsonAsync(
            HttpMethod.Post,
            "/api/partner/v1/makepay/donations",
            new
            {
                status = createOptions.Status,
                sendPaymentRequestEmail = createOptions.SendPaymentRequestEmail,
                payload
            },
            null,
            cancellationToken);
    }

    public Task<JsonDocument> ListDonationLinksAsync(CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, "/api/partner/v1/makepay/donations", null, null, cancellationToken);
    }

    public Task<JsonDocument> GetDonationLinkAsync(string uid, CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, $"/api/partner/v1/makepay/donations/{Escape(uid, nameof(uid))}", null, null, cancellationToken);
    }

    public Task<JsonDocument> UpdateDonationLinkAsync(
        string uid,
        MakePayPaymentLinkStatusUpdate update,
        CancellationToken cancellationToken = default)
    {
        if (update == null)
        {
            throw new ArgumentNullException(nameof(update));
        }

        return SendJsonAsync(Patch, $"/api/partner/v1/makepay/donations/{Escape(uid, nameof(uid))}", update, null, cancellationToken);
    }

    public Task<JsonDocument> ListCustomersAsync(CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, "/api/partner/v1/makepay/customers", null, null, cancellationToken);
    }

    public Task<JsonDocument> UpsertCustomerAsync(MakePayCustomerPayload payload, CancellationToken cancellationToken = default)
    {
        if (payload == null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        return SendJsonAsync(HttpMethod.Post, "/api/partner/v1/makepay/customers", payload, null, cancellationToken);
    }

    public Task<JsonDocument> CreateCustomerPortalAsync(
        string customerId,
        object? payload = null,
        CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Post, $"/api/partner/v1/makepay/customers/{Escape(customerId, nameof(customerId))}/portal", payload ?? new { }, null, cancellationToken);
    }

    public Task<JsonDocument> ListSubscriptionsAsync(CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, "/api/partner/v1/makepay/subscriptions", null, null, cancellationToken);
    }

    public Task<JsonDocument> CreateSubscriptionAsync(MakePaySubscriptionPayload payload, CancellationToken cancellationToken = default)
    {
        if (payload == null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        return SendJsonAsync(HttpMethod.Post, "/api/partner/v1/makepay/subscriptions", payload, null, cancellationToken);
    }

    public Task<JsonDocument> ListDestinationAssetsAsync(CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, "/api/partner/v1/makepay/destination-assets", null, null, cancellationToken);
    }

    public Task<JsonDocument> ListWebhookRequestsAsync(
        IReadOnlyDictionary<string, string?>? query = null,
        CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, "/api/partner/v1/makepay/webhook-requests", null, query, cancellationToken);
    }

    public Task<JsonDocument> ListPosTerminalsAsync(CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, "/api/partner/v1/makepay/pos-terminals", null, null, cancellationToken);
    }

    public Task<JsonDocument> CreatePosTerminalAsync(MakePayPosTerminalPayload payload, CancellationToken cancellationToken = default)
    {
        if (payload == null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        return SendJsonAsync(HttpMethod.Post, "/api/partner/v1/makepay/pos-terminals", payload, null, cancellationToken);
    }

    public Task<JsonDocument> GetPosTerminalAsync(string terminalId, CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, $"/api/partner/v1/makepay/pos-terminals/{Escape(terminalId, nameof(terminalId))}", null, null, cancellationToken);
    }

    public Task<JsonDocument> UpdatePosTerminalAsync(string terminalId, MakePayPosTerminalPayload payload, CancellationToken cancellationToken = default)
    {
        if (payload == null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        return SendJsonAsync(Patch, $"/api/partner/v1/makepay/pos-terminals/{Escape(terminalId, nameof(terminalId))}", payload, null, cancellationToken);
    }

    public Task<JsonDocument> ListProductsAsync(CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, "/api/partner/v1/makepay/products", null, null, cancellationToken);
    }

    public Task<JsonDocument> CreateProductAsync(object payload, CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Post, "/api/partner/v1/makepay/products", RequiredPayload(payload), null, cancellationToken);
    }

    public Task<JsonDocument> GetProductAsync(string productId, CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, $"/api/partner/v1/makepay/products/{Escape(productId, nameof(productId))}", null, null, cancellationToken);
    }

    public Task<JsonDocument> UpdateProductAsync(string productId, object payload, CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(Patch, $"/api/partner/v1/makepay/products/{Escape(productId, nameof(productId))}", RequiredPayload(payload), null, cancellationToken);
    }

    public Task<JsonDocument> GetShopAsync(CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, "/api/partner/v1/makepay/shop", null, null, cancellationToken);
    }

    public Task<JsonDocument> UpdateShopAsync(object payload, CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(Patch, "/api/partner/v1/makepay/shop", RequiredPayload(payload), null, cancellationToken);
    }

    public Task<JsonDocument> GetShopBuilderAsync(CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, "/api/partner/v1/makepay/shop/builder", null, null, cancellationToken);
    }

    public Task<JsonDocument> UpdateShopBuilderAsync(object payload, CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Put, "/api/partner/v1/makepay/shop/builder", RequiredPayload(payload), null, cancellationToken);
    }

    public Task<JsonDocument> GetShopDomainAsync(CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, "/api/partner/v1/makepay/shop/domains", null, null, cancellationToken);
    }

    public Task<JsonDocument> UpdateShopDomainAsync(string? domain, CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Put, "/api/partner/v1/makepay/shop/domains", new { domain }, null, cancellationToken);
    }

    public Task<JsonDocument> RefreshShopDomainAsync(string? domain = null, CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Post, "/api/partner/v1/makepay/shop/domains", domain == null ? new { } : new { domain }, null, cancellationToken);
    }

    public Task<JsonDocument> ListShopOrdersAsync(
        IReadOnlyDictionary<string, string?>? query = null,
        CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, "/api/partner/v1/makepay/shop/orders", null, query, cancellationToken);
    }

    public Task<JsonDocument> GetBrandingAsync(CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, "/api/partner/v1/makepay/branding", null, null, cancellationToken);
    }

    public Task<JsonDocument> UpdateBrandingAsync(object payload, CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(Patch, "/api/partner/v1/makepay/branding", RequiredPayload(payload), null, cancellationToken);
    }

    public Task<JsonDocument> RefreshBrandingDomainsAsync(string kind = "all", CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Post, "/api/partner/v1/makepay/branding/domains/refresh", new { kind }, null, cancellationToken);
    }

    public Task<JsonDocument> GetSettingsAsync(CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, "/api/partner/v1/makepay/settings", null, null, cancellationToken);
    }

    public Task<JsonDocument> UpdateSettingsAsync(object payload, CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Put, "/api/partner/v1/makepay/settings", RequiredPayload(payload), null, cancellationToken);
    }

    public Task<JsonDocument> GetBookkeepingSummaryAsync(CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, "/api/partner/v1/makepay/bookkeeping", null, null, cancellationToken);
    }

    public Task<JsonDocument> ListBookkeepingInvoicesAsync(CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, "/api/partner/v1/makepay/bookkeeping/invoices", null, null, cancellationToken);
    }

    public Task<JsonDocument> CreateBookkeepingInvoiceAsync(object payload, CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Post, "/api/partner/v1/makepay/bookkeeping/invoices", RequiredPayload(payload), null, cancellationToken);
    }

    public Task<JsonDocument> GetBookkeepingInvoiceAsync(string invoiceId, CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, $"/api/partner/v1/makepay/bookkeeping/invoices/{Escape(invoiceId, nameof(invoiceId))}", null, null, cancellationToken);
    }

    public Task<JsonDocument> UpdateBookkeepingInvoiceAsync(string invoiceId, object payload, CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(Patch, $"/api/partner/v1/makepay/bookkeeping/invoices/{Escape(invoiceId, nameof(invoiceId))}", RequiredPayload(payload), null, cancellationToken);
    }

    public Task<JsonDocument> CreateBookkeepingInvoicePaymentLinkAsync(
        string invoiceId,
        bool sendPaymentRequestEmail = false,
        CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(
            HttpMethod.Post,
            $"/api/partner/v1/makepay/bookkeeping/invoices/{Escape(invoiceId, nameof(invoiceId))}/payment-link",
            new { sendPaymentRequestEmail },
            null,
            cancellationToken);
    }

    public Task<JsonDocument> ListBookkeepingExpensesAsync(CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, "/api/partner/v1/makepay/bookkeeping/expenses", null, null, cancellationToken);
    }

    public Task<JsonDocument> CreateBookkeepingExpenseAsync(object payload, CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Post, "/api/partner/v1/makepay/bookkeeping/expenses", RequiredPayload(payload), null, cancellationToken);
    }

    public Task<JsonDocument> CreateBookkeepingExpenseFromActivityAsync(object payload, CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Post, "/api/partner/v1/makepay/bookkeeping/expenses/from-activity", RequiredPayload(payload), null, cancellationToken);
    }

    public Task<JsonDocument> GetBookkeepingExpenseAsync(string expenseId, CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, $"/api/partner/v1/makepay/bookkeeping/expenses/{Escape(expenseId, nameof(expenseId))}", null, null, cancellationToken);
    }

    public Task<JsonDocument> UpdateBookkeepingExpenseAsync(string expenseId, object payload, CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(Patch, $"/api/partner/v1/makepay/bookkeeping/expenses/{Escape(expenseId, nameof(expenseId))}", RequiredPayload(payload), null, cancellationToken);
    }

    public Task<JsonDocument> ListBookkeepingDocumentsAsync(CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, "/api/partner/v1/makepay/bookkeeping/documents", null, null, cancellationToken);
    }

    public async Task<JsonDocument> UploadBookkeepingDocumentAsync(
        MakePayBookkeepingDocumentUpload upload,
        CancellationToken cancellationToken = default)
    {
        if (upload == null)
        {
            throw new ArgumentNullException(nameof(upload));
        }

        if (upload.File == null)
        {
            throw new MakePayException("A bookkeeping document stream is required.");
        }

        using var content = new MultipartFormDataContent();
        content.Add(new StreamContent(upload.File), "file", upload.FileName);
        AddFormString(content, "documentType", upload.DocumentType);
        AddFormString(content, "invoiceId", upload.InvoiceId);
        AddFormString(content, "expenseId", upload.ExpenseId);

        return await SendAsync(HttpMethod.Post, "/api/partner/v1/makepay/bookkeeping/documents", content, null, cancellationToken).ConfigureAwait(false);
    }

    public Task<JsonDocument> GetBookkeepingDocumentDownloadUrlAsync(string documentId, CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Get, $"/api/partner/v1/makepay/bookkeeping/documents/{Escape(documentId, nameof(documentId))}/download", null, null, cancellationToken);
    }

    public Task<JsonDocument> RunBookkeepingDocumentOcrAsync(string documentId, CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Post, $"/api/partner/v1/makepay/bookkeeping/documents/{Escape(documentId, nameof(documentId))}/ocr", new { }, null, cancellationToken);
    }

    public Task<JsonDocument> CreateBookkeepingReconciliationAsync(object payload, CancellationToken cancellationToken = default)
    {
        return SendJsonAsync(HttpMethod.Post, "/api/partner/v1/makepay/bookkeeping/reconciliation", RequiredPayload(payload), null, cancellationToken);
    }

    public string BuildHostedCheckoutUrl(string paymentUid)
    {
        return BuildCheckoutUrl($"/payment/{Escape(paymentUid, nameof(paymentUid))}");
    }

    public string BuildHostedDonationUrl(string donationSlug)
    {
        return BuildCheckoutUrl($"/donations/{Escape(donationSlug, nameof(donationSlug))}");
    }

    public string BuildEmbeddedCheckoutUrl(string paymentUid, string? parentOrigin = null)
    {
        var query = parentOrigin == null ? null : new Dictionary<string, string?> { ["parentOrigin"] = parentOrigin };
        return BuildCheckoutUrl($"/embed/payment/{Escape(paymentUid, nameof(paymentUid))}", query);
    }

    public string BuildEmbeddedDonationUrl(string donationSlug, string? parentOrigin = null)
    {
        var query = parentOrigin == null ? null : new Dictionary<string, string?> { ["parentOrigin"] = parentOrigin };
        return BuildCheckoutUrl($"/embed/donations/{Escape(donationSlug, nameof(donationSlug))}", query);
    }

    public string BuildModalScriptUrl()
    {
        return BuildCheckoutUrl("/modal/makepay.js");
    }

    private async Task<JsonDocument> SendJsonAsync(
        HttpMethod method,
        string path,
        object? body,
        IReadOnlyDictionary<string, string?>? query,
        CancellationToken cancellationToken)
    {
        HttpContent? content = null;
        if (body != null && method != HttpMethod.Get)
        {
            var json = JsonSerializer.Serialize(body, JsonOptions);
            content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        return await SendAsync(method, path, content, query, cancellationToken).ConfigureAwait(false);
    }

    private async Task<JsonDocument> SendAsync(
        HttpMethod method,
        string path,
        HttpContent? content,
        IReadOnlyDictionary<string, string?>? query,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, BuildPartnerUri(path, query));
        request.Headers.TryAddWithoutValidation("Accept", "application/json");
        request.Headers.TryAddWithoutValidation("User-Agent", options.UserAgent);
        request.Headers.TryAddWithoutValidation("X-MakeCrypto-Key-Id", options.KeyId);
        request.Headers.TryAddWithoutValidation("X-MakeCrypto-Key-Secret", options.KeySecret);
        request.Content = content;

        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseBody = response.Content == null
            ? string.Empty
            : await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw new MakePayException($"MakePay API request failed with {(int)response.StatusCode}.", response.StatusCode, responseBody);
        }

        return string.IsNullOrWhiteSpace(responseBody)
            ? JsonDocument.Parse("{}")
            : JsonDocument.Parse(responseBody);
    }

    private Uri BuildPartnerUri(string path, IReadOnlyDictionary<string, string?>? query)
    {
        return BuildUri(options.BaseUrl, path, query);
    }

    private string BuildCheckoutUrl(string path, IReadOnlyDictionary<string, string?>? query = null)
    {
        return BuildUri(options.CheckoutBaseUrl, path, query).ToString();
    }

    private static Uri BuildUri(Uri baseUri, string path, IReadOnlyDictionary<string, string?>? query)
    {
        var normalizedBase = baseUri.ToString().TrimEnd('/') + "/";
        var builder = new UriBuilder(new Uri(new Uri(normalizedBase), path.TrimStart('/')));

        if (query != null && query.Count > 0)
        {
            var parts = new List<string>();
            foreach (var item in query)
            {
                if (item.Value == null)
                {
                    continue;
                }

                parts.Add(Uri.EscapeDataString(item.Key) + "=" + Uri.EscapeDataString(item.Value));
            }

            builder.Query = string.Join("&", parts);
        }

        return builder.Uri;
    }

    private static string Escape(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return Uri.EscapeDataString(value);
    }

    private static object RequiredPayload(object payload)
    {
        if (payload == null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        return payload;
    }

    private static void AddFormString(MultipartFormDataContent content, string name, string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            content.Add(new StringContent(value), name);
        }
    }

    private static readonly HttpMethod Patch = new HttpMethod("PATCH");
}
