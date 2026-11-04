// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace AnointedAutomation.APIMiddleware.BotDefense
{
    /// <summary>
    /// DI and pipeline registration for bot defense, the form guard and captcha verification.
    /// </summary>
    public static class BotDefenseExtensions
    {
        /// <summary>
        /// Registers bot defense from a configuration section. Register your own <see cref="IBanStore"/> first to
        /// keep bans in Mongo or Redis.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="section">The configuration section.</param>
        /// <returns>The services.</returns>
        public static IServiceCollection AddBotDefense(this IServiceCollection services, IConfiguration section)
        {
            services.Configure<BotDefenseOptions>(section);
            return AddBotDefenseCore(services);
        }

        /// <summary>
        /// Registers bot defense from code.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configure">Sets the options.</param>
        /// <returns>The services.</returns>
        public static IServiceCollection AddBotDefense(this IServiceCollection services, Action<BotDefenseOptions> configure)
        {
            services.Configure(configure);
            return AddBotDefenseCore(services);
        }

        /// <summary>
        /// Adds the middleware. Invalid options throw here, at startup.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <returns>The builder.</returns>
        public static IApplicationBuilder UseBotDefense(this IApplicationBuilder app)
        {
            app.ApplicationServices.GetRequiredService<IOptions<BotDefenseOptions>>().Value.Validate();
            return app.UseMiddleware<BotDefenseMiddleware>();
        }

        /// <summary>
        /// Registers the form guard (honeypot field and timing token).
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configure">Sets the options.</param>
        /// <returns>The services.</returns>
        public static IServiceCollection AddFormBotGuard(this IServiceCollection services, Action<FormBotGuardOptions> configure)
        {
            services.Configure(configure);
            services.TryAddSingleton<FormBotGuard>(provider => new FormBotGuard(provider.GetRequiredService<IOptions<FormBotGuardOptions>>().Value));
            return services;
        }

        /// <summary>
        /// Registers the form guard from a configuration section.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="section">The configuration section.</param>
        /// <returns>The services.</returns>
        public static IServiceCollection AddFormBotGuard(this IServiceCollection services, IConfiguration section)
        {
            services.Configure<FormBotGuardOptions>(section);
            services.TryAddSingleton<FormBotGuard>(provider => new FormBotGuard(provider.GetRequiredService<IOptions<FormBotGuardOptions>>().Value));
            return services;
        }

        /// <summary>
        /// Registers captcha verification for one provider.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configure">Sets the options (Provider and SecretKey are required).</param>
        /// <returns>The services.</returns>
        public static IServiceCollection AddCaptchaVerifier(this IServiceCollection services, Action<CaptchaOptions> configure)
        {
            services.Configure(configure);
            services.TryAddSingleton<ICaptchaVerifier>(provider =>
                new CaptchaVerifier(new HttpClient { Timeout = TimeSpan.FromSeconds(10) }, provider.GetRequiredService<IOptions<CaptchaOptions>>().Value));
            return services;
        }

        private static IServiceCollection AddBotDefenseCore(IServiceCollection services)
        {
            services.TryAddSingleton<IBanStore, InMemoryBanStore>();
            services.TryAddSingleton<ICrawlerVerifier, ReverseDnsCrawlerVerifier>();
            services.TryAddSingleton<FixedWindowRateLimiter>();
            return services;
        }
    }
}
