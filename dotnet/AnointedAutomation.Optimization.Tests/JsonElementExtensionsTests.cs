// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Copyright 2026 Anointed Automation, LLC. All Rights Reserved.
// Ported from the API repo's NUnit suite to xUnit.

using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using AnointedAutomation.Optimization;
using Xunit;

namespace AnointedAutomation.Optimization.Tests
{
    public class JsonElementExtensionsTests
    {
        private static JsonElement Parse(string json) => JsonDocument.Parse(json).RootElement.Clone();

        private static JsonElement Sample() => Parse(
            "{\"name\":\"widget\",\"blank\":\"\",\"count\":7,\"priceText\":\"12.5\",\"grouped\":\"1,200.75\"," +
            "\"nothing\":null,\"child\":{\"value\":\"inner\"},\"tags\":[\"a\",2,\"b\"],\"notArray\":\"x\"}");

        [Fact]
        public void GetStringOrNull_StrictString_ElseNull()
        {
            JsonElement e = Sample();
            Assert.Equal("widget", e.GetStringOrNull("name"));
            Assert.Equal(string.Empty, e.GetStringOrNull("blank"));
            Assert.Null(e.GetStringOrNull("count"));
            Assert.Null(e.GetStringOrNull("nothing"));
            Assert.Null(e.GetStringOrNull("absent"));
        }

        [Fact]
        public void GetStringOrNull_NonObjectReceiver_ReturnsNullInsteadOfThrowing()
        {
            Assert.Null(Parse("[1,2]").GetStringOrNull("name"));
            Assert.Null(default(JsonElement).GetStringOrNull("name"));
        }

        [Fact]
        public void GetNonEmptyStringOrNull_CollapsesEmptyToNull()
        {
            JsonElement e = Sample();
            Assert.Equal("widget", e.GetNonEmptyStringOrNull("name"));
            Assert.Null(e.GetNonEmptyStringOrNull("blank"));
            Assert.Null(e.GetNonEmptyStringOrNull("absent"));
        }

        [Fact]
        public void GetStringOrEmpty_NeverNull()
        {
            JsonElement e = Sample();
            Assert.Equal("widget", e.GetStringOrEmpty("name"));
            Assert.Equal(string.Empty, e.GetStringOrEmpty("blank"));
            Assert.Equal(string.Empty, e.GetStringOrEmpty("count"));
            Assert.Equal(string.Empty, e.GetStringOrEmpty("absent"));
        }

        [Fact]
        public void GetStringOrNumberStringOrNull_CoercesNumberToString()
        {
            JsonElement e = Sample();
            Assert.Equal("widget", e.GetStringOrNumberStringOrNull("name"));
            Assert.Equal("7", e.GetStringOrNumberStringOrNull("count"));
            Assert.Null(e.GetStringOrNumberStringOrNull("nothing"));
            Assert.Null(e.GetStringOrNumberStringOrNull("absent"));
        }

        [Fact]
        public void HasProperty_TrueOnlyForPresentProperties()
        {
            JsonElement e = Sample();
            Assert.True(e.HasProperty("name"));
            Assert.True(e.HasProperty("nothing"));
            Assert.False(e.HasProperty("absent"));
            Assert.False(Parse("[1]").HasProperty("name"));
        }

        [Fact]
        public void GetElementOrDefault_ChainsAndFallsBackToUndefined()
        {
            JsonElement e = Sample();
            Assert.Equal("inner", e.GetElementOrDefault("child").GetStringOrNull("value"));
            Assert.Equal(JsonValueKind.Undefined, e.GetElementOrDefault("absent").ValueKind);
            Assert.Null(e.GetElementOrDefault("absent").GetStringOrNull("value"));
        }

        [Fact]
        public void TryGetArray_OnlyMatchesArrays()
        {
            JsonElement e = Sample();
            Assert.True(e.TryGetArray("tags", out JsonElement arr));
            Assert.Equal(3, arr.GetArrayLength());
            Assert.False(e.TryGetArray("notArray", out JsonElement none));
            Assert.Equal(JsonValueKind.Undefined, none.ValueKind);
        }

        [Fact]
        public void GetStringArray_SkipsNonStringMembers()
        {
            JsonElement e = Sample();
            Assert.Equal(new List<string> { "a", "b" }, e.GetStringArray("tags"));
            Assert.Empty(e.GetStringArray("notArray"));
            Assert.Empty(e.GetStringArray("absent"));
        }

        [Fact]
        public void GetDoubleCoercedOrNull_AcceptsNumbersAndNumericStrings()
        {
            JsonElement e = Sample();
            Assert.Equal(7d, e.GetDoubleCoercedOrNull("count"));
            Assert.Equal(12.5d, e.GetDoubleCoercedOrNull("priceText"));
            Assert.Null(e.GetDoubleCoercedOrNull("name"));
            Assert.Null(e.GetDoubleCoercedOrNull("absent"));
        }

        [Fact]
        public void GetDoubleCoercedOrNull_StylesControlThousandsSeparators()
        {
            JsonElement e = Sample();
            Assert.Null(e.GetDoubleCoercedOrNull("grouped"));
            Assert.Equal(1200.75d, e.GetDoubleCoercedOrNull("grouped", NumberStyles.Any));
        }

        [Fact]
        public void StrictGetDoubleOrNull_DoesNotCoerceStrings()
        {
            JsonElement e = Sample();
            Assert.Equal(7d, e.GetDoubleOrNull("count"));
            Assert.Null(e.GetDoubleOrNull("priceText"));
        }

        [Fact]
        public void GetIntGetters_ReadNumbersOrFallBack()
        {
            JsonElement e = Sample();
            Assert.Equal(7, e.GetIntOrNull("count"));
            Assert.Null(e.GetIntOrNull("name"));
            Assert.Equal(7, e.GetIntOrDefault("count", -1));
            Assert.Equal(-1, e.GetIntOrDefault("name", -1));
        }

        [Fact]
        public void GetBoolOrDefault_TrueFalseElseDefault_NoStringCoercion()
        {
            JsonElement e = Parse("{\"yes\":true,\"no\":false,\"strTrue\":\"true\",\"num\":1}");
            Assert.True(e.GetBoolOrDefault("yes"));
            Assert.False(e.GetBoolOrDefault("no", true));
            Assert.False(e.GetBoolOrDefault("strTrue"));
            Assert.True(e.GetBoolOrDefault("num", true));
            Assert.True(e.GetBoolOrDefault("absent", true));
        }

        [Fact]
        public void EnumerateConnection_HandlesEdgesAndNodes()
        {
            JsonElement edges = Parse("{\"conn\":{\"edges\":[{\"node\":{\"id\":\"a\"}},{\"node\":{\"id\":\"b\"}}]}}");
            List<string?> fromEdges = new List<string?>();
            foreach (JsonElement n in edges.EnumerateConnection("conn"))
            {
                fromEdges.Add(n.GetStringOrNull("id"));
            }
            Assert.Equal(new List<string?> { "a", "b" }, fromEdges);

            JsonElement nodes = Parse("{\"conn\":{\"nodes\":[{\"id\":\"c\"}]}}");
            List<string?> fromNodes = new List<string?>();
            foreach (JsonElement n in nodes.EnumerateConnection("conn"))
            {
                fromNodes.Add(n.GetStringOrNull("id"));
            }
            Assert.Equal(new List<string?> { "c" }, fromNodes);

            Assert.Empty(new List<JsonElement>(Sample().EnumerateConnection("absent")));
        }
    }
}
