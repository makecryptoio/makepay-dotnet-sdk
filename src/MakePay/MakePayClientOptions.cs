using System;

namespace MakePay;

public sealed class MakePayClientOptions
{
    public Uri BaseUrl { get; set; } = new Uri("https://www.makecrypto.io");

    public Uri CheckoutBaseUrl { get; set; } = new Uri("https://makepay.io");

    public string KeyId { get; set; } = string.Empty;

    public string KeySecret { get; set; } = string.Empty;

    public string UserAgent { get; set; } = "MakePayDotNet/0.1.0";
}
