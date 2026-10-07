// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AnointedAutomation.SSO
{
    /// <summary>
    /// The default <see cref="IAnointedSessionStatus"/>: exchanges your API key for a machine token (cached until
    /// shortly before it expires, re-exchanged once on a 401), calls <c>GET api/session-status</c> and caches
    /// Active and Ended answers. FAIL-OPEN: any failure is <see cref="AnointedSessionOutcome.Unknown"/>, never cached.
    /// </summary>
    public class AnointedSessionStatusClient : IAnointedSessionStatus
    {
        private readonly HttpClient _httpClient;
        private readonly AnointedSessionStatusOptions _options;
        private readonly AnointedSessionStatusCache _cache;
        private readonly TimeProvider _timeProvider;
        private readonly ILogger<AnointedSessionStatusClient> _logger;

        /// <summary>Creates the client (normally through <c>AddAnointedSessionStatus</c>).</summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="options">Validated settings.</param>
        /// <param name="cache">The shared token and answer cache.</param>
        /// <param name="timeProvider">The clock.</param>
        /// <param name="logger">The logger.</param>
        public AnointedSessionStatusClient(HttpClient httpClient, AnointedSessionStatusOptions options, AnointedSessionStatusCache cache, TimeProvider timeProvider, ILogger<AnointedSessionStatusClient> logger)
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (cache == null)
            {
                throw new ArgumentNullException(nameof(cache));
            }
            if (timeProvider == null)
            {
                throw new ArgumentNullException(nameof(timeProvider));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            options.Validate();
            _httpClient = httpClient;
            _options = options;
            _cache = cache;
            _timeProvider = timeProvider;
            _logger = logger;
        }

        /// <summary>Checks one user's session. Never throws for provider or network failures (fail open).</summary>
        /// <param name="userId">The Anointed Automation user id (<c>sub</c>).</param>
        /// <param name="grant">True for a session from Sign in with Anointed Automation.</param>
        /// <param name="cancellationToken">Cancels the check; a cancellation by the caller is rethrown.</param>
        /// <returns>The check.</returns>
        public async Task<AnointedSessionCheck> CheckAsync(string userId, bool grant, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("A user id (the sub claim) is required.", nameof(userId));
            }
            string cacheKey = (grant ? "1|" : "0|") + userId;
            if (_cache.TryGetAnswer(cacheKey, _timeProvider.GetUtcNow(), out AnointedSessionCheck cached))
            {
                return cached;
            }

            AnointedSessionCheck answer;
            try
            {
                answer = await QueryAsync(userId, grant, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is OperationCanceledException || ex is JsonException || ex is AnointedSessionStatusException)
            {
                _logger.LogWarning(ex, "Anointed session status check failed; keeping the session (fail open).");
                return AnointedSessionCheck.Unknown(ex.GetType().Name + ": " + ex.Message);
            }

            if (answer.Outcome == AnointedSessionOutcome.Unknown)
            {
                _logger.LogWarning("Anointed session status gave no answer ({Error}); keeping the session (fail open).", answer.Error);
                return answer;
            }
            _cache.SetAnswer(cacheKey, answer, _timeProvider.GetUtcNow(), _options.AnswerCacheDuration);
            return answer;
        }

        /// <summary>Calls session-status with a machine token, re-exchanging it once on a 401.</summary>
        /// <param name="userId">The user id.</param>
        /// <param name="grant">The grant flag.</param>
        /// <param name="cancellationToken">The caller's token.</param>
        /// <returns>The parsed answer, or Unknown for a non-200.</returns>
        private async Task<AnointedSessionCheck> QueryAsync(string userId, bool grant, CancellationToken cancellationToken)
        {
            string token = await GetMachineTokenAsync(null, cancellationToken).ConfigureAwait(false);
            using (HttpResponseMessage first = await SendStatusAsync(userId, grant, token, cancellationToken).ConfigureAwait(false))
            {
                if (first.StatusCode != HttpStatusCode.Unauthorized)
                {
                    return await ReadStatusAsync(first, cancellationToken).ConfigureAwait(false);
                }
            }
            token = await GetMachineTokenAsync(token, cancellationToken).ConfigureAwait(false);
            using (HttpResponseMessage second = await SendStatusAsync(userId, grant, token, cancellationToken).ConfigureAwait(false))
            {
                return await ReadStatusAsync(second, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>Sends one session-status request with the per-request timeout.</summary>
        /// <param name="userId">The user id.</param>
        /// <param name="grant">The grant flag.</param>
        /// <param name="token">The machine token.</param>
        /// <param name="cancellationToken">The caller's token.</param>
        /// <returns>The response (the caller disposes it).</returns>
        private async Task<HttpResponseMessage> SendStatusAsync(string userId, bool grant, string token, CancellationToken cancellationToken)
        {
            string relative = AnointedAutomationDefaults.SessionStatusPath + "?userId=" + Uri.EscapeDataString(userId) + "&grant=" + (grant ? "true" : "false");
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(_options.BaseAddress, relative)))
            using (CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                timeout.CancelAfter(_options.Timeout);
                HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, timeout.Token).ConfigureAwait(false);
                return response;
            }
        }

        /// <summary>Turns a session-status response into a check.</summary>
        /// <param name="response">The response.</param>
        /// <param name="cancellationToken">The caller's token.</param>
        /// <returns>Active or Ended for a well-formed 200; Unknown for any other status.</returns>
        private static async Task<AnointedSessionCheck> ReadStatusAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return AnointedSessionCheck.Unknown("session-status answered HTTP " + ((int)response.StatusCode).ToString(CultureInfo.InvariantCulture));
            }
            string body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return ParseStatus(body);
        }

        /// <summary>
        /// Parses a session-status body case-insensitively (the API mixes <c>Data</c>/<c>Reason</c> with
        /// <c>active</c>/<c>success</c>).
        /// </summary>
        /// <param name="body">The JSON body.</param>
        /// <returns>Active or Ended, or Unknown when <c>success</c> is false.</returns>
        /// <exception cref="AnointedSessionStatusException">The body is not the expected shape.</exception>
        /// <exception cref="JsonException">The body is not JSON.</exception>
        internal static AnointedSessionCheck ParseStatus(string body)
        {
            using (JsonDocument document = JsonDocument.Parse(body))
            {
                JsonElement root = document.RootElement;
                if (root.ValueKind != JsonValueKind.Object)
                {
                    throw new AnointedSessionStatusException("session-status body is not a JSON object");
                }
                if (TryFind(root, "success", out JsonElement success) && success.ValueKind == JsonValueKind.False)
                {
                    return AnointedSessionCheck.Unknown("session-status answered success: false");
                }
                if (!TryFind(root, "data", out JsonElement data) || data.ValueKind != JsonValueKind.Object)
                {
                    throw new AnointedSessionStatusException("session-status body has no Data object");
                }
                if (!TryFind(data, "active", out JsonElement active) || (active.ValueKind != JsonValueKind.True && active.ValueKind != JsonValueKind.False))
                {
                    throw new AnointedSessionStatusException("session-status Data.active is not a boolean");
                }
                string reason = null;
                if (TryFind(data, "reason", out JsonElement reasonElement) && reasonElement.ValueKind == JsonValueKind.String)
                {
                    reason = reasonElement.GetString();
                }
                return active.ValueKind == JsonValueKind.True ? AnointedSessionCheck.Active(reason) : AnointedSessionCheck.Ended(reason);
            }
        }

        /// <summary>
        /// Returns the cached machine token, or exchanges the API key for a new one when there is none, it is
        /// near expiry, or the caller reports <paramref name="rejectedToken"/> was refused.
        /// </summary>
        /// <param name="rejectedToken">A token the API just refused with 401, or null.</param>
        /// <param name="cancellationToken">The caller's token.</param>
        /// <returns>A machine token.</returns>
        private async Task<string> GetMachineTokenAsync(string rejectedToken, CancellationToken cancellationToken)
        {
            await _cache.TokenGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                DateTimeOffset now = _timeProvider.GetUtcNow();
                string current = _cache.MachineToken;
                bool usable = current != null && now < _cache.MachineTokenRefreshAt;
                bool refused = rejectedToken != null && string.Equals(current, rejectedToken, StringComparison.Ordinal);
                if (usable && !refused)
                {
                    return current;
                }
                MachineToken exchanged = await ExchangeAsync(cancellationToken).ConfigureAwait(false);
                _cache.MachineToken = exchanged.Token;
                _cache.MachineTokenRefreshAt = exchanged.ExpiresAt - _options.TokenRefreshMargin;
                return exchanged.Token;
            }
            finally
            {
                _cache.TokenGate.Release();
            }
        }

        /// <summary>POSTs the API key to the machine token endpoint.</summary>
        /// <param name="cancellationToken">The caller's token.</param>
        /// <returns>The token and its expiry.</returns>
        /// <exception cref="AnointedSessionStatusException">The exchange did not answer 200 with a token.</exception>
        private async Task<MachineToken> ExchangeAsync(CancellationToken cancellationToken)
        {
            Dictionary<string, string> payload = new Dictionary<string, string>
            {
                { "apiKey", _options.ApiKey },
                { "name", _options.AppName },
            };
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(_options.BaseAddress, AnointedAutomationDefaults.MachineTokenPath)))
            using (CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                timeout.CancelAfter(_options.Timeout);
                using (HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, timeout.Token).ConfigureAwait(false))
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new AnointedSessionStatusException("machine-token exchange answered HTTP " + ((int)response.StatusCode).ToString(CultureInfo.InvariantCulture));
                    }
                    string body = await response.Content.ReadAsStringAsync(timeout.Token).ConfigureAwait(false);
                    return ParseMachineToken(body);
                }
            }
        }

        /// <summary>Parses <c>{"Data":{"token":"...","expiresAt":"..."}}</c> case-insensitively.</summary>
        /// <param name="body">The JSON body.</param>
        /// <returns>The token and its expiry.</returns>
        /// <exception cref="AnointedSessionStatusException">A field is missing or unreadable.</exception>
        internal static MachineToken ParseMachineToken(string body)
        {
            using (JsonDocument document = JsonDocument.Parse(body))
            {
                JsonElement root = document.RootElement;
                if (root.ValueKind != JsonValueKind.Object || !TryFind(root, "data", out JsonElement data) || data.ValueKind != JsonValueKind.Object)
                {
                    throw new AnointedSessionStatusException("machine-token body has no Data object");
                }
                if (!TryFind(data, "token", out JsonElement tokenElement) || tokenElement.ValueKind != JsonValueKind.String || string.IsNullOrEmpty(tokenElement.GetString()))
                {
                    throw new AnointedSessionStatusException("machine-token body has no token");
                }
                if (!TryFind(data, "expiresAt", out JsonElement expiresElement) || expiresElement.ValueKind != JsonValueKind.String
                    || !DateTimeOffset.TryParse(expiresElement.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset expiresAt))
                {
                    throw new AnointedSessionStatusException("machine-token body has no readable expiresAt");
                }
                return new MachineToken(tokenElement.GetString(), expiresAt);
            }
        }

        /// <summary>Finds a property by name, ignoring case.</summary>
        /// <param name="element">The object to search.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The value when found.</param>
        /// <returns>True when found.</returns>
        private static bool TryFind(JsonElement element, string name, out JsonElement value)
        {
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    value = property.Value;
                    return true;
                }
            }
            value = default(JsonElement);
            return false;
        }

        /// <summary>A machine token and when it expires.</summary>
        internal sealed class MachineToken
        {
            /// <summary>Creates the pair.</summary>
            /// <param name="token">The bearer token.</param>
            /// <param name="expiresAt">Its expiry.</param>
            public MachineToken(string token, DateTimeOffset expiresAt)
            {
                Token = token;
                ExpiresAt = expiresAt;
            }

            /// <summary>The bearer token.</summary>
            public string Token { get; }

            /// <summary>When the token expires.</summary>
            public DateTimeOffset ExpiresAt { get; }
        }
    }

    /// <summary>
    /// The state <see cref="AnointedSessionStatusClient"/> shares across requests (registered as a singleton):
    /// the machine token and the cached answers.
    /// </summary>
    public class AnointedSessionStatusCache
    {
        private readonly ConcurrentDictionary<string, CachedAnswer> _answers = new ConcurrentDictionary<string, CachedAnswer>(StringComparer.Ordinal);

        /// <summary>Serializes machine token exchanges.</summary>
        internal SemaphoreSlim TokenGate { get; } = new SemaphoreSlim(1, 1);

        /// <summary>The current machine token, or null.</summary>
        internal string MachineToken { get; set; }

        /// <summary>When the current machine token must be re-exchanged.</summary>
        internal DateTimeOffset MachineTokenRefreshAt { get; set; }

        /// <summary>Reads a cached answer that has not expired.</summary>
        /// <param name="key">The (grant, userId) key.</param>
        /// <param name="now">The current time.</param>
        /// <param name="answer">The answer when found.</param>
        /// <returns>True when a live answer exists.</returns>
        internal bool TryGetAnswer(string key, DateTimeOffset now, out AnointedSessionCheck answer)
        {
            if (_answers.TryGetValue(key, out CachedAnswer entry) && now < entry.ExpiresAt)
            {
                answer = entry.Answer;
                return true;
            }
            answer = null;
            return false;
        }

        /// <summary>Stores an answer and prunes expired ones.</summary>
        /// <param name="key">The (grant, userId) key.</param>
        /// <param name="answer">The Active or Ended answer.</param>
        /// <param name="now">The current time.</param>
        /// <param name="lifetime">How long the answer is reused.</param>
        internal void SetAnswer(string key, AnointedSessionCheck answer, DateTimeOffset now, TimeSpan lifetime)
        {
            foreach (KeyValuePair<string, CachedAnswer> entry in _answers)
            {
                if (entry.Value.ExpiresAt <= now)
                {
                    _answers.TryRemove(entry);
                }
            }
            _answers[key] = new CachedAnswer(answer, now + lifetime);
        }

        /// <summary>A cached answer and its expiry.</summary>
        private sealed class CachedAnswer
        {
            /// <summary>Creates the entry.</summary>
            /// <param name="answer">The answer.</param>
            /// <param name="expiresAt">Its expiry.</param>
            public CachedAnswer(AnointedSessionCheck answer, DateTimeOffset expiresAt)
            {
                Answer = answer;
                ExpiresAt = expiresAt;
            }

            /// <summary>The answer.</summary>
            public AnointedSessionCheck Answer { get; }

            /// <summary>When it expires.</summary>
            public DateTimeOffset ExpiresAt { get; }
        }
    }

    /// <summary>The session status API answered something this package cannot read.</summary>
    public class AnointedSessionStatusException : Exception
    {
        /// <summary>Creates the exception.</summary>
        /// <param name="message">What was wrong.</param>
        public AnointedSessionStatusException(string message) : base(message)
        {
        }
    }
}
