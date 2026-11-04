// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Optimization.Logging;
using Microsoft.Extensions.Hosting;

namespace AnointedAutomation.APIMiddleware.Security
{
    /// <summary>Downloads one country data file.</summary>
    public interface ICountryDataFetcher
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
    /// <see cref="ICountryDataFetcher"/> over <see cref="HttpClient"/>.
    /// </summary>
    public sealed class HttpCountryDataFetcher : ICountryDataFetcher
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpCountryDataFetcher"/> class.
        /// </summary>
        /// <param name="client">The HTTP client.</param>
        public HttpCountryDataFetcher(HttpClient client)
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
    /// Downloads the registry files and swaps them into <see cref="CountryDataState"/>. If any file fails to
    /// download or yields no ranges the whole refresh is discarded: the failure is logged at Error and the last
    /// good data stays (a half-loaded refresh would silently drop a region).
    /// </summary>
    public sealed class CountryDataRefresher
    {
        private readonly CountryDataState _state;
        private readonly ICountryDataFetcher _fetcher;
        private readonly CountryBlockOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryDataRefresher"/> class.
        /// </summary>
        /// <param name="state">The live data.</param>
        /// <param name="fetcher">The downloader.</param>
        /// <param name="options">The options.</param>
        public CountryDataRefresher(CountryDataState state, ICountryDataFetcher fetcher, CountryBlockOptions options)
        {
            _state = state;
            _fetcher = fetcher;
            _options = options;
        }

        /// <summary>
        /// Runs one refresh.
        /// </summary>
        /// <param name="cancellationToken">Cancellation (shutdown); rethrown without an error log.</param>
        /// <returns>True when new data was swapped in.</returns>
        public async Task<bool> RefreshAsync(CancellationToken cancellationToken)
        {
            try
            {
                RirDelegatedCountryLookup.Builder builder = new RirDelegatedCountryLookup.Builder();
                foreach (string url in _options.ResolveDataUrls())
                {
                    string body = await _fetcher.FetchAsync(url, cancellationToken).ConfigureAwait(false);
                    if (builder.Add(body) == 0)
                    {
                        throw new FormatException("Country data file had no allocated ranges: " + url);
                    }
                }
                _state.Replace(builder.Build());
                return true;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                IPBlacklistMiddleware.AddLog(LogMessage.Error(
                    "CountryBlock data refresh failed; " + (_state.HasData ? "keeping the last good data." : "no data loaded yet, unknown-country policy applies."), ex));
                return false;
            }
        }
    }

    /// <summary>
    /// Background service that loads the country data at startup and refreshes it on the interval. Does nothing
    /// when country blocking is off, when <see cref="CountryBlockOptions.DownloadRirData"/> is false, or when the
    /// host registered its own <see cref="ICountryLookup"/>.
    /// </summary>
    public sealed class CountryDataRefreshService : BackgroundService
    {
        private readonly CountryBlockOptions _options;
        private readonly CountryDataState _state;
        private readonly CountryDataRefresher _refresher;
        private readonly ICountryLookup _registeredLookup;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryDataRefreshService"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="state">The live data.</param>
        /// <param name="refresher">The refresher.</param>
        /// <param name="registeredLookup">The lookup the middleware uses.</param>
        public CountryDataRefreshService(CountryBlockOptions options, CountryDataState state, CountryDataRefresher refresher, ICountryLookup registeredLookup)
        {
            _options = options;
            _state = state;
            _refresher = refresher;
            _registeredLookup = registeredLookup;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_options.Enabled || !_options.DownloadRirData || !object.ReferenceEquals(_registeredLookup, _state))
            {
                return;
            }
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        await _refresher.RefreshAsync(stoppingToken).ConfigureAwait(false);
                    }
                    catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
                    {
                        IPBlacklistMiddleware.AddLog(LogMessage.Error("CountryBlock refresh cycle crashed; will retry.", ex));
                    }
                    await Task.Delay(_state.HasData ? _options.RefreshInterval : _options.RetryInterval, stoppingToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                // Shutdown: the one quiet case.
            }
        }
    }
}
