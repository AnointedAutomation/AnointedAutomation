// Copyright 2026 Anointed Automation, LLC. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
using System;
using System.Text.Json;
using Xunit;

namespace AnointedAutomation.Objects.API.Tests
{
    /// <summary>
    /// Unit tests for UtcDateTimeJsonConverter and its registration in JsonCasingConvention.
    /// </summary>
    public class UtcDateTimeJsonConverterTests
    {
        private sealed class Stamped
        {
            public DateTime CreatedAt { get; set; }
            public DateTime? DeletedAt { get; set; }
            public string Name { get; set; } = "x";
        }

        #region Write

        [Fact]
        public void Write_UtcKind_EndsInZ()
        {
            DateTime value = new DateTime(2026, 9, 1, 14, 30, 0, DateTimeKind.Utc);

            string json = JsonSerializer.Serialize(value, JsonCasingConvention.Options);

            Assert.Equal("\"2026-09-01T14:30:00Z\"", json);
        }

        [Fact]
        public void Write_UnspecifiedKind_TreatedAsUtcAndEndsInZ()
        {
            DateTime value = new DateTime(2026, 9, 1, 14, 30, 0, DateTimeKind.Unspecified);

            string json = JsonSerializer.Serialize(value, JsonCasingConvention.Options);

            Assert.Equal("\"2026-09-01T14:30:00Z\"", json);
        }

        [Fact]
        public void Write_LocalKind_ConvertedToUtc()
        {
            DateTime utc = new DateTime(2026, 9, 1, 14, 30, 0, DateTimeKind.Utc);
            DateTime local = utc.ToLocalTime();

            string json = JsonSerializer.Serialize(local, JsonCasingConvention.Options);

            Assert.Equal("\"2026-09-01T14:30:00Z\"", json);
        }

        [Fact]
        public void Write_FractionalSeconds_MatchesDefaultUtcFormatting()
        {
            DateTime value = new DateTime(2026, 9, 1, 14, 30, 0, 123, DateTimeKind.Utc);

            string converted = JsonSerializer.Serialize(value, JsonCasingConvention.Options);
            string defaultJson = JsonSerializer.Serialize(value);

            Assert.Equal(defaultJson, converted);
        }

        [Fact]
        public void Write_NullableValueAndNull_BothHandled()
        {
            Stamped stamped = new Stamped
            {
                CreatedAt = new DateTime(2026, 9, 1, 14, 30, 0, DateTimeKind.Unspecified),
                DeletedAt = null,
            };

            string json = JsonSerializer.Serialize(stamped, JsonCasingConvention.Options);

            Assert.Contains("\"createdAt\":\"2026-09-01T14:30:00Z\"", json);
            Assert.Contains("\"deletedAt\":null", json);
        }

        [Fact]
        public void Write_DoesNotChangePropertyCasing()
        {
            string json = JsonSerializer.Serialize(new Stamped(), JsonCasingConvention.Options);

            Assert.Contains("\"createdAt\"", json);
            Assert.Contains("\"Name\"", json);
        }

        #endregion

        #region Read

        [Fact]
        public void Read_ZSuffixed_ReturnsUtcKind()
        {
            DateTime value = JsonSerializer.Deserialize<DateTime>("\"2026-09-01T14:30:00Z\"", JsonCasingConvention.Options);

            Assert.Equal(DateTimeKind.Utc, value.Kind);
            Assert.Equal(new DateTime(2026, 9, 1, 14, 30, 0, DateTimeKind.Utc), value);
        }

        [Fact]
        public void Read_NoDesignator_KeepsDefaultBehavior()
        {
            DateTime converted = JsonSerializer.Deserialize<DateTime>("\"2026-09-01T14:30:00\"", JsonCasingConvention.Options);
            DateTime defaultParse = JsonSerializer.Deserialize<DateTime>("\"2026-09-01T14:30:00\"");

            Assert.Equal(defaultParse, converted);
            Assert.Equal(defaultParse.Kind, converted.Kind);
        }

        [Fact]
        public void Read_Malformed_Throws()
        {
            Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<DateTime>("\"not a date\"", JsonCasingConvention.Options));
        }

        #endregion

        #region ToUtc

        [Theory]
        [InlineData(DateTimeKind.Utc)]
        [InlineData(DateTimeKind.Unspecified)]
        public void ToUtc_UtcOrUnspecified_KeepsWallClockAndSetsUtc(DateTimeKind kind)
        {
            DateTime value = new DateTime(2026, 1, 2, 3, 4, 5, kind);

            DateTime result = UtcDateTimeJsonConverter.ToUtc(value);

            Assert.Equal(DateTimeKind.Utc, result.Kind);
            Assert.Equal(value.Ticks, result.Ticks);
        }

        #endregion
    }
}
