// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using AnointedAutomation.APIMiddleware.Security;
using AnointedAutomation.APIMiddleware.Utility;
using AnointedAutomation.Optimization.Logging;
using Microsoft.AspNetCore.Http;

namespace AnointedAutomation.APIMiddleware.BotDefense
{
    /// <summary>The outcome of a form bot check.</summary>
    public enum FormBotCheckResult
    {
        /// <summary>No bot signal (or the guard is off).</summary>
        Ok = 0,

        /// <summary>The hidden honeypot field was filled in.</summary>
        HoneypotFilled = 1,

        /// <summary>The form was submitted faster than a human could.</summary>
        TooFast = 2,

        /// <summary>The timing token is missing, forged or expired.</summary>
        InvalidTiming = 3
    }

    /// <summary>Options for <see cref="FormBotGuard"/>. Off by default.</summary>
    public class FormBotGuardOptions
    {
        /// <summary>The configuration section name.</summary>
        public const string SectionName = "FormBotGuard";

        /// <summary>Gets or sets whether the guard checks anything. Off by default.</summary>
        public bool Enabled { get; set; }

        /// <summary>Gets or sets the name of the hidden field humans never fill (hide it with CSS, not type=hidden, which some bots skip).</summary>
        public string HoneypotFieldName { get; set; } = "website";

        /// <summary>Gets or sets the least seconds between rendering and submitting the form; 0 turns the timing check off.</summary>
        public int MinSubmitSeconds { get; set; }

        /// <summary>Gets or sets how long a timing token stays valid.</summary>
        public TimeSpan TokenMaxAge { get; set; } = TimeSpan.FromHours(2);

        /// <summary>Gets or sets the secret used to sign timing tokens. Required when the timing check is on. Read it from your secret store, never commit it.</summary>
        public string SigningKey { get; set; }

        /// <summary>Gets or sets how the client IP is found behind proxies (used for log lines).</summary>
        public ClientIpOptions ClientIp { get; set; } = new ClientIpOptions();

        /// <summary>
        /// Throws when the options cannot work.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown for missing or invalid values.</exception>
        public void Validate()
        {
            if (!Enabled)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(HoneypotFieldName))
            {
                throw new InvalidOperationException("FormBotGuard HoneypotFieldName must be set.");
            }
            if (MinSubmitSeconds < 0)
            {
                throw new InvalidOperationException("FormBotGuard MinSubmitSeconds must not be negative.");
            }
            if (MinSubmitSeconds > 0 && (string.IsNullOrWhiteSpace(SigningKey) || SigningKey.Length < 16))
            {
                throw new InvalidOperationException("FormBotGuard timing check needs a SigningKey of at least 16 characters.");
            }
            if (MinSubmitSeconds > 0 && TokenMaxAge <= TimeSpan.FromSeconds(MinSubmitSeconds))
            {
                throw new InvalidOperationException("FormBotGuard TokenMaxAge must be longer than MinSubmitSeconds.");
            }
            ClientIp.BuildTrustedProxies();
        }
    }

    /// <summary>
    /// Form-level bot checks: a hidden honeypot field and a signed render-time token that proves how long the form
    /// was open. The guard only decides and logs (Warning, with IP, rule and path); the caller answers a bot with a
    /// normal-looking success response so the bot learns nothing.
    /// </summary>
    public sealed class FormBotGuard
    {
        private readonly FormBotGuardOptions _options;
        private readonly IpNetworkList _trustedProxies;
        private readonly Func<DateTimeOffset> _clock;

        /// <summary>Initializes a guard that uses the system clock.</summary>
        /// <param name="options">The options (validated here).</param>
        public FormBotGuard(FormBotGuardOptions options) : this(options, () => DateTimeOffset.UtcNow)
        {
        }

        /// <summary>Initializes a guard with a custom clock (tests).</summary>
        /// <param name="options">The options (validated here).</param>
        /// <param name="clock">The clock.</param>
        public FormBotGuard(FormBotGuardOptions options, Func<DateTimeOffset> clock)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            options.Validate();
            _options = options;
            _clock = clock;
            _trustedProxies = options.ClientIp.BuildTrustedProxies();
        }

        /// <summary>Gets the honeypot field name to render.</summary>
        public string HoneypotFieldName
        {
            get { return _options.HoneypotFieldName; }
        }

        /// <summary>
        /// Creates the signed token to embed in the form when it is rendered.
        /// </summary>
        /// <returns>The token.</returns>
        public string CreateTimingToken()
        {
            string issued = _clock().ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
            return issued + "." + Sign(issued);
        }

        /// <summary>
        /// Checks a submission.
        /// </summary>
        /// <param name="context">The request (for the log line).</param>
        /// <param name="honeypotValue">The submitted value of the honeypot field.</param>
        /// <param name="timingToken">The submitted timing token.</param>
        /// <returns>Ok, or the first bot signal found.</returns>
        public FormBotCheckResult Check(HttpContext context, string honeypotValue, string timingToken)
        {
            if (!_options.Enabled)
            {
                return FormBotCheckResult.Ok;
            }
            if (!string.IsNullOrEmpty(honeypotValue))
            {
                Log(context, "form honeypot field '" + _options.HoneypotFieldName + "' was filled");
                return FormBotCheckResult.HoneypotFilled;
            }
            if (_options.MinSubmitSeconds > 0)
            {
                long issuedAt;
                if (!TryReadToken(timingToken, out issuedAt))
                {
                    Log(context, "form timing token missing, forged or malformed");
                    return FormBotCheckResult.InvalidTiming;
                }
                TimeSpan age = _clock() - DateTimeOffset.FromUnixTimeSeconds(issuedAt);
                if (age > _options.TokenMaxAge || age < TimeSpan.Zero)
                {
                    Log(context, "form timing token expired or from the future");
                    return FormBotCheckResult.InvalidTiming;
                }
                if (age < TimeSpan.FromSeconds(_options.MinSubmitSeconds))
                {
                    Log(context, "form submitted in " + age.TotalSeconds.ToString("0", CultureInfo.InvariantCulture) + "s, faster than " + _options.MinSubmitSeconds + "s");
                    return FormBotCheckResult.TooFast;
                }
            }
            return FormBotCheckResult.Ok;
        }

        private bool TryReadToken(string token, out long issuedAt)
        {
            issuedAt = 0;
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }
            int dot = token.IndexOf('.');
            if (dot <= 0 || !long.TryParse(token.Substring(0, dot), NumberStyles.None, CultureInfo.InvariantCulture, out issuedAt))
            {
                return false;
            }
            byte[] expected = Encoding.ASCII.GetBytes(Sign(token.Substring(0, dot)));
            byte[] presented = Encoding.ASCII.GetBytes(token.Substring(dot + 1));
            return CryptographicOperations.FixedTimeEquals(expected, presented);
        }

        private string Sign(string payload)
        {
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_options.SigningKey)))
            {
                return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));
            }
        }

        private void Log(HttpContext context, string rule)
        {
            IPAddress client = ClientIpResolver.Resolve(context, _trustedProxies, _options.ClientIp.HonorCfConnectingIp);
            IPBlacklistMiddleware.AddLog(LogMessage.Warning(
                "FormBotGuard flagged " + (client == null ? "(no client address)" : client.ToString()) + " " + APIUtility.DescribeRequest(context) + ": " + rule + "."));
        }
    }
}
