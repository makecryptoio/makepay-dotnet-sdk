using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MakePay;

public static class MakePayWebhookVerifier
{
    public static bool Verify(
        byte[] rawBody,
        string signatureHeader,
        string webhookSecret,
        TimeSpan? tolerance = null,
        DateTimeOffset? now = null)
    {
        if (rawBody == null)
        {
            throw new ArgumentNullException(nameof(rawBody));
        }

        if (string.IsNullOrWhiteSpace(signatureHeader) || string.IsNullOrEmpty(webhookSecret))
        {
            return false;
        }

        var parts = ParseSignatureHeader(signatureHeader);
        if (!parts.TryGetValue("t", out var timestampText) ||
            !parts.TryGetValue("v1", out var signatureHex) ||
            !long.TryParse(timestampText, out var timestamp))
        {
            return false;
        }

        var current = now ?? DateTimeOffset.UtcNow;
        var signedAt = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        var allowedSkew = tolerance ?? TimeSpan.FromMinutes(5);
        if (current - signedAt > allowedSkew || signedAt - current > allowedSkew)
        {
            return false;
        }

        var actual = TryDecodeHex(signatureHex);
        if (actual == null)
        {
            return false;
        }

        var prefix = Encoding.UTF8.GetBytes(timestampText + ".");
        var signedPayload = new byte[prefix.Length + rawBody.Length];
        Buffer.BlockCopy(prefix, 0, signedPayload, 0, prefix.Length);
        Buffer.BlockCopy(rawBody, 0, signedPayload, prefix.Length, rawBody.Length);

        byte[] expected;
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(webhookSecret)))
        {
            expected = hmac.ComputeHash(signedPayload);
        }

        return FixedTimeEquals(expected, actual);
    }

    public static JsonDocument ParseVerified(
        byte[] rawBody,
        string signatureHeader,
        string webhookSecret,
        TimeSpan? tolerance = null,
        DateTimeOffset? now = null)
    {
        if (!Verify(rawBody, signatureHeader, webhookSecret, tolerance, now))
        {
            throw new MakePayException("Invalid MakePay webhook signature.");
        }

        return JsonDocument.Parse(rawBody);
    }

    private static Dictionary<string, string> ParseSignatureHeader(string header)
    {
        var parts = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var segment in header.Split(','))
        {
            var trimmed = segment.Trim();
            var separator = trimmed.IndexOf('=');
            if (separator <= 0 || separator == trimmed.Length - 1)
            {
                continue;
            }

            parts[trimmed.Substring(0, separator)] = trimmed.Substring(separator + 1);
        }

        return parts;
    }

    private static byte[]? TryDecodeHex(string value)
    {
        if (value.Length == 0 || value.Length % 2 != 0)
        {
            return null;
        }

        var bytes = new byte[value.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            var high = FromHex(value[i * 2]);
            var low = FromHex(value[(i * 2) + 1]);
            if (high < 0 || low < 0)
            {
                return null;
            }

            bytes[i] = (byte)((high << 4) | low);
        }

        return bytes;
    }

    private static int FromHex(char c)
    {
        if (c >= '0' && c <= '9')
        {
            return c - '0';
        }

        if (c >= 'a' && c <= 'f')
        {
            return c - 'a' + 10;
        }

        if (c >= 'A' && c <= 'F')
        {
            return c - 'A' + 10;
        }

        return -1;
    }

    private static bool FixedTimeEquals(byte[] expected, byte[] actual)
    {
        if (expected.Length != actual.Length)
        {
            return false;
        }

        var diff = 0;
        for (var i = 0; i < expected.Length; i++)
        {
            diff |= expected[i] ^ actual[i];
        }

        return diff == 0;
    }
}
