// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Shopify webhook signature check: X-Shopify-Hmac-Sha256 is base64(HMAC-SHA256(app secret, raw request body)).
// Verify the RAW bytes as received (never a re-serialized body) and compare in constant time.

using System;
using System.Security.Cryptography;
using System.Text;

namespace AnointedAutomation.Shopify.Webhooks
{
    /// <summary>Verifies the HMAC signature Shopify puts on every webhook delivery.</summary>
    public static class ShopifyWebhookHmac
    {
        /// <summary>The header carrying the signature.</summary>
        public const string HeaderName = "X-Shopify-Hmac-Sha256";

        /// <summary>base64(HMAC-SHA256(secret, body)).</summary>
        public static string Compute(byte[] body, string secret)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }
            RequireSecret(secret);
            return Convert.ToBase64String(HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), body));
        }

        /// <summary>True only when <paramref name="hmacHeader"/> is the signature of <paramref name="body"/> under
        /// <paramref name="secret"/>. A missing, blank or non-base64 header is false. The comparison is constant time.
        /// A missing secret throws (a configuration error must never read as "unsigned").</summary>
        public static bool Verify(byte[] body, string hmacHeader, string secret)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }
            RequireSecret(secret);
            if (string.IsNullOrWhiteSpace(hmacHeader))
            {
                return false;
            }
            string trimmed = hmacHeader.Trim();
            byte[] given = new byte[(trimmed.Length * 3 / 4) + 3];
            if (!Convert.TryFromBase64String(trimmed, given, out int written))
            {
                return false;
            }
            byte[] expected = HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), body);
            return written == expected.Length && CryptographicOperations.FixedTimeEquals(expected, given.AsSpan(0, written));
        }

        /// <summary><see cref="Verify(byte[], string, string)"/> over the UTF-8 bytes of <paramref name="body"/>. Prefer
        /// the byte overload with the raw request bytes.</summary>
        public static bool Verify(string body, string hmacHeader, string secret)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }
            return Verify(Encoding.UTF8.GetBytes(body), hmacHeader, secret);
        }

        private static void RequireSecret(string secret)
        {
            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentException("The webhook signing secret is required.", nameof(secret));
            }
        }
    }
}
