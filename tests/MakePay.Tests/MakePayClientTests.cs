using System.Net;
using System.Text.Json;
using MakePay;
using Xunit;

namespace MakePay.Tests;

public sealed class MakePayClientTests
{
    [Fact]
    public async Task CreatePaymentLinkSendsAuthHeadersAndPayload()
    {
        HttpRequestMessage? capturedRequest = null;
        var handler = new StubHandler(request =>
        {
            capturedRequest = request;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"paymentLink\":{\"uid\":\"pay_123\",\"publicUrl\":\"https://makepay.io/payment/pay_123\"}}")
            };
        });

        using var httpClient = new HttpClient(handler);
        var client = new MakePayClient(
            httpClient,
            new MakePayClientOptions
            {
                KeyId = "key_id",
                KeySecret = "key_secret"
            });

        using var response = await client.CreatePaymentLinkAsync(
            new MakePayPaymentLinkPayload
            {
                Title = "Order #1042",
                Amount = "10.00",
                Currency = "USDT",
                OrderId = "order_1042"
            });

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest!.Method);
        Assert.Equal("https://www.makecrypto.io/api/partner/v1/makepay/payment-links", capturedRequest.RequestUri!.ToString());
        Assert.True(capturedRequest.Headers.TryGetValues("X-MakeCrypto-Key-Id", out var keyIds));
        Assert.Equal("key_id", keyIds.Single());
        Assert.True(capturedRequest.Headers.TryGetValues("X-MakeCrypto-Key-Secret", out var keySecrets));
        Assert.Equal("key_secret", keySecrets.Single());

        var requestBody = await capturedRequest.Content!.ReadAsStringAsync();
        using var json = JsonDocument.Parse(requestBody);
        Assert.Equal("active", json.RootElement.GetProperty("status").GetString());
        Assert.Equal("10.00", json.RootElement.GetProperty("payload").GetProperty("amount").GetString());
        Assert.Equal("pay_123", response.RootElement.GetProperty("paymentLink").GetProperty("uid").GetString());
    }

    [Fact]
    public void CheckoutUrlHelpersBuildExpectedUrls()
    {
        using var httpClient = new HttpClient(new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)));
        var client = new MakePayClient(
            httpClient,
            new MakePayClientOptions
            {
                KeyId = "key_id",
                KeySecret = "key_secret"
            });

        Assert.Equal("https://makepay.io/payment/pay_123", client.BuildHostedCheckoutUrl("pay_123"));
        Assert.Equal(
            "https://makepay.io/embed/payment/pay_123?parentOrigin=https%3A%2F%2Fmerchant.example",
            client.BuildEmbeddedCheckoutUrl("pay_123", "https://merchant.example"));
        Assert.Equal("https://makepay.io/modal/makepay.js", client.BuildModalScriptUrl());
    }

    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> responder;

        public StubHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
        {
            this.responder = responder;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(responder(request));
        }
    }
}
