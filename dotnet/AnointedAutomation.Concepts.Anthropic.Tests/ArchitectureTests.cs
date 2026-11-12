// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields
using System;
using System.Linq;
using System.Reflection;
using AnointedAutomation.Concepts.Reasoning.Language;
using Xunit;

namespace AnointedAutomation.Concepts.Anthropic.Tests
{
    /// <summary>
    /// The dependency points one way: the adapter depends on the core, never the core on the adapter or an AI SDK.
    /// </summary>
    public class ArchitectureTests
    {
        [Fact]
        public void Core_DoesNotReference_TheAdapterOrTheAnthropicSdk()
        {
            Assembly core = typeof(ILanguageModel).Assembly;
            string[] references = core.GetReferencedAssemblies().Select(a => a.Name).ToArray();

            Assert.DoesNotContain(typeof(AnthropicLanguageModel).Assembly.GetName().Name, references);
            Assert.DoesNotContain(typeof(global::Anthropic.AnthropicClient).Assembly.GetName().Name, references);
            Assert.DoesNotContain(references, name => name.StartsWith("Anthropic", StringComparison.Ordinal));
        }

        [Fact]
        public void Adapter_References_TheCore()
        {
            string[] references = typeof(AnthropicLanguageModel).Assembly.GetReferencedAssemblies().Select(a => a.Name).ToArray();

            Assert.Contains(typeof(ILanguageModel).Assembly.GetName().Name, references);
        }
    }
}
