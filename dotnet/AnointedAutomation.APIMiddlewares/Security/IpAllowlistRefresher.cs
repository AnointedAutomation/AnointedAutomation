// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Optimization.Logging;

namespace AnointedAutomation.APIMiddleware.Security
{
    /// <summary>Downloads a provider's published list.</summary>
    public interface IProviderRangeFetcher
    {
        /// <summary>
        /// Downloads one URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="cancellationToken">Cancellation.</param>
        /// <returns>The response body.</returns>
        Task<string> FetchAsync(string url, CancellationToken cancellationToken);
    }

    /// <summary>
    /// <see cref="IProviderRangeFetcher"/> over <see cref="HttpClient"/>.
    /// </summary>
    public sealed class HttpProviderRangeFetcher : IProviderRangeFetcher
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpProviderRangeFetcher"/> class.
        /// </summary>
        /// <param name="client">The HTTP client (its Timeout applies to each download).</param>
        public HttpProviderRangeFetcher(HttpClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public async Task<string> FetchAsync(string url, CancellationToken cancellationToken)
        {
            using (HttpResponseMessage response = await _client.GetAsync(url, cancellationToken).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Refreshes provider ranges. A failure of any kind (network, HTTP status, malformed or empty list) logs at
    /// Error and keeps the last good list; the allowlist never opens up and never empties.
    /// </summary>
    public sealed class IpAllowlistRefresher
    {
        private readonly IpAllowlistState _state;
        private readonly IProviderRangeFetcher _fetcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="IpAllowlistRefresher"/> class.
        /// </summary>
        /// <param name="state">The live state.</param>
        /// <param name="fetcher">The downloader.</param>
        public IpAllowlistRefresher(IpAllowlistState state, IProviderRangeFetcher fetcher)
        {
            _state = state;
            _fetcher = fetcher;
        }

        /// <summary>
        /// Refreshes every active provider.
        /// </summary>
        /// <param name="cancellationToken">Cancellation (shutdown); rethrown without an error log.</param>
        /// <returns>The number of providers whose refresh failed.</returns>
        public async Task<int> RefreshAllAsync(CancellationToken cancellationToken)
        {
            int failures = 0;
            foreach (string provider in _state.ProviderNames)
            {
                if (!await RefreshProviderAsync(provider, cancellationToken).ConfigureAwait(false))
                {
                    failures++;
                }
            }
            return failures;
        }

        /// <summary>
        /// Refreshes one provider.
        /// </summary>
        /// <param name="provider">The preset name.</param>
        /// <param name="cancellationToken">Cancellation.</param>
        /// <returns>True when the list was replaced.</returns>
        public async Task<bool> RefreshProviderAsync(string provider, CancellationToken cancellationToken)
        {
            try
            {
                IpProviderSource source = IpProviderPresets.GetSource(provider);
                List<string> bodies = new List<string>();
                foreach (string url in source.Urls)
                {
                    bodies.Add(await _fetcher.FetchAsync(url, cancellationToken).ConfigureAwait(false));
                }
                List<string> cidrs = IpProviderListParser.Parse(source.Format, bodies);
                _state.ReplaceProvider(source.Name, IpNetworkList.Parse(cidrs));
                return true;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                IPBlacklistMiddleware.AddLog(LogMessage.Error(
                    "IpAllowlist refresh of provider '" + provider + "' failed; keeping the last good list of "
                    + _state.GetProviderRangeCount(provider) + " ranges.", ex));
                return false;
            }
        }
    }
}
