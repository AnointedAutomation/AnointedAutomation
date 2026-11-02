// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Optimization.Logging;

namespace AnointedAutomation.APIMiddleware.BotDefense
{
    /// <summary>Supported captcha providers.</summary>
    public enum CaptchaProvider
    {
        /// <summary>Google reCAPTCHA v3 (score based).</summary>
        ReCaptchaV3 = 1,

        /// <summary>Cloudflare Turnstile.</summary>
        Turnstile = 2,

        /// <summary>hCaptcha.</summary>
        HCaptcha = 3
    }

    /// <summary>The verdict on one captcha token.</summary>
    public sealed class CaptchaResult
    {
        /// <summary>Gets or sets whether the token passed (valid and, when a score exists, at or above the threshold).</summary>
        public bool Success { get; set; }

        /// <summary>Gets or sets the provider's score, when it gave one.</summary>
        public double? Score { get; set; }

        /// <summary>Gets or sets a short reason when the token did not pass.</summary>
        public string Reason { get; set; }
    }

    /// <summary>Verifies captcha tokens server side.</summary>
    public interface ICaptchaVerifier
    {
        /// <summary>
        /// Verifies a token. Never throws for provider or network failures: those return a failed result (fail closed).
        /// </summary>
        /// <param name="token">The token the browser sent.</param>
        /// <param name="remoteIp">The client IP, or null.</param>
        /// <param name="cancellationToken">Cancellation.</param>
        /// <returns>The verdict.</returns>
        Task<CaptchaResult> VerifyAsync(string token, string remoteIp, CancellationToken cancellationToken);
    }

    /// <summary>Options for <see cref="CaptchaVerifier"/>.</summary>
    public class CaptchaOptions
    {
        /// <summary>The configuration section name.</summary>
        public const string SectionName = "Captcha";

        /// <summary>Gets or sets the provider. Required.</summary>
        public CaptchaProvider? Provider { get; set; }

        /// <summary>Gets or sets the provider's secret key. Required; load it from your secret store, never commit it.</summary>
        public string SecretKey { get; set; }

        /// <summary>Gets or sets the least score that passes when the provider returns one (reCAPTCHA v3, hCaptcha Enterprise).</summary>
        public double MinScore { get; set; } = 0.5;

        /// <summary>Gets or sets an alternate verify URL (self-hosted proxy or tests); null uses the provider's official one.</summary>
        public string VerifyUrl { get; set; }

        /// <summary>
        /// Throws when the options cannot work.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown for missing values.</exception>
        public void Validate()
        {
            if (!Provider.HasValue)
            {
                throw new InvalidOperationException("Captcha Provider is not set.");
            }
            if (string.IsNullOrWhiteSpace(SecretKey))
            {
                throw new InvalidOperationException("Captcha SecretKey is not set.");
            }
            if (MinScore < 0 || MinScore > 1)
            {
                throw new InvalidOperationException("Captcha MinScore must be between 0 and 1.");
            }
        }

        /// <summary>
        /// Gets the verify URL in effect.
        /// </summary>
        /// <returns>The override or the provider's official URL.</returns>
        public string ResolveVerifyUrl()
        {
            if (!string.IsNullOrWhiteSpace(VerifyUrl))
            {
                return VerifyUrl;
            }
            if (Provider.Value == CaptchaProvider.ReCaptchaV3)
            {
                return "https://www.google.com/recaptcha/api/siteverify";
            }
            if (Provider.Value == CaptchaProvider.Turnstile)
            {
                return "https://challenges.cloudflare.com/turnstile/v0/siteverify";
            }
            return "https://api.hcaptcha.com/siteverify";
        }
    }

    /// <summary>
    /// reCAPTCHA v3, Turnstile and hCaptcha all share the same "siteverify" protocol (POST secret, response and
    /// remoteip; JSON back with success, an optional score and error-codes), so one verifier covers them. A
    /// provider outage, bad response or missing score where one is required logs at Error and fails closed.
    /// </summary>
    public sealed class CaptchaVerifier : ICaptchaVerifier
    {
        private readonly HttpClient _client;
        private readonly CaptchaOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptchaVerifier"/> class.
        /// </summary>
        /// <param name="client">The HTTP client.</param>
        /// <param name="options">The options (validated here).</param>
        public CaptchaVerifier(HttpClient client, CaptchaOptions options)
        {
            options.Validate();
            _client = client;
            _options = options;
        }

        /// <inheritdoc />
        public async Task<CaptchaResult> VerifyAsync(string token, string remoteIp, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return new CaptchaResult { Success = false, Reason = "missing captcha token" };
            }
            try
            {
                List<KeyValuePair<string, string>> form = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("secret", _options.SecretKey),
                    new KeyValuePair<string, string>("response", token)
                };
                if (!string.IsNullOrEmpty(remoteIp))
                {
                    form.Add(new KeyValuePair<string, string>("remoteip", remoteIp));
                }
                using (FormUrlEncodedContent content = new FormUrlEncodedContent(form))
                using (HttpResponseMessage response = await _client.PostAsync(_options.ResolveVerifyUrl(), content, cancellationToken).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    string body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                    return Interpret(body);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                IPBlacklistMiddleware.AddLog(LogMessage.Error("Captcha verification with " + _options.Provider.Value + " failed; rejecting the token (fail closed).", ex));
                return new CaptchaResult { Success = false, Reason = "captcha provider unavailable" };
            }
        }

        private CaptchaResult Interpret(string body)
        {
            using (JsonDocument document = JsonDocument.Parse(body))
            {
                JsonElement root = document.RootElement;
                JsonElement successElement;
                if (!root.TryGetProperty("success", out successElement) || successElement.ValueKind == JsonValueKind.Null)
                {
                    throw new FormatException("Captcha response had no 'success' field.");
                }
                bool success = successElement.GetBoolean();
                double? score = null;
                JsonElement scoreElement;
                if (root.TryGetProperty("score", out scoreElement) && scoreElement.ValueKind == JsonValueKind.Number)
                {
                    score = scoreElement.GetDouble();
                }
                if (!success)
                {
                    return new CaptchaResult { Success = false, Score = score, Reason = "provider rejected the token: " + ErrorCodes(root) };
                }
                if (_options.Provider.Value == CaptchaProvider.ReCaptchaV3 && !score.HasValue)
                {
                    throw new FormatException("reCAPTCHA v3 response had no score.");
                }
                if (score.HasValue && score.Value < _options.MinScore)
                {
                    return new CaptchaResult
                    {
                        Success = false,
                        Score = score,
                        Reason = "score " + score.Value.ToString("0.00", CultureInfo.InvariantCulture) + " is below " + _options.MinScore.ToString("0.00", CultureInfo.InvariantCulture)
                    };
                }
                return new CaptchaResult { Success = true, Score = score };
            }
        }

        private static string ErrorCodes(JsonElement root)
        {
            JsonElement codes;
            if (root.TryGetProperty("error-codes", out codes) && codes.ValueKind == JsonValueKind.Array)
            {
                List<string> list = new List<string>();
                foreach (JsonElement code in codes.EnumerateArray())
                {
                    list.Add(code.GetString());
                }
                return string.Join(",", list);
            }
            return "no error codes";
        }
    }
}
