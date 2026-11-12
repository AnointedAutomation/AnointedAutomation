// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System.Security.Cryptography;
using System.Text;
using AnointedAutomation.Concepts.Reasoning.Language;
using Xunit;

namespace AnointedAutomation.Concepts.Tests.Reasoning.Language
{
    public class PromptComposerTests
    {
        [Theory]
        [InlineData("prompt-abortion-violates-love.txt", false, "prompt-ee05fad796731622")]
        [InlineData("prompt-compassion-upholds-mercy-context.txt", true, "prompt-5ca13e3196f09cd5")]
        public void Compose_MatchesGoldenTextAndId(string golden, bool withContext, string expectedId)
        {
            ComposedPrompt prompt = PromptComposer.Compose(withContext ? LanguageKit.CompassionUpholdsMercyInContext() : LanguageKit.AbortionViolatesLove());

            Assert.Equal(LanguageKit.Golden(golden), prompt.Text);
            Assert.Equal(expectedId, prompt.Id);
        }

        [Fact]
        public void Compose_IdIsTheHashOfTheText()
        {
            ComposedPrompt prompt = PromptComposer.Compose(LanguageKit.AbortionViolatesLove());

            string hex = System.Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(prompt.Text)));
            Assert.Equal("prompt-" + hex.Substring(0, 16), prompt.Id);
        }

        [Fact]
        public void Compose_IsDeterministic()
        {
            Assert.Equal(PromptComposer.Compose(LanguageKit.AbortionViolatesLove()), PromptComposer.Compose(LanguageKit.AbortionViolatesLove()));
        }

        [Fact]
        public void Compose_DifferentQueries_DifferentIds()
        {
            Assert.NotEqual(PromptComposer.Compose(LanguageKit.AbortionViolatesLove()).Id, PromptComposer.Compose(LanguageKit.CompassionUpholdsMercyInContext()).Id);
        }

        [Fact]
        public void Compose_AsksForEveryFieldOnce()
        {
            string text = PromptComposer.Compose(LanguageKit.AbortionViolatesLove()).Text;

            foreach (string field in new[] { "classification", "truth", "confidence", "explanation", "scripture", "sinIds", "assertions" })
            {
                Assert.Contains("\"" + field + "\"", text);
            }

            Assert.Contains("\"additionalProperties\": false", text);
        }

        [Fact]
        public void Compose_NullQuery_Throws()
        {
            Assert.Throws<System.ArgumentNullException>(() => PromptComposer.Compose(null));
        }
    }
}
