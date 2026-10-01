// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// The Objects.API types are back-compat shims over AnointedAutomation.Serialization; they must behave identically.

using System;
using System.Text.Json;
using AnointedAutomation.Objects.API;
using Xunit;

namespace AnointedAutomation.Objects.API.Tests
{
    public class BackCompatShimTests
    {
        private class Dto
        {
            public string Name { get; set; }
            public int Count { get; set; }
            public DateTime At { get; set; }
        }

        [Fact]
        public void Options_IsTheSerializationInstance() =>
            Assert.Same(AnointedAutomation.Serialization.SystemTextJson.JsonCasingConvention.Options, JsonCasingConvention.Options);

        [Fact]
        public void Configure_ProducesTheSameWire()
        {
            JsonSerializerOptions legacy = new JsonSerializerOptions();
            JsonCasingConvention.Configure(legacy);
            Dto dto = new Dto { Name = "n", Count = 1, At = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Unspecified) };
            Assert.Equal("{\"Name\":\"n\",\"count\":1,\"at\":\"2026-01-01T00:00:00Z\"}", JsonSerializer.Serialize(dto, legacy));
            Assert.Equal(JsonSerializer.Serialize(dto, AnointedAutomation.Serialization.SystemTextJson.JsonCasingConvention.Options), JsonSerializer.Serialize(dto, legacy));
        }

        [Fact]
        public void LegacyConverter_Delegates()
        {
            JsonSerializerOptions o = new JsonSerializerOptions();
            o.Converters.Add(new UtcDateTimeJsonConverter());
            Assert.Equal("\"2026-01-01T00:00:00Z\"", JsonSerializer.Serialize(new DateTime(2026, 1, 1), o));
            Assert.Equal(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), JsonSerializer.Deserialize<DateTime>("\"2026-01-01T00:00:00Z\"", o));
        }
    }
}
