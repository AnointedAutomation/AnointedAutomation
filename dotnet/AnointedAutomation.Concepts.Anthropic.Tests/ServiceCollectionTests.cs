// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields
using System.Collections.Generic;
using AnointedAutomation.Concepts.Reasoning.Language;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AnointedAutomation.Concepts.Anthropic.Tests
{
    public class ServiceCollectionTests
    {
        private static IConfigurationSection Section(Dictionary<string, string> values)
        {
            return new ConfigurationBuilder().AddInMemoryCollection(values).Build().GetSection("Anthropic");
        }

        private static Dictionary<string, string> Complete()
        {
            return new Dictionary<string, string>
            {
                ["Anthropic:ApiKey"] = TestKit.ApiKey,
                ["Anthropic:Model"] = TestKit.ModelId,
                ["Anthropic:MaxTokens"] = "4096",
                ["Anthropic:TimeoutSeconds"] = "60",
                ["Anthropic:MaxAttempts"] = "3",
                ["Anthropic:RetryBaseDelayMilliseconds"] = "500",
            };
        }

        [Fact]
        public void AddAnthropicLanguageModel_ResolvesTheAdapter_AsTheLanguageModel()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddLogging();
            services.AddAnthropicLanguageModel(Section(Complete()));

            using (ServiceProvider provider = services.BuildServiceProvider())
            {
                ILanguageModel model = provider.GetRequiredService<ILanguageModel>();
                Assert.IsType<AnthropicLanguageModel>(model);
                Assert.Equal("anthropic/" + TestKit.ModelId, model.Name);
            }
        }

        [Fact]
        public void AddAnthropicLanguageModel_FailsFast_WhenTheSectionIsMissing()
        {
            AnthropicConfigurationException failed = Assert.Throws<AnthropicConfigurationException>(
                () => new ServiceCollection().AddAnthropicLanguageModel(Section(new Dictionary<string, string>())));

            Assert.Contains("Anthropic", failed.Message, System.StringComparison.Ordinal);
        }

        [Fact]
        public void AddAnthropicLanguageModel_FailsFast_WhenTheKeyIsMissing()
        {
            Dictionary<string, string> values = Complete();
            values.Remove("Anthropic:ApiKey");

            AnthropicConfigurationException failed = Assert.Throws<AnthropicConfigurationException>(
                () => new ServiceCollection().AddAnthropicLanguageModel(Section(values)));

            Assert.Contains("ApiKey", failed.Message, System.StringComparison.Ordinal);
        }
    }
}
