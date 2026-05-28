using System.Security.Cryptography;
using System.Text;
using MakePay;

namespace MakePay.Tests;

public sealed class MakePayWebhookVerifierTests
{
    [Fact]
    public void VerifyAcceptsValidSignature()
    {
        var now = DateTimeOffset.FromUnixTimeSeconds(1_779_997_509);
        var body = Encoding.UTF8.GetBytes("{\"deliveryId\":\"00000000-0000-0000-0000-000000000000\"}");
        var header = BuildHeader(body, "secret", now);

        Assert.True(MakePayWebhookVerifier.Verify(body, header, "secret", now: now));
    }

    [Fact]
    public void VerifyRejectsTamperedBody()
    {
        var now = DateTimeOffset.FromUnixTimeSeconds(1_779_997_509);
        var body = Encoding.UTF8.GetBytes("{\"ok\":true}");
        var header = BuildHeader(body, "secret", now);
        var tampered = Encoding.UTF8.GetBytes("{\"ok\":false}");

        Assert.False(MakePayWebhookVerifier.Verify(tampered, header, "secret", now: now));
    }

    [Fact]
    public void VerifyRejectsStaleTimestamp()
    {
        var signedAt = DateTimeOffset.FromUnixTimeSeconds(1_779_997_509);
        var body = Encoding.UTF8.GetBytes("{\"ok\":true}");
        var header = BuildHeader(body, "secret", signedAt);

        Assert.False(MakePayWebhookVerifier.Verify(body, header, "secret", now: signedAt.AddMinutes(6)));
    }

    private static string BuildHeader(byte[] body, string secret, DateTimeOffset timestamp)
    {
        var timestampText = timestamp.ToUnixTimeSeconds().ToString();
        var prefix = Encoding.UTF8.GetBytes(timestampText + ".");
        var payload = new byte[prefix.Length + body.Length];
        Buffer.BlockCopy(prefix, 0, payload, 0, prefix.Length);
        Buffer.BlockCopy(body, 0, payload, prefix.Length, body.Length);

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var digest = hmac.ComputeHash(payload);
        return "t=" + timestampText + ",v1=" + ToHex(digest);
    }

    private static string ToHex(byte[] bytes)
    {
        var builder = new StringBuilder(bytes.Length * 2);
        foreach (var value in bytes)
        {
            builder.Append(value.ToString("x2"));
        }

        return builder.ToString();
    }
}
