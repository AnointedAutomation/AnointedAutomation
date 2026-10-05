// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using AnointedAutomation.Serialization.Json;
using Xunit;

namespace AnointedAutomation.Serialization.Tests
{
    public class JsonElementExtensionsTests
    {
        private static JsonElement P(string json) => JsonDocument.Parse(json).RootElement.Clone();

        // { "v": <json> }
        private static JsonElement V(string valueJson) => P("{\"v\":" + valueJson + "}");

        private static readonly JsonElement NonObject = P("[1,2]");
        private static readonly JsonElement Undefined = default;

        [Fact]
        public void NonObjectReceivers_NeverThrow()
        {
            foreach (JsonElement e in new[] { NonObject, Undefined, P("\"s\""), P("5"), P("null") })
            {
                Assert.Null(e.GetStringOrNull("v"));
                Assert.Equal(string.Empty, e.GetStringOrEmpty("v"));
                Assert.Null(e.GetStringOrRawTextOrNull("v"));
                Assert.Null(e.GetStringOrToString("v"));
                Assert.Equal(string.Empty, e.GetToStringOrEmpty("v"));
                Assert.Null(e.GetInt64CoercedOrNull("v"));
                Assert.Null(e.GetDoubleCoercedOrNull("v"));
                Assert.Null(e.GetDecimalCoercedOrNull("v"));
                Assert.Null(e.GetBoolOrNull("v"));
                Assert.False(e.GetBoolLenient("v"));
                Assert.True(e.GetBoolOrDefault("v", true));
                Assert.False(e.HasProperty("v"));
                Assert.Equal(JsonValueKind.Undefined, e.GetElementOrDefault("v").ValueKind);
                Assert.False(e.TryGetNonNull("v", out _));
                Assert.False(e.TryGetPropertySafe("v", out _));
                Assert.Empty(e.GetStringArray("v"));
                Assert.Empty(e.EnumerateConnection("v"));
                Assert.Null(e.GetUtcDateTimeOrNull("v"));
                Assert.Null(e.GetUnixSecondsOrIsoUtcOrNull("a", "b"));
            }
        }

        // ---------------- strings

        [Theory]
        [InlineData("\"a\"", "a")]
        [InlineData("\" a \"", " a ")]
        [InlineData("\"\"", "")]
        [InlineData("1", null)]
        [InlineData("true", null)]
        [InlineData("null", null)]
        [InlineData("{}", null)]
        public void GetStringOrNull(string json, string expected)
        {
            Assert.Equal(expected, V(json).GetStringOrNull("v"));
            Assert.Equal(expected ?? string.Empty, V(json).GetStringOrEmpty("v"));
            Assert.Null(V(json).GetStringOrNull("missing"));
        }

        [Theory]
        [InlineData("\"a\"", "a", "a", "a")]
        [InlineData("\" \"", " ", null, null)]
        [InlineData("\" a \"", " a ", " a ", "a")]
        [InlineData("\"\"", null, null, null)]
        [InlineData("2", null, null, null)]
        public void EmptyBlankTrimmedVariants(string json, string nonEmpty, string nonBlank, string trimmed)
        {
            Assert.Equal(nonEmpty, V(json).GetNonEmptyStringOrNull("v"));
            Assert.Equal(nonBlank, V(json).GetNonBlankStringOrNull("v"));
            Assert.Equal(trimmed, V(json).GetTrimmedStringOrNull("v"));
        }

        [Theory]
        [InlineData("\"x\"", "x")]
        [InlineData("12", "12")]
        [InlineData("1.50", "1.50")]
        [InlineData("1e3", "1e3")]
        [InlineData("true", null)]
        [InlineData("null", null)]
        [InlineData("[1]", null)]
        public void GetStringOrNumberStringOrNull(string json, string expected)
        {
            Assert.Equal(expected, V(json).GetStringOrNumberStringOrNull("v"));
            Assert.Equal(expected ?? string.Empty, V(json).GetStringOrNumberStringOrEmpty("v"));
        }

        [Theory]
        [InlineData("\" \"", null)]
        [InlineData("\" 1.5 \"", " 1.5 ")]
        [InlineData("-84.3", "-84.3")]
        [InlineData("true", null)]
        public void GetNonBlankStringOrNumberStringOrNull(string json, string expected) =>
            Assert.Equal(expected, V(json).GetNonBlankStringOrNumberStringOrNull("v"));

        [Theory]
        [InlineData("\"x\"", "x")]
        [InlineData("\"\"", "")]
        [InlineData("12.0", "12.0")]
        [InlineData("true", "true")]
        [InlineData("false", "false")]
        [InlineData("{\"a\":1}", "{\"a\":1}")]
        [InlineData("[1,2]", "[1,2]")]
        [InlineData("null", null)]
        public void GetStringOrRawTextOrNull(string json, string expected) =>
            Assert.Equal(expected, V(json).GetStringOrRawTextOrNull("v"));

        [Theory]
        [InlineData("\"x\"", "x")]
        [InlineData("\" \"", " ")]
        [InlineData("\"\"", null)]
        [InlineData("7", "7")]
        [InlineData("true", "true")]
        [InlineData("false", "false")]
        [InlineData("null", null)]
        [InlineData("{}", null)]
        [InlineData("[]", null)]
        public void GetScalarTextOrNull(string json, string expected) =>
            Assert.Equal(expected, V(json).GetScalarTextOrNull("v"));

        [Theory]
        [InlineData("\"x\"", "x")]
        [InlineData("12", "12")]
        [InlineData("true", "True")]
        [InlineData("null", "")]
        [InlineData("{\"a\":1}", "{\"a\":1}")]
        public void GetStringOrToString(string json, string expected)
        {
            Assert.Equal(expected, V(json).GetStringOrToString("v"));
            Assert.Equal(expected, V(json).GetToStringOrEmpty("v"));
            Assert.Null(V(json).GetStringOrToString("missing"));
            Assert.Equal(string.Empty, V(json).GetToStringOrEmpty("missing"));
        }

        [Fact]
        public void GetFirstNonBlankStringOrNull()
        {
            JsonElement e = P("{\"a\":\" \",\"b\":5,\"c\":\" x \",\"d\":\"y\"}");
            Assert.Equal(" x ", e.GetFirstNonBlankStringOrNull("missing", "a", "b", "c", "d"));
            Assert.Null(e.GetFirstNonBlankStringOrNull("a", "b"));
            Assert.Null(e.GetFirstNonBlankStringOrNull(null));
        }

        [Theory]
        [InlineData("\"x\"", "x", "x", "\"x\"")]
        [InlineData("5", "5", "5", "5")]
        [InlineData("true", "true", "True", "true")]
        [InlineData("null", null, "", null)]
        public void ValueLevelStrings(string json, string rawOrNull, string toStr, string rawText)
        {
            JsonElement v = V(json).GetElementOrDefault("v");
            Assert.Equal(rawOrNull, v.AsStringOrRawTextOrNull());
            Assert.Equal(toStr, v.AsStringOrToString());
            Assert.Equal(rawText, v.RawTextOrNull());
            JsonElement? nullable = v;
            Assert.Equal(rawText, nullable.RawTextOrNull());
        }

        [Fact]
        public void ValueLevel_UndefinedAndNoValue()
        {
            Assert.Null(Undefined.AsStringOrRawTextOrNull());
            Assert.Equal(string.Empty, Undefined.AsStringOrToString());
            Assert.Null(Undefined.RawTextOrNull());
            JsonElement? none = null;
            Assert.Null(none.RawTextOrNull());
        }

        // ---------------- booleans

        [Theory]
        [InlineData("true", true, true, true, true)]
        [InlineData("false", false, false, false, false)]
        [InlineData("\"true\"", null, false, true, true)]
        [InlineData("\"TRUE\"", null, false, true, true)]
        [InlineData("\" true \"", null, false, true, false)]
        [InlineData("\"1\"", null, false, null, true)]
        [InlineData("\"yes\"", null, false, null, false)]
        [InlineData("\"0\"", null, false, null, false)]
        [InlineData("1", null, false, null, true)]
        [InlineData("2", null, false, null, true)]
        [InlineData("-1", null, false, null, true)]
        [InlineData("0", null, false, null, false)]
        [InlineData("1.5", null, false, null, false)]
        [InlineData("null", null, false, null, false)]
        public void Booleans(string json, bool? strict, bool orDefault, bool? coerced, bool lenient)
        {
            JsonElement e = V(json);
            Assert.Equal(strict, e.GetBoolOrNull("v"));
            Assert.Equal(orDefault, e.GetBoolOrDefault("v"));
            Assert.Equal(coerced, e.GetBoolCoercedOrNull("v"));
            Assert.Equal(lenient, e.GetBoolLenient("v"));
        }

        [Fact]
        public void GetBoolOrDefault_UsesDefaultForNonBool() =>
            Assert.True(V("\"false\"").GetBoolOrDefault("v", true));

        // ---------------- integers

        [Theory]
        [InlineData("42", 42L, 42L, 42)]
        [InlineData("\"42\"", null, 42L, 42)]
        [InlineData("\" 42 \"", null, 42L, 42)]
        [InlineData("\"-3\"", null, -3L, -3)]
        [InlineData("\"+3\"", null, 3L, 3)]
        [InlineData("\"1,000\"", null, null, null)]
        [InlineData("\"4.0\"", null, null, null)]
        [InlineData("4.0", null, null, null)]
        [InlineData("1.5", null, null, null)]
        [InlineData("1e2", null, null, null)]
        [InlineData("true", null, null, null)]
        [InlineData("null", null, null, null)]
        [InlineData("\"\"", null, null, null)]
        public void Int64Readers(string json, long? strict, long? coerced, int? intCoerced)
        {
            JsonElement e = V(json);
            Assert.Equal(strict, e.GetInt64OrNull("v"));
            Assert.Equal(coerced, e.GetInt64CoercedOrNull("v"));
            Assert.Equal(intCoerced, e.GetIntCoercedOrNull("v"));
        }

        [Fact]
        public void Int64_IsExactAbove2Pow53()
        {
            Assert.Equal(9007199254740993L, V("9007199254740993").GetInt64OrNull("v"));
            Assert.Equal(9223372036854775807L, V("\"9223372036854775807\"").GetInt64CoercedOrNull("v"));
            Assert.Null(V("9223372036854775808").GetInt64OrNull("v"));
            Assert.Null(V("\"9223372036854775808\"").GetInt64CoercedOrNull("v"));
            Assert.Equal("9007199254740993", V("9007199254740993").GetStringOrNumberStringOrNull("v"));
        }

        [Theory]
        [InlineData("5", 5)]
        [InlineData("5.5", null)]
        [InlineData("3000000000", null)]
        [InlineData("\"5\"", null)]
        public void IntStrict(string json, int? expected)
        {
            Assert.Equal(expected, V(json).GetIntOrNull("v"));
            Assert.Equal(expected ?? -1, V(json).GetIntOrDefault("v", -1));
        }

        [Theory]
        [InlineData("123", 123L)]
        [InlineData("-7", -7L)]
        [InlineData("12.9", 12L)]
        [InlineData("-12.9", -12L)]
        [InlineData("1e30", null)]
        [InlineData("\"12abc\"", 12L)]
        [InlineData("\"  -5x\"", -5L)]
        [InlineData("\"+8\"", 8L)]
        [InlineData("\"abc\"", null)]
        [InlineData("\"\"", null)]
        [InlineData("\"-\"", null)]
        [InlineData("\"99999999999999999999\"", null)]
        [InlineData("true", null)]
        [InlineData("null", null)]
        public void JsParseInt(string json, long? expected) =>
            Assert.Equal(expected, V(json).GetInt64JsParseIntOrNull("v"));

        [Fact]
        public void ParseIntLeading_Null() => Assert.Null(JsonElementExtensions.ParseIntLeading(null));

        // ---------------- doubles / decimals

        [Theory]
        [InlineData("1.5", 1.5, 1.5)]
        [InlineData("\"1.5\"", 1.5, null)]
        [InlineData("\" 2 \"", 2.0, null)]
        [InlineData("\"1e3\"", 1000.0, null)]
        [InlineData("\"1,000\"", null, null)]
        [InlineData("\"abc\"", null, null)]
        [InlineData("true", null, null)]
        [InlineData("null", null, null)]
        public void Doubles(string json, double? coerced, double? strict)
        {
            JsonElement e = V(json);
            Assert.Equal(coerced, e.GetDoubleCoercedOrNull("v"));
            Assert.Equal(coerced ?? 0, e.GetDoubleCoercedOr("v"));
            Assert.Equal(coerced ?? 9, e.GetDoubleCoercedOr("v", 9));
            Assert.Equal(strict, e.GetDoubleOrNull("v"));
            Assert.Equal(strict ?? 7, e.GetDoubleOr("v", 7));
            Assert.Equal(coerced.HasValue, e.GetElementOrDefault("v").TryGetDoubleLenient(out double d));
            Assert.Equal(coerced ?? 0, d);
        }

        [Fact]
        public void Doubles_StylesAreHonored() =>
            Assert.Equal(1000.0, V("\"1,000\"").GetDoubleCoercedOrNull("v", NumberStyles.Float | NumberStyles.AllowThousands));

        [Theory]
        [InlineData("19.99", "19.99", "19.99", "19.99")]
        [InlineData("\"19.99\"", null, "19.99", "19.99")]
        [InlineData("\"1,234.50\"", null, "1234.50", "1234.50")]
        [InlineData("\"$5\"", null, null, null)]
        [InlineData("\"(5)\"", null, null, "-5")]
        [InlineData("\"1e2\"", null, null, "100")]
        [InlineData("\" \"", null, null, null)]
        [InlineData("1e400", null, null, null)]
        [InlineData("true", null, null, null)]
        public void Decimals(string json, string strict, string number, string any)
        {
            JsonElement e = V(json);
            Assert.Equal(strict == null ? (decimal?)null : decimal.Parse(strict, CultureInfo.InvariantCulture), e.GetDecimalOrNull("v"));
            Assert.Equal(number == null ? (decimal?)null : decimal.Parse(number, CultureInfo.InvariantCulture), e.GetDecimalCoercedOrNull("v"));
            Assert.Equal(any == null ? (decimal?)null : decimal.Parse(any, CultureInfo.InvariantCulture), e.GetDecimalCoercedOrNull("v", NumberStyles.Any));
        }

        // ---------------- dates

        [Theory]
        [InlineData("\"2026-01-02T03:04:05Z\"", "2026-01-02T03:04:05")]
        [InlineData("\"2026-01-02T03:04:05-05:00\"", "2026-01-02T08:04:05")]
        [InlineData("\"2026-01-02\"", "2026-01-02T00:00:00")]
        [InlineData("\"garbage\"", null)]
        [InlineData("1700000000", null)]
        public void UtcDate(string json, string expected)
        {
            DateTime? d = V(json).GetUtcDateTimeOrNull("v");
            if (expected == null)
            {
                Assert.Null(d);
                return;
            }

            Assert.Equal(DateTimeKind.Utc, d.Value.Kind);
            Assert.Equal(DateTime.Parse(expected, CultureInfo.InvariantCulture), d.Value);
        }

        [Fact]
        public void UnixSecondsFirst()
        {
            DateTime? fromSecs = P("{\"created\":1700000000,\"created_at\":\"2020-01-01T00:00:00Z\"}").GetUnixSecondsOrIsoUtcOrNull("created", "created_at");
            Assert.Equal(new DateTime(2023, 11, 14, 22, 13, 20, DateTimeKind.Utc), fromSecs);
            Assert.Equal(DateTimeKind.Utc, fromSecs.Value.Kind);

            DateTime? fromIso = P("{\"created\":\"1700000000\",\"created_at\":\"2020-01-01T05:00:00+05:00\"}").GetUnixSecondsOrIsoUtcOrNull("created", "created_at");
            Assert.Equal(new DateTime(2020, 1, 1, 0, 0, 0), fromIso);
            Assert.Equal(DateTimeKind.Utc, fromIso.Value.Kind);

            Assert.Null(P("{\"created\":1.5,\"created_at\":\"nope\"}").GetUnixSecondsOrIsoUtcOrNull("created", "created_at"));
            Assert.Null(P("{\"created\":99999999999999}").GetUnixSecondsOrIsoUtcOrNull("created", "created_at"));
        }

        // ---------------- elements

        [Fact]
        public void Elements()
        {
            JsonElement e = P("{\"a\":{\"b\":{\"c\":\"deep\"}},\"n\":null,\"arr\":[\"x\",1,\"y\",null]}");
            Assert.Equal("deep", e.GetPathOrDefault("a", "b").GetStringOrNull("c"));
            Assert.Equal("deep", e.GetPathOrDefault("a", "b", "c").GetString());
            Assert.Equal(JsonValueKind.Undefined, e.GetPathOrDefault("a", "zz", "c").ValueKind);
            Assert.Equal(JsonValueKind.Object, e.GetPathOrDefault(null).ValueKind);
            Assert.True(e.HasProperty("n"));
            Assert.True(e.TryGetPropertySafe("n", out JsonElement n));
            Assert.Equal(JsonValueKind.Null, n.ValueKind);
            Assert.False(e.TryGetNonNull("n", out JsonElement nn));
            Assert.Equal(JsonValueKind.Undefined, nn.ValueKind);
            Assert.True(e.TryGetNonNull("a", out _));
            Assert.Equal(new List<string> { "x", "y" }, e.GetStringArray("arr"));
            Assert.False(e.TryGetArray("a", out _));
        }

        [Fact]
        public void IgnoreCaseLookup()
        {
            JsonElement e = P("{\"Data\":1,\"data\":2}");
            Assert.True(e.TryGetPropertyIgnoreCase("DATA", out JsonElement v));
            Assert.Equal(1, v.GetInt32());
            Assert.False(e.TryGetPropertyIgnoreCase("nope", out JsonElement miss));
            Assert.Equal(JsonValueKind.Undefined, miss.ValueKind);
            Assert.False(NonObject.TryGetPropertyIgnoreCase("Data", out _));
        }

        [Fact]
        public void Connections()
        {
            JsonElement nodes = P("{\"c\":{\"nodes\":[{\"id\":1},{\"id\":2}]}}");
            JsonElement edges = P("{\"c\":{\"edges\":[{\"node\":{\"id\":3}},{\"cursor\":\"x\"},5]}}");
            Assert.Equal(new long?[] { 1, 2 }, nodes.EnumerateConnection("c").Select(x => x.GetInt64OrNull("id")).ToArray());
            Assert.Equal(new long?[] { 3 }, edges.EnumerateConnection("c").Select(x => x.GetInt64OrNull("id")).ToArray());
            Assert.Empty(P("{\"c\":5}").EnumerateConnection("c"));
            Assert.Empty(P("{\"c\":{}}").EnumerateConnection("c"));
            Assert.Empty(NonObject.EnumerateConnectionNodes());
        }

        // ---------------- ResponseJson

        [Fact]
        public void ResponseJson_Parses()
        {
            Assert.Null(ResponseJson.TryParse(null));
            Assert.Null(ResponseJson.TryParse("   "));
            Assert.Null(ResponseJson.TryParse("Service Unavailable"));
            using (JsonDocument d = ResponseJson.TryParse("{\"a\":1}"))
            {
                Assert.Equal(1L, d.RootElement.GetInt64OrNull("a"));
            }

            Assert.True(ResponseJson.IsNotJson("<html>"));
            Assert.False(ResponseJson.IsNotJson("[]"));
            Assert.Equal("(empty response)", ResponseJson.Snippet(" "));
            Assert.Equal("a b", ResponseJson.Snippet(" a\nb "));
            string longBody = new string('x', 300);
            Assert.Equal(new string('x', ResponseJson.SnippetLength) + "...", ResponseJson.Snippet(longBody));
        }
    }
}
