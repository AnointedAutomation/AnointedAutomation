// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace AnointedAutomation.Concepts.Anthropic
{
    /// <summary>
    /// Captures the Retry-After header of each response into the slot of the current call. The SDK's exceptions in
    /// 13.8.0 do not expose response headers, so this is how a 429 backoff can honor the server's hint.
    /// </summary>
    internal sealed class RetryAfterRecorder : DelegatingHandler
    {
        internal static readonly AsyncLocal<RetryAfterSlot> Current = new AsyncLocal<RetryAfterSlot>();

        public RetryAfterRecorder(HttpMessageHandler inner)
            : base(inner)
        {
        }

        internal static TimeSpan? Read(RetryConditionHeaderValue header, DateTimeOffset now)
        {
            if (header == null)
            {
                return null;
            }

            if (header.Delta.HasValue)
            {
                return header.Delta.Value;
            }

            if (header.Date.HasValue)
            {
                TimeSpan wait = header.Date.Value - now;
                return wait < TimeSpan.Zero ? TimeSpan.Zero : wait;
            }

            return null;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            RetryAfterSlot slot = Current.Value;
            if (slot != null)
            {
                slot.Delay = Read(response.Headers.RetryAfter, DateTimeOffset.UtcNow);
            }

            return response;
        }
    }

    /// <summary>
    /// The Retry-After delay seen by one attempt, if the response carried one.
    /// </summary>
    internal sealed class RetryAfterSlot
    {
        public TimeSpan? Delay { get; set; }
    }
}
