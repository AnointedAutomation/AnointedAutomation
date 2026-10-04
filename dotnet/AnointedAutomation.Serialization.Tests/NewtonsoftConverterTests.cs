// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System;
using System.Runtime.Serialization;
using AnointedAutomation.Serialization.Newtonsoft;
using Newtonsoft.Json;
using Xunit;

namespace AnointedAutomation.Serialization.Tests
{
    public enum Status
    {
        Unknown = 0,

        [EnumMember(Value = "in-progress")]
        InProgress = 1,

        [EnumMember(Value = "done/ok")]
        Done = 2,

        Pending = 3,
    }

    public class EnumHolder
    {
        [JsonConverter(typeof(TolerantEnumConverter))]
        public Status Plain { get; set; }

        [JsonConverter(typeof(TolerantEnumConverter))]
        public Status? Maybe { get; set; }
    }

    public class BoolHolder
    {
        [JsonConverter(typeof(StringOrBoolConverter))]
        public bool? Flag { get; set; }
    }

    public class DateHolder
    {
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime? Expires { get; set; }
    }

    public class ErrorHolder
    {
        [JsonConverter(typeof(NullOnErrorConverter))]
        public DateTimeOffset? At { get; set; }

        [JsonConverter(typeof(NullOnErrorConverter))]
        public DateTime? When { get; set; }

        [JsonConverter(typeof(NullOnErrorConverter))]
        public int? Number { get; set; }

        [JsonConverter(typeof(NullOnErrorConverter))]
        public string Text { get; set; }
    }

    public class NewtonsoftConverterTests
    {
        [Theory]
        [InlineData("\"in-progress\"", Status.InProgress, Status.InProgress)]
        [InlineData("\"done/ok\"", Status.Done, Status.Done)]
        [InlineData("\"Pending\"", Status.Pending, Status.Pending)]
        [InlineData("\"pending\"", Status.Pending, Status.Pending)]
        [InlineData("2", Status.Done, Status.Done)]
        public void Enum_ReadsKnownValues(string wire, Status plain, Status maybe)
        {
            EnumHolder h = JsonConvert.DeserializeObject<EnumHolder>("{\"Plain\":" + wire + ",\"Maybe\":" + wire + "}");
            Assert.Equal(plain, h.Plain);
            Assert.Equal(maybe, h.Maybe);
        }

        [Theory]
        [InlineData("\"brand-new\"")]
        [InlineData("\"IN-PROGRESS\"")]
        [InlineData("\"\"")]
        [InlineData("null")]
        [InlineData("99")]
        [InlineData("true")]
        [InlineData("\"5\"")]
        public void Enum_UnknownIsDefaultOrNull(string wire)
        {
            EnumHolder h = JsonConvert.DeserializeObject<EnumHolder>("{\"Plain\":" + wire + ",\"Maybe\":" + wire + "}");
            Assert.Equal(Status.Unknown, h.Plain);
            Assert.Null(h.Maybe);
        }

        [Fact]
        public void Enum_WritesMemberValueOrName()
        {
            Assert.Equal("{\"Plain\":\"in-progress\",\"Maybe\":null}", JsonConvert.SerializeObject(new EnumHolder { Plain = Status.InProgress }));
            Assert.Equal("{\"Plain\":\"Pending\",\"Maybe\":\"done/ok\"}", JsonConvert.SerializeObject(new EnumHolder { Plain = Status.Pending, Maybe = Status.Done }));
            Assert.Equal("in-progress", TolerantEnumConverter.ToWire(Status.InProgress));
            Assert.Equal(Status.Done, TolerantEnumConverter.Parse<Status>("done/ok"));
            Assert.Null(TolerantEnumConverter.Parse<Status>("nope"));
            Assert.Null(TolerantEnumConverter.Parse<Status>(null));
            Assert.True(new TolerantEnumConverter().CanConvert(typeof(Status?)));
            Assert.False(new TolerantEnumConverter().CanConvert(typeof(int)));
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("\"true\"", true)]
        [InlineData("\" FALSE \"", false)]
        [InlineData("\"yes\"", null)]
        [InlineData("\"\"", null)]
        [InlineData("null", null)]
        public void StringOrBool_Reads(string wire, bool? expected)
        {
            Assert.Equal(expected, JsonConvert.DeserializeObject<BoolHolder>("{\"Flag\":" + wire + "}").Flag);
        }

        [Fact]
        public void StringOrBool_RejectsNumbers_AndWritesBool()
        {
            Assert.Throws<JsonSerializationException>(() => JsonConvert.DeserializeObject<BoolHolder>("{\"Flag\":1}"));
            Assert.Equal("{\"Flag\":true}", JsonConvert.SerializeObject(new BoolHolder { Flag = true }));
            Assert.Equal("{\"Flag\":null}", JsonConvert.SerializeObject(new BoolHolder()));
        }

        [Fact]
        public void DateFormat_WritesFormat()
        {
            Assert.Equal("{\"Expires\":\"2026-02-03\"}", JsonConvert.SerializeObject(new DateHolder { Expires = new DateTime(2026, 2, 3, 4, 5, 6) }));
            Assert.Equal(new DateTime(2026, 2, 3), JsonConvert.DeserializeObject<DateHolder>("{\"Expires\":\"2026-02-03\"}").Expires);
        }

        [Fact]
        public void NullOnError_SwallowsBadValues_AndInvalidShopifyDates()
        {
            ErrorHolder h = JsonConvert.DeserializeObject<ErrorHolder>(
                "{\"At\":\"0000-12-31T18:09:24-05:50\",\"When\":\"0000-01-01T00:00:00Z\",\"Number\":\"abc\",\"Text\":\"ok\"}");
            Assert.Null(h.At);
            Assert.Null(h.When);
            Assert.Null(h.Number);
            Assert.Equal("ok", h.Text);

            ErrorHolder good = JsonConvert.DeserializeObject<ErrorHolder>("{\"At\":\"2026-01-01T00:00:00+00:00\",\"Number\":4}");
            Assert.Equal(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), good.At);
            Assert.Equal(4, good.Number);
            Assert.False(new NullOnErrorConverter().CanWrite);
        }
    }
}
