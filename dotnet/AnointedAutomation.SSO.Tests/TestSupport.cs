// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using AnointedAutomation.SSO;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AnointedAutomation.SSO.Tests
{
    /// <summary>A clock the tests move by hand.</summary>
    internal sealed class MutableTimeProvider : TimeProvider
    {
        /// <summary>Creates the clock at a fixed instant.</summary>
        /// <param name="now">The starting time.</param>
        public MutableTimeProvider(DateTimeOffset now)
        {
            Now = now;
        }

        /// <summary>The current time.</summary>
        public DateTimeOffset Now { get; set; }

        /// <summary>Returns <see cref="Now"/>.</summary>
        /// <returns>The current time.</returns>
        public override DateTimeOffset GetUtcNow()
        {
            return Now;
        }

        /// <summary>Moves the clock forward.</summary>
        /// <param name="by">How far.</param>
        public void Advance(TimeSpan by)
        {
            Now = Now + by;
        }
    }

    /// <summary>A configuration manager that serves fixed keys and records refresh requests.</summary>
    internal sealed class FakeConfigurationManager : IConfigurationManager<OpenIdConnectConfiguration>
    {
        private OpenIdConnectConfiguration _current;

        /// <summary>Creates the manager.</summary>
        /// <param name="current">The configuration served first.</param>
        /// <param name="afterRefresh">The configuration served after a refresh request, or null to keep the first.</param>
        public FakeConfigurationManager(OpenIdConnectConfiguration current, OpenIdConnectConfiguration afterRefresh)
        {
            _current = current;
            AfterRefresh = afterRefresh;
        }

        /// <summary>The configuration served after a refresh request.</summary>
        public OpenIdConnectConfiguration AfterRefresh { get; set; }

        /// <summary>How many times a refresh was requested.</summary>
        public int RefreshCount { get; private set; }

        /// <summary>When true, every read throws (keys unavailable).</summary>
        public bool Fail { get; set; }

        /// <summary>Serves the current configuration.</summary>
        /// <param name="cancel">Unused.</param>
        /// <returns>The configuration.</returns>
        public Task<OpenIdConnectConfiguration> GetConfigurationAsync(CancellationToken cancel)
        {
            if (Fail)
            {
                throw new InvalidOperationException("keys unavailable");
            }
            return Task.FromResult(_current);
        }

        /// <summary>Switches to <see cref="AfterRefresh"/> when set.</summary>
        public void RequestRefresh()
        {
            RefreshCount++;
            if (AfterRefresh != null)
            {
                _current = AfterRefresh;
            }
        }
    }

    /// <summary>RSA test keys and a Logout Token builder. Nothing here talks to the real provider.</summary>
    internal sealed class TestKeys
    {
        public const string Issuer = "https://issuer.test/";
        public const string ClientId = "aa_test123";

        /// <summary>Creates a key pair with the given key id.</summary>
        /// <param name="kid">The key id.</param>
        public TestKeys(string kid)
        {
            Kid = kid;
            Rsa = RSA.Create(2048);
            SecurityKey = new RsaSecurityKey(Rsa.ExportParameters(false)) { KeyId = kid };
        }

        /// <summary>The key id.</summary>
        public string Kid { get; }

        /// <summary>The private key.</summary>
        public RSA Rsa { get; }

        /// <summary>The public key as the provider would publish it.</summary>
        public SecurityKey SecurityKey { get; }

        /// <summary>A configuration holding only these keys.</summary>
        /// <param name="keys">The keys to publish.</param>
        /// <returns>The configuration.</returns>
        public static OpenIdConnectConfiguration ConfigurationWith(params TestKeys[] keys)
        {
            OpenIdConnectConfiguration configuration = new OpenIdConnectConfiguration { Issuer = Issuer };
            foreach (TestKeys key in keys)
            {
                configuration.SigningKeys.Add(key.SecurityKey);
            }
            return configuration;
        }

        /// <summary>A valid Logout Token payload issued at <paramref name="now"/>.</summary>
        /// <param name="now">The issue time.</param>
        /// <returns>A mutable claim set.</returns>
        public static Dictionary<string, object> ValidPayload(DateTimeOffset now)
        {
            return new Dictionary<string, object>
            {
                { "iss", Issuer },
                { "aud", ClientId },
                { "iat", now.ToUnixTimeSeconds() },
                { "exp", now.AddMinutes(2).ToUnixTimeSeconds() },
                { "jti", "jti-" + Guid.NewGuid().ToString("N") },
                { "sub", "user-1" },
                { "sid", "grant-1" },
                { "events", new Dictionary<string, object> { { AnointedAutomationDefaults.BackchannelLogoutEvent, new Dictionary<string, object>() } } },
            };
        }

        /// <summary>A valid Logout Token header for this key.</summary>
        /// <returns>A mutable header.</returns>
        public Dictionary<string, object> ValidHeader()
        {
            return new Dictionary<string, object>
            {
                { "alg", "RS256" },
                { "typ", AnointedAutomationDefaults.LogoutTokenType },
                { "kid", Kid },
            };
        }

        /// <summary>Signs a header and payload with RS256 using this key.</summary>
        /// <param name="header">The header.</param>
        /// <param name="payload">The payload.</param>
        /// <returns>A compact JWS.</returns>
        public string SignRs256(Dictionary<string, object> header, Dictionary<string, object> payload)
        {
            string signingInput = Encode(header) + "." + Encode(payload);
            byte[] signature = Rsa.SignData(Encoding.ASCII.GetBytes(signingInput), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return signingInput + "." + Base64UrlEncoder.Encode(signature);
        }

        /// <summary>Signs a header and payload with HS256 (an algorithm the validator must refuse).</summary>
        /// <param name="header">The header.</param>
        /// <param name="payload">The payload.</param>
        /// <returns>A compact JWS.</returns>
        public static string SignHs256(Dictionary<string, object> header, Dictionary<string, object> payload)
        {
            string signingInput = Encode(header) + "." + Encode(payload);
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes("a-shared-secret-that-is-long-enough-1234")))
            {
                byte[] signature = hmac.ComputeHash(Encoding.ASCII.GetBytes(signingInput));
                return signingInput + "." + Base64UrlEncoder.Encode(signature);
            }
        }

        /// <summary>An unsigned token (<c>alg: none</c>).</summary>
        /// <param name="header">The header.</param>
        /// <param name="payload">The payload.</param>
        /// <returns>A compact JWS with an empty signature.</returns>
        public static string Unsigned(Dictionary<string, object> header, Dictionary<string, object> payload)
        {
            return Encode(header) + "." + Encode(payload) + ".";
        }

        /// <summary>Base64url JSON.</summary>
        /// <param name="value">The object.</param>
        /// <returns>The encoded segment.</returns>
        private static string Encode(Dictionary<string, object> value)
        {
            return Base64UrlEncoder.Encode(JsonSerializer.Serialize(value));
        }
    }

    /// <summary>An HTTP handler that answers from a callback and counts calls per path.</summary>
    internal sealed class FakeHttpHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, string, Task<HttpResponseMessage>> _respond;

        /// <summary>Creates the handler.</summary>
        /// <param name="respond">Answers a request; the string is the request body (or null).</param>
        public FakeHttpHandler(Func<HttpRequestMessage, string, Task<HttpResponseMessage>> respond)
        {
            _respond = respond;
        }

        /// <summary>Every request seen, in order.</summary>
        public List<HttpRequestMessage> Requests { get; } = new List<HttpRequestMessage>();

        /// <summary>The body of every request seen, in order.</summary>
        public List<string> Bodies { get; } = new List<string>();

        /// <summary>How many requests hit a path ending with <paramref name="suffix"/>.</summary>
        /// <param name="suffix">The path suffix.</param>
        /// <returns>The count.</returns>
        public int Count(string suffix)
        {
            int count = 0;
            foreach (HttpRequestMessage request in Requests)
            {
                if (request.RequestUri.AbsolutePath.EndsWith(suffix, StringComparison.Ordinal))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>Records and answers the request.</summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The token.</param>
        /// <returns>The response.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string body = null;
            if (request.Content != null)
            {
                body = await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            }
            Requests.Add(request);
            Bodies.Add(body);
            Task<HttpResponseMessage> response = _respond(request, body);
            return await response.WaitAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>A JSON response.</summary>
        /// <param name="status">The status code.</param>
        /// <param name="json">The body.</param>
        /// <returns>The response.</returns>
        public static Task<HttpResponseMessage> Json(HttpStatusCode status, string json)
        {
            HttpResponseMessage response = new HttpResponseMessage(status) { Content = new StringContent(json, Encoding.UTF8, "application/json") };
            return Task.FromResult(response);
        }
    }
}
