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
    /// DI and pipeline registration for country blocking.
    /// </summary>
    public static class CountryBlockExtensions
    {
        /// <summary>
        /// Registers country blocking from a configuration section (for example <c>CountryBlock</c>). Register your
        /// own <see cref="ICountryLookup"/> before this call to use a different data source.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="section">The configuration section.</param>
        /// <returns>The services.</returns>
        public static IServiceCollection AddCountryBlock(this IServiceCollection services, IConfiguration section)
        {
            services.Configure<CountryBlockOptions>(section);
            return AddCore(services);
        }

        /// <summary>
        /// Registers country blocking from code.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configure">Sets the options.</param>
        /// <returns>The services.</returns>
        public static IServiceCollection AddCountryBlock(this IServiceCollection services, Action<CountryBlockOptions> configure)
        {
            services.Configure(configure);
            return AddCore(services);
        }

        /// <summary>
        /// Adds the middleware. Invalid options throw here, at startup.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <returns>The builder.</returns>
        public static IApplicationBuilder UseCountryBlock(this IApplicationBuilder app)
        {
            CountryBlockOptions options = app.ApplicationServices.GetRequiredService<IOptions<CountryBlockOptions>>().Value;
            options.Validate();
            // When the IP allowlist is registered its refreshed Cloudflare list decides which peers are Cloudflare;
            // otherwise the ranges shipped with the package do.
            IpAllowlistState allowlist = app.ApplicationServices.GetService<IpAllowlistState>();
            Func<IpNetworkList> cloudflareEdge = allowlist == null
                ? () => ClientIpResolver.CloudflareEdge
                : () => allowlist.CloudflareEdge;
            return app.UseMiddleware<CountryBlockMiddleware>(options, cloudflareEdge);
        }

        private static IServiceCollection AddCore(IServiceCollection services)
        {
            services.TryAddSingleton<CountryDataState>();
            services.TryAddSingleton<ICountryLookup>(provider => provider.GetRequiredService<CountryDataState>());
            services.TryAddSingleton<ICountryDataFetcher>(provider =>
                new HttpCountryDataFetcher(new HttpClient { Timeout = provider.GetRequiredService<IOptions<CountryBlockOptions>>().Value.RefreshTimeout }));
            services.TryAddSingleton<CountryDataRefresher>(provider => new CountryDataRefresher(
                provider.GetRequiredService<CountryDataState>(),
                provider.GetRequiredService<ICountryDataFetcher>(),
                provider.GetRequiredService<IOptions<CountryBlockOptions>>().Value));
            services.AddHostedService<CountryDataRefreshService>(provider => new CountryDataRefreshService(
                provider.GetRequiredService<IOptions<CountryBlockOptions>>().Value,
                provider.GetRequiredService<CountryDataState>(),
                provider.GetRequiredService<CountryDataRefresher>(),
                provider.GetRequiredService<ICountryLookup>()));
            return services;
        }
    }
}
