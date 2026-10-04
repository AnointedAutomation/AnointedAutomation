// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using AnointedAutomation.Serialization.SystemTextJson;
using Xunit;

namespace AnointedAutomation.Serialization.Tests
{
    public enum Color
    {
        DarkRed,
        Blue,
    }

    public class Widget
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public long? OrderId { get; set; }
        public Color Shade { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Tags { get; set; }
        public Point Origin { get; set; }

        [JsonPropertyName("custom_name")]
        public string Custom { get; set; }

        public string URLValue { get; set; }
        public int IDNumber { get; set; }
    }

    public struct Point
    {
        public int X { get; set; }
        public string Label { get; set; }
    }

    public class SnakeDto
    {
        public string FirstName { get; set; }
        public long? OrderId { get; set; }
        public string Missing { get; set; }
    }

    public class SystemTextJsonTests
    {
        private static Widget Sample() => new Widget
        {
            Name = "n",
            Count = 2,
            OrderId = 5,
            Shade = Color.DarkRed,
            CreatedAt = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Unspecified),
            Tags = new List<string> { "t" },
            Origin = new Point { X = 1, Label = "l" },
            Custom = "c",
            URLValue = "u",
            IDNumber = 7,
        };

        [Fact]
        public void Convention_AppliesHybridRule()
        {
            string json = JsonSerializer.Serialize(Sample(), JsonCasingConvention.Options);
            Assert.Equal(
                "{\"Name\":\"n\",\"count\":2,\"orderId\":5,\"shade\":\"darkRed\",\"createdAt\":\"2026-01-02T03:04:05Z\",\"Tags\":[\"t\"],\"origin\":{\"x\":1,\"label\":\"l\"},\"custom_name\":\"c\",\"URLValue\":\"u\",\"idNumber\":7}",
                json);
        }

        [Fact]
        public void Convention_SkipsAnonymousTypes()
        {
            string json = JsonSerializer.Serialize(new { snake_key = 1, Pascal = "p" }, JsonCasingConvention.Options);
            Assert.Equal("{\"snake_key\":1,\"Pascal\":\"p\"}", json);
        }

        [Fact]
        public void ConfigureApi_MatchesConventionAndReadsNumbersFromStrings()
        {
            JsonSerializerOptions a = new JsonSerializerOptions();
            AnointedJson.ConfigureApi(a);
            JsonSerializerOptions b = new JsonSerializerOptions();
            AnointedJson.ConfigureApi(b);

            Assert.Null(a.PropertyNamingPolicy);
            Assert.Null(a.DictionaryKeyPolicy);
            Assert.Equal(JsonNumberHandling.AllowReadingFromString, a.NumberHandling);
            Assert.NotSame(a.TypeInfoResolver, b.TypeInfoResolver);
            Assert.Equal(JsonSerializer.Serialize(Sample(), JsonCasingConvention.Options), JsonSerializer.Serialize(Sample(), a));

            Widget read = JsonSerializer.Deserialize<Widget>("{\"count\":\"12\",\"orderId\":\"99\"}", a);
            Assert.Equal(12, read.Count);
            Assert.Equal(99L, read.OrderId);
        }

        [Fact]
        public void ConfigureApi_WorksAfterDefaultResolverWasUsed()
        {
            JsonSerializer.Serialize(Sample());
            JsonSerializerOptions o = new JsonSerializerOptions();
            AnointedJson.ConfigureApi(o);
            Assert.Contains("\"count\":2", JsonSerializer.Serialize(Sample(), o));
            Assert.Throws<ArgumentNullException>(() => AnointedJson.ConfigureApi(null));
        }

        [Theory]
        [InlineData(DateTimeKind.Utc)]
        [InlineData(DateTimeKind.Unspecified)]
        public void UtcConverter_WritesZ(DateTimeKind kind)
        {
            DateTime value = new DateTime(2026, 3, 4, 5, 6, 7, 120, kind);
            string json = JsonSerializer.Serialize(value, JsonCasingConvention.Options);
            Assert.Equal("\"2026-03-04T05:06:07.12Z\"", json);
            Assert.Equal(DateTimeKind.Utc, UtcDateTimeJsonConverter.ToUtc(value).Kind);
        }

        [Fact]
        public void UtcConverter_ConvertsLocal()
        {
            DateTime local = new DateTime(2026, 3, 4, 5, 6, 7, DateTimeKind.Local);
            Assert.Equal(local.ToUniversalTime(), UtcDateTimeJsonConverter.ToUtc(local));
        }

        [Fact]
        public void Presets_HaveExactSettings()
        {
            Assert.Same(JsonNamingPolicy.SnakeCaseLower, JsonPresets.SnakeCase.PropertyNamingPolicy);
            Assert.True(JsonPresets.SnakeCase.PropertyNameCaseInsensitive);
            Assert.Equal(JsonNumberHandling.Strict, JsonPresets.SnakeCase.NumberHandling);

            Assert.Same(JsonNamingPolicy.SnakeCaseLower, JsonPresets.SnakeCaseLenient.PropertyNamingPolicy);
            Assert.True(JsonPresets.SnakeCaseLenient.PropertyNameCaseInsensitive);
            Assert.Equal(JsonNumberHandling.AllowReadingFromString, JsonPresets.SnakeCaseLenient.NumberHandling);

            Assert.Same(JsonNamingPolicy.SnakeCaseLower, JsonPresets.SnakeCaseWriteNulls.PropertyNamingPolicy);
            Assert.False(JsonPresets.SnakeCaseWriteNulls.PropertyNameCaseInsensitive);
            Assert.Equal(JsonIgnoreCondition.Never, JsonPresets.SnakeCaseWriteNulls.DefaultIgnoreCondition);

            Assert.Same(JsonNamingPolicy.CamelCase, JsonPresets.CamelCase.PropertyNamingPolicy);
            Assert.False(JsonPresets.CamelCase.PropertyNameCaseInsensitive);

            Assert.Same(JsonNamingPolicy.CamelCase, JsonPresets.CamelCaseInsensitive.PropertyNamingPolicy);
            Assert.True(JsonPresets.CamelCaseInsensitive.PropertyNameCaseInsensitive);

            Assert.Null(JsonPresets.CaseInsensitive.PropertyNamingPolicy);
            Assert.True(JsonPresets.CaseInsensitive.PropertyNameCaseInsensitive);

            Assert.True(JsonPresets.CaseInsensitiveLenient.PropertyNameCaseInsensitive);
            Assert.Equal(JsonNumberHandling.AllowReadingFromString, JsonPresets.CaseInsensitiveLenient.NumberHandling);

            Assert.Equal(JsonIgnoreCondition.WhenWritingNull, JsonPresets.IgnoreNulls.DefaultIgnoreCondition);
            Assert.Null(JsonPresets.IgnoreNulls.ReferenceHandler);

            Assert.Equal(JsonIgnoreCondition.WhenWritingNull, JsonPresets.IgnoreNullsAndCycles.DefaultIgnoreCondition);
            Assert.Same(ReferenceHandler.IgnoreCycles, JsonPresets.IgnoreNullsAndCycles.ReferenceHandler);

            Assert.Same(JsonCasingConvention.Options, JsonPresets.Api);
        }

        [Fact]
        public void Presets_AreReadOnly_ButCopyable()
        {
            Assert.True(JsonPresets.CamelCase.IsReadOnly);
            Assert.Throws<InvalidOperationException>(() => JsonPresets.CamelCase.WriteIndented = true);
            JsonSerializerOptions copy = new JsonSerializerOptions(JsonPresets.CamelCase) { WriteIndented = true };
            Assert.True(copy.WriteIndented);
        }

        [Fact]
        public void Presets_SnakeCase_RoundTrips()
        {
            SnakeDto dto = JsonSerializer.Deserialize<SnakeDto>("{\"FIRST_NAME\":\"a\",\"order_id\":5}", JsonPresets.SnakeCase);
            Assert.Equal("a", dto.FirstName);
            Assert.Equal(5L, dto.OrderId);
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<SnakeDto>("{\"order_id\":\"5\"}", JsonPresets.SnakeCase));
            Assert.Equal(5L, JsonSerializer.Deserialize<SnakeDto>("{\"order_id\":\"5\"}", JsonPresets.SnakeCaseLenient).OrderId);
            Assert.Equal("{\"first_name\":\"a\",\"order_id\":null,\"missing\":null}",
                JsonSerializer.Serialize(new SnakeDto { FirstName = "a" }, JsonPresets.SnakeCaseWriteNulls));
            Assert.Equal("{\"FirstName\":\"a\"}", JsonSerializer.Serialize(new SnakeDto { FirstName = "a" }, JsonPresets.IgnoreNulls));
        }
    }
}
