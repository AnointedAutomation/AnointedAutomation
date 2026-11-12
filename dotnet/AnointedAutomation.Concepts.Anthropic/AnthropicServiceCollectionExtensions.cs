// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields
using System;
using System.Net.Http;
using AnointedAutomation.Concepts.Reasoning.Language;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AnointedAutomation.Concepts.Anthropic
{
    /// <summary>
    /// Registers <see cref="AnthropicLanguageModel"/> as the <see cref="ILanguageModel"/>.
    /// </summary>
    public static class AnthropicServiceCollectionExtensions
    {
        /// <summary>
        /// The IHttpClientFactory name whose handler pipeline the adapter sends through; configure proxies or handlers on it.
        /// </summary>
        public const string HttpClientName = "AnointedAutomation.Concepts.Anthropic";

        /// <summary>
        /// Binds <see cref="AnthropicLanguageModelOptions"/> from <paramref name="section"/>, validates it at once
        /// (throwing <see cref="AnthropicConfigurationException"/> when ApiKey, Model or another setting is missing),
        /// and registers the adapter (over the named HTTP client <see cref="HttpClientName"/>) resolved as <see cref="ILanguageModel"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="section">The configuration section holding the options, for example "Anthropic".</param>
        /// <returns>The same collection.</returns>
        public static IServiceCollection AddAnthropicLanguageModel(this IServiceCollection services, IConfigurationSection section)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(section);

            AnthropicLanguageModelOptions options = section.Get<AnthropicLanguageModelOptions>();
            if (options == null)
            {
                throw new AnthropicConfigurationException("Configuration section '" + section.Path + "' is missing or empty; ApiKey, Model, MaxTokens, TimeoutSeconds, MaxAttempts and RetryBaseDelayMilliseconds are required.", null);
            }

            options.ThrowIfInvalid();
            services.AddSingleton(options);

            services.AddHttpClient(HttpClientName);
            services.AddTransient<ILanguageModel>(provider => new AnthropicLanguageModel(
                provider.GetRequiredService<IHttpMessageHandlerFactory>().CreateHandler(HttpClientName),
                provider.GetRequiredService<AnthropicLanguageModelOptions>(),
                provider.GetRequiredService<ILogger<AnthropicLanguageModel>>()));
            return services;
        }
    }
}
