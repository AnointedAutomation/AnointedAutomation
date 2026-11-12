// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System.Linq;
using AnointedAutomation.Concepts.Reasoning;
using AnointedAutomation.Concepts.Reasoning.Language;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AnointedAutomation.Concepts.Tests.Reasoning.Language
{
    public class InterpretationParserTests
    {
        [Fact]
        public void Parse_ValidReply_Accepted()
        {
            var logger = new LanguageKit.CapturingLogger<InterpretationParser>();

            Interpretation result = LanguageKit.Parser(logger).Parse(LanguageKit.Golden("reply-valid.json"), "prompt-x", LanguageKit.ModelName);

            Assert.Equal(TruthValue.True, result.Truth);
            Assert.Equal(0.9, result.Confidence);
            Assert.Equal(new[] { ConceptRef.Of(SinId.Abortion), new ConceptRef(ConceptRef.MoralConceptKind, "Murder") }, result.Classification);
            Assert.Equal(new[] { "Exodus 21:22-25", "Jeremiah 1:5", "Psalms 139:13" }, result.Scripture.Select(s => s.ToString()));
            Assert.Equal(new[] { SinId.Abortion }, result.SinIds);
            Assert.Equal(new Assertion(ConceptRef.Of(SinId.Abortion), RelationKind.Violates, new ConceptRef(CanonFacts.AttributeKind, "Love")), Assert.Single(result.Assertions));
            Assert.Empty(logger.Entries);
        }

        [Theory]
        [InlineData("reply-bad-truth.json", "truth must be")]
        [InlineData("reply-confidence-out-of-range.json", "confidence must be")]
        [InlineData("reply-invented-verse.json", "John has 21 chapters")]
        [InlineData("reply-unknown-sin.json", "Gossiping About Elves")]
        [InlineData("reply-extra-field.json", "unknown field 'mood'")]
        [InlineData("reply-malformed.json", "malformed JSON")]
        public void Parse_BadReply_RejectedAndLoggedInFull(string fixture, string expectedReason)
        {
            var logger = new LanguageKit.CapturingLogger<InterpretationParser>();
            string reply = LanguageKit.Golden(fixture);

            InterpretationRejected rejected = Assert.Throws<InterpretationRejected>(() => LanguageKit.Parser(logger).Parse(reply, "prompt-x", LanguageKit.ModelName));

            Assert.Equal(reply, rejected.Reply);
            Assert.Contains(expectedReason, rejected.Reason);
            var entry = Assert.Single(logger.Entries);
            Assert.Equal(LogLevel.Error, entry.Level);
            Assert.Same(rejected, entry.Exception);
            Assert.Contains(reply, entry.Message);
            Assert.Contains("prompt-x", entry.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        [InlineData("[]")]
        [InlineData("{\"truth\":\"true\"}")]
        public void Parse_EmptyOrWrongShape_Rejected(string reply)
        {
            var logger = new LanguageKit.CapturingLogger<InterpretationParser>();

            Assert.Throws<InterpretationRejected>(() => LanguageKit.Parser(logger).Parse(reply, "prompt-x", LanguageKit.ModelName));
            Assert.Single(logger.Entries);
        }

        [Theory]
        [InlineData("\"confidence\": 0.9", "\"confidence\": -0.1")]
        [InlineData("\"confidence\": 0.9", "\"confidence\": \"0.9\"")]
        [InlineData("\"truth\": \"true\"", "\"truth\": \"True\"")]
        [InlineData("\"truth\": \"true\"", "\"truth\": true")]
        [InlineData("\"Psalms 139:13\"", "\"Hezekiah 3:16\"")]
        [InlineData("\"Psalms 139:13\"", "\"Psalms 139\"")]
        [InlineData("\"sinIds\": [\"Abortion\"]", "\"sinIds\": [\"1\"]")]
        [InlineData("\"sinIds\": [\"Abortion\"]", "\"sinIds\": [\"Omitted367\"]")]
        [InlineData("\"sinIds\": [\"Abortion\"]", "\"sinIds\": [\"abortion\"]")]
        [InlineData("{\"kind\": \"Sin\", \"id\": \"Abortion\"}, {", "{\"kind\": \"Sin\", \"id\": \"NotASin\"}, {")]
        [InlineData("\"relation\": \"Violates\"", "\"relation\": \"None\"")]
        [InlineData("\"relation\": \"Violates\"", "\"relation\": \"Loves\"")]
        [InlineData("\"id\": \"Love\"}", "\"id\": \"Love\", \"weight\": 2}")]
        [InlineData("\"explanation\": \"Taking", "\"explanation\": \"\", \"x\": \"Taking")]
        [InlineData("\"truth\": \"true\",", "\"truth\": \"true\", \"truth\": \"false\",")]
        public void Parse_EachViolation_Rejected(string find, string replace)
        {
            var logger = new LanguageKit.CapturingLogger<InterpretationParser>();
            string valid = LanguageKit.Golden("reply-valid.json");
            Assert.Contains(find, valid);

            Assert.Throws<InterpretationRejected>(() => LanguageKit.Parser(logger).Parse(valid.Replace(find, replace), "prompt-x", LanguageKit.ModelName));
            Assert.Single(logger.Entries);
        }

        [Theory]
        [InlineData("\"truth\": \"true\"", "\"truth\": \"unknown\"", TruthValue.Unknown)]
        [InlineData("\"truth\": \"true\"", "\"truth\": \"false\"", TruthValue.False)]
        public void Parse_OtherTruths_Accepted(string find, string replace, TruthValue expected)
        {
            var logger = new LanguageKit.CapturingLogger<InterpretationParser>();

            Interpretation result = LanguageKit.Parser(logger).Parse(LanguageKit.Golden("reply-valid.json").Replace(find, replace), "prompt-x", LanguageKit.ModelName);

            Assert.Equal(expected, result.Truth);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("1")]
        public void Parse_ConfidenceBounds_Accepted(string value)
        {
            var logger = new LanguageKit.CapturingLogger<InterpretationParser>();

            Interpretation result = LanguageKit.Parser(logger).Parse(LanguageKit.Golden("reply-valid.json").Replace("0.9", value), "prompt-x", LanguageKit.ModelName);

            Assert.Equal(double.Parse(value, System.Globalization.CultureInfo.InvariantCulture), result.Confidence);
        }

        [Fact]
        public void Constructor_NullLogger_Throws()
        {
            Assert.Throws<System.ArgumentNullException>(() => new InterpretationParser(null));
        }
    }
}
