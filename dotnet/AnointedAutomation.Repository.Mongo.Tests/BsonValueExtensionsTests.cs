// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System;
using AnointedAutomation.Repository.Mongo.Bson;
using MongoDB.Bson;
using Xunit;

namespace AnointedAutomation.Repository.Mongo.Tests
{
    public class BsonValueExtensionsTests
    {
        private static readonly DateTime When = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);

        private static BsonDocument Doc() => new BsonDocument
        {
            { "s", "text" },
            { "pad", "  hi  " },
            { "blank", "   " },
            { "empty", "" },
            { "i32", 7 },
            { "i64", 9007199254740993L },
            { "big", 5000000000L },
            { "dbl", 12.9 },
            { "negdbl", -12.9 },
            { "hugeDbl", 1e30 },
            { "nan", double.NaN },
            { "dec", new Decimal128(42.7m) },
            { "numStr", " 15 " },
            { "numStr2", "15" },
            { "floatStr", "1.5" },
            { "t", true },
            { "tStr", "TRUE" },
            { "dt", When },
            { "nul", BsonNull.Value },
            { "nested", new BsonDocument { { "inner", new BsonDocument { { "x", 3 } } }, { "nul", BsonNull.Value } } },
        };

        [Fact]
        public void Presence()
        {
            BsonDocument d = Doc();
            Assert.True(d.Has("s"));
            Assert.False(d.Has("nul"));
            Assert.False(d.Has("missing"));
            Assert.False(((BsonDocument)null).Has("s"));
            Assert.Null(d.GetValueOrNull("nul"));
            Assert.Equal(3, d.GetPath("nested.inner.x").AsInt32);
            Assert.True(d.GetPath("nested.nul").IsBsonNull);
            Assert.Null(d.GetPath("nested.inner.x.y"));
            Assert.Null(d.GetPath("nested.zz.x"));
            Assert.Equal("text", d.GetPath("s").AsString);
            Assert.Null(d.GetPath("missing"));
            Assert.Null(((BsonDocument)null).GetPath("s"));
            Assert.Equal(7, d.PickFirstNonNull("missing", "nul", "i32", "s").AsInt32);
            Assert.Null(d.PickFirstNonNull("missing", "nul"));
            Assert.True(d.GetFirstPresent("nul", "s").IsBsonNull);
            Assert.Equal("text", d.GetFirstPresent("missing", "s").AsString);
            Assert.Null(d.GetFirstPresent("missing", "missing2"));
        }

        [Fact]
        public void Strings()
        {
            BsonDocument d = Doc();
            Assert.Equal("text", d.GetStringOrNull("s"));
            Assert.Null(d.GetStringOrNull("i32"));
            Assert.Null(d.GetStringOrNull("nul"));
            Assert.Equal("", d.GetStringOrNull("empty"));
            Assert.Equal("fb", d.GetStringOr("i32", "fb"));
            Assert.Equal("", d.GetStringOr("missing"));
            Assert.Null(d.GetNonEmptyStringOrNull("empty"));
            Assert.Equal("   ", d.GetNonEmptyStringOrNull("blank"));
            Assert.Equal("hi", d.GetTrimmedStringOrNull("pad"));
            Assert.Null(d.GetTrimmedStringOrNull("blank"));
            Assert.Null(d.GetTrimmedStringOrNull("i32"));
            Assert.Null(d.GetTrimmedStringOrNull("nul", "s"));
            Assert.Equal("hi", d.GetTrimmedStringOrNull("missing", "pad"));
            Assert.Null(((BsonDocument)null).GetStringOrNull("s"));
        }

        [Fact]
        public void NumberAsString_IsExactAbove2Pow53()
        {
            BsonDocument d = Doc();
            Assert.Equal("9007199254740993", d.GetNumberAsStringOrNull("i64"));
            Assert.Equal("12", d.GetNumberAsStringOrNull("dbl"));
            Assert.Equal("-12", d.GetNumberAsStringOrNull("negdbl"));
            Assert.Equal("42", d.GetNumberAsStringOrNull("dec"));
            Assert.Null(d.GetNumberAsStringOrNull("hugeDbl"));
            Assert.Null(d.GetNumberAsStringOrNull("nan"));
            Assert.Null(d.GetNumberAsStringOrNull("numStr2"));
            Assert.Null(d.GetNumberAsStringOrNull("nul"));
        }

        [Fact]
        public void Paths()
        {
            BsonDocument d = new BsonDocument
            {
                { "a", BsonNull.Value },
                { "b", "nope" },
                { "c", new BsonDocument { { "d", "12" } } },
                { "huge", 1e30 },
                { "e", 5.5 },
                { "o", new BsonDocument { { "k", 1 } } },
            };
            Assert.Equal(12, d.GetInt64FromPathsOr(0, "missing", "a", "b", "huge", "c.d"));
            Assert.Equal(5, d.GetInt64FromPathsOr(0, "e"));
            Assert.Equal(-1, d.GetInt64FromPathsOr(-1, "a", "b"));
            Assert.Equal(-1, d.GetInt64FromPathsOr(-1, null));
            Assert.Equal("nope", d.GetStringFromPathsOrEmpty("a", "b"));
            Assert.Equal("5.5", d.GetStringFromPathsOrEmpty("missing", "e"));
            Assert.Equal("{ \"k\" : 1 }", d.GetStringFromPathsOrEmpty("o"));
            Assert.Equal("", d.GetStringFromPathsOrEmpty("a"));
            Assert.Equal("", d.GetStringFromPathsOrEmpty(null));
        }

        [Fact]
        public void IntegersRangeChecked()
        {
            BsonDocument d = Doc();
            Assert.Equal(7, d.GetIntOr("i32"));
            Assert.Equal(-1, d.GetIntOr("big", -1));
            Assert.Equal(12, d.GetIntOr("dbl"));
            Assert.Equal(-1, d.GetIntOr("hugeDbl", -1));
            Assert.Equal(-1, d.GetIntOr("nan", -1));
            Assert.Equal(42, d.GetIntOr("dec"));
            Assert.Equal(-1, d.GetIntOr("numStr2", -1));
            Assert.Equal(5000000000L, d.GetLongOr("big"));
            Assert.Equal(12L, d.GetLongOr("dbl"));
            Assert.Equal(42L, d.GetLongOr("dec"));
            Assert.Equal(-1L, d.GetLongOr("hugeDbl", -1));
            Assert.Equal(-1L, d.GetLongOr("numStr2", -1));

            Assert.Equal(5000000000L, d.GetIntegralInt64Or("big"));
            Assert.Equal(7L, d.GetIntegralInt64Or("i32"));
            Assert.Equal(0L, d.GetIntegralInt64Or("dbl"));
            Assert.Equal(0L, d.GetIntegralInt64Or("dec"));
            Assert.Equal(7, d.GetIntegralInt32Or("i32"));
            Assert.Equal(0, d.GetIntegralInt32Or("big"));
            Assert.Equal(0, d.GetIntegralInt32Or("dbl"));
            Assert.Equal(12L, d.GetInt64TruncOr("dbl"));
            Assert.Equal(0L, d.GetInt64TruncOr("dec"));
            Assert.Equal(0L, d.GetInt64TruncOr("numStr2"));
        }

        [Fact]
        public void Doubles()
        {
            BsonDocument d = Doc();
            Assert.Equal(12.9, d.GetDoubleOr("dbl"));
            Assert.Equal(7.0, d.GetDoubleOr("i32"));
            Assert.Equal(-1.0, d.GetDoubleOr("numStr2", -1));
            Assert.Equal(15.0, d.GetDoubleCoercedOr("missing", "numStr"));
            Assert.Equal(-1.0, d.GetDoubleCoercedOr("nul", "numStr", -1));
            Assert.Equal(1.5, d.GetDoubleCoercedOrNull("floatStr"));
            Assert.Equal(15.0, d.GetDoubleCoercedOrNull("numStr"));
            Assert.Null(d.GetDoubleCoercedOrNull("t"));
            Assert.True(double.IsNaN(d.GetDoubleCoercedOrNull("nan").Value));
            Assert.Null(d.GetFiniteDoubleCoercedOrNull("nan"));
            Assert.Equal(12.9, d.GetFiniteDoubleCoercedOrNull("dbl"));
            Assert.Equal(1.5, d.GetFiniteDoubleCoercedOrNull("floatStr"));
            Assert.Null(d.GetFiniteDoubleCoercedOrNull("nul"));
            Assert.Null(d.GetFiniteDoubleCoercedOrNull("t"));
        }

        [Fact]
        public void BoolsAndDates()
        {
            BsonDocument d = Doc();
            Assert.True(d.GetBoolOr("t"));
            Assert.False(d.GetBoolOr("tStr"));
            Assert.True(d.GetBoolCoercedOrNull("tStr"));
            Assert.Null(d.GetBoolCoercedOrNull("s"));
            Assert.Null(d.GetBoolCoercedOrNull("nul"));
            Assert.Equal(When, d.GetDateTimeOr("dt", DateTime.MinValue));
            Assert.Equal(DateTime.MinValue, d.GetDateTimeOr("s", DateTime.MinValue));
            Assert.Equal(When, d.GetNullableDateTime("dt"));
            Assert.Null(d.GetNullableDateTime("nul"));
        }

        [Fact]
        public void ValueLevel()
        {
            Assert.Null(((BsonValue)null).AsStringOrNull());
            Assert.Null(BsonNull.Value.AsStringOrNull());
            Assert.Equal("x", new BsonString("x").AsStringOrNull());
            Assert.Equal("5", new BsonInt32(5).AsStringOrNull());
            Assert.Null(new BsonInt32(5).AsStringStrictOrNull());
            Assert.Equal("x", new BsonString("x").AsStringStrictOrNull());
            Assert.Null(((BsonValue)null).AsStringStrictOrNull());

            Assert.Equal(12L, new BsonDouble(12.9).AsInt64Or());
            Assert.Equal(15L, new BsonString(" 15 ").AsInt64Or());
            Assert.Equal(-1L, new BsonDouble(1e30).AsInt64Or(-1));
            Assert.Equal(-1L, BsonNull.Value.AsInt64Or(-1));
            Assert.Equal(-1L, ((BsonValue)null).AsInt64Or(-1));
            Assert.Equal(12L, new BsonDouble(12.9).AsInt64TruncOr());
            Assert.Equal(0L, new BsonString("15").AsInt64TruncOr());

            Assert.Equal(15L, new BsonString("15").AsIntegralOrNumericStringInt64Or());
            Assert.Equal(0L, new BsonDouble(15).AsIntegralOrNumericStringInt64Or());
            Assert.Equal(9007199254740993L, new BsonInt64(9007199254740993L).AsIntegralOrNumericStringInt64Or());
            Assert.Equal(0L, BsonNull.Value.AsIntegralOrNumericStringInt64Or());

            Assert.Equal(5, new BsonInt64(5).AsNullableInt32Trunc());
            Assert.Null(new BsonInt64(5000000000L).AsNullableInt32Trunc());
            Assert.Equal(12, new BsonDouble(12.9).AsNullableInt32Trunc());
            Assert.Null(new BsonString("1").AsNullableInt32Trunc());

            Assert.True(new BsonBoolean(true).AsBooleanOr());
            Assert.True(new BsonString("true").AsBooleanOr(true));
            Assert.Null(new BsonString("x").AsNullableUtcDateTime());
        }

        [Theory]
        [InlineData("gid://shopify/ProductVariant/42", 42L, 42L)]
        [InlineData("42", 42L, 42L)]
        [InlineData("gid://shopify/ProductVariant/x", null, null)]
        [InlineData("gid://shopify/ProductVariant/42?x", null, null)]
        public void GidTailReaders(string text, long? lenient, long? integral)
        {
            Assert.Equal(lenient, new BsonString(text).AsInt64OrGidTailOrNull());
            Assert.Equal(integral, new BsonString(text).AsIntegralOrGidTailInt64OrNull());
        }

        [Fact]
        public void GidTailReaders_NonStrings()
        {
            Assert.Equal(12L, new BsonDouble(12.9).AsInt64OrGidTailOrNull());
            Assert.Null(new BsonDouble(12.9).AsIntegralOrGidTailInt64OrNull());
            Assert.Equal(5L, new BsonInt32(5).AsIntegralOrGidTailInt64OrNull());
            Assert.Equal(5L, new BsonInt64(5).AsInt64OrGidTailOrNull());
            Assert.Null(BsonNull.Value.AsInt64OrGidTailOrNull());
            Assert.Null(((BsonValue)null).AsIntegralOrGidTailInt64OrNull());
            Assert.Null(new BsonBoolean(true).AsInt64OrGidTailOrNull());
        }

        [Fact]
        public void Quantity()
        {
            Assert.Equal(3, new BsonInt32(3).AsQuantityInt32OrNull());
            Assert.Equal(3, new BsonInt64(3).AsQuantityInt32OrNull());
            Assert.Null(new BsonInt64(5000000000L).AsQuantityInt32OrNull());
            Assert.Equal(3, new BsonDouble(2.99999).AsQuantityInt32OrNull());
            Assert.Null(new BsonDouble(2.5).AsQuantityInt32OrNull());
            Assert.Null(new BsonDouble(1e30).AsQuantityInt32OrNull());
            Assert.Null(new BsonDouble(double.NaN).AsQuantityInt32OrNull());
            Assert.Equal(4, new BsonString(" 4 ").AsQuantityInt32OrNull());
            Assert.Null(new BsonString("4.0").AsQuantityInt32OrNull());
            Assert.Null(BsonNull.Value.AsQuantityInt32OrNull());
            Assert.Null(((BsonValue)null).AsQuantityInt32OrNull());
        }
    }
}
