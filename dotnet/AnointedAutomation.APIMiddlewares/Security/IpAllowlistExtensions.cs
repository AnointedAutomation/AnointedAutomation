// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace AnointedAutomation.APIMiddleware.Security
{
    /// <summary>
    /// DI and pipeline registration for the IP allowlist.
    /// </summary>
    public static class IpAllowlistExtensions
    {
        /// <summary>
        /// Registers the allowlist from a configuration section (for example <c>IpAllowlist</c>).
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="section">The configuration section.</param>
        /// <returns>The services.</returns>
        public static IServiceCollection AddIpAllowlist(this IServiceCollection services, IConfiguration section)
        {
            services.Configure<IpAllowlistOptions>(section);
            return AddCore(services);
        }

        /// <summary>
        /// Registers the allowlist from code.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configure">Sets the options.</param>
        /// <returns>The services.</returns>
        public static IServiceCollection AddIpAllowlist(this IServiceCollection services, Action<IpAllowlistOptions> configure)
        {
            services.Configure(configure);
            return AddCore(services);
        }

        /// <summary>
        /// Adds the middleware. Invalid options throw here, at startup.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <returns>The builder.</returns>
        public static IApplicationBuilder UseIpAllowlist(this IApplicationBuilder app)
        {
            app.ApplicationServices.GetRequiredService<IpAllowlistState>();
            return app.UseMiddleware<IpAllowlistMiddleware>();
        }

        private static IServiceCollection AddCore(IServiceCollection services)
        {
            services.TryAddSingleton<IpAllowlistState>(provider => new IpAllowlistState(provider.GetRequiredService<IOptions<IpAllowlistOptions>>().Value));
            services.TryAddSingleton<IProviderRangeFetcher>(provider =>
                new HttpProviderRangeFetcher(new HttpClient { Timeout = provider.GetRequiredService<IOptions<IpAllowlistOptions>>().Value.RefreshTimeout }));
            services.TryAddSingleton<IpAllowlistRefresher>();
            services.AddHostedService<IpAllowlistRefreshService>();
            return services;
        }
    }
}
