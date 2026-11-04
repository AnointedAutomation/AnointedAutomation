// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Optimization.Logging;
using Microsoft.Extensions.Hosting;

namespace AnointedAutomation.APIMiddleware.Security
{
    /// <summary>
    /// Background service that runs <see cref="IpAllowlistRefresher"/> on the configured interval. Does nothing
    /// unless the allowlist and RefreshEnabled are both on.
    /// </summary>
    public sealed class IpAllowlistRefreshService : BackgroundService
    {
        private readonly IpAllowlistState _state;
        private readonly IpAllowlistRefresher _refresher;

        /// <summary>
        /// Initializes a new instance of the <see cref="IpAllowlistRefreshService"/> class.
        /// </summary>
        /// <param name="state">The live state.</param>
        /// <param name="refresher">The refresher.</param>
        public IpAllowlistRefreshService(IpAllowlistState state, IpAllowlistRefresher refresher)
        {
            _state = state;
            _refresher = refresher;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_state.Options.Enabled || !_state.Options.RefreshEnabled)
            {
                return;
            }
            using (PeriodicTimer timer = new PeriodicTimer(_state.Options.RefreshInterval))
            {
                try
                {
                    while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false))
                    {
                        try
                        {
                            await _refresher.RefreshAllAsync(stoppingToken).ConfigureAwait(false);
                        }
                        catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
                        {
                            IPBlacklistMiddleware.AddLog(LogMessage.Error("IpAllowlist refresh cycle crashed; will retry next interval.", ex));
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Shutdown: the one quiet case.
                }
            }
        }
    }
}
