// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Copyright 2026 Anointed Automation, LLC. All Rights Reserved.
// Ported from the API repo's NUnit suite to xUnit.

using System;
using AnointedAutomation.Optimization;
using MongoDB.Bson;
using Xunit;

namespace AnointedAutomation.Optimization.Tests
{
    public class BsonMapTests
    {
        private static BsonDocument Sample() => new BsonDocument
        {
            { "name", "widget" },
            { "count", 7 },
            { "big", 9000000000L },
            { "price", 12.5 },
            { "active", true },
            { "when", new BsonDateTime(new DateTime(2026, 7, 30, 0, 0, 0, DateTimeKind.Utc)) },
            { "nothing", BsonNull.Value },
        };

        [Fact]
        public void Has_TrueForPresent_FalseForMissingOrNull()
        {
            BsonDocument doc = Sample();
            Assert.True(doc.Has("name"));
            Assert.False(doc.Has("nothing"));
            Assert.False(doc.Has("absent"));
        }

        [Fact]
        public void GetStringOr_ReadsOrFallsBack()
        {
            BsonDocument doc = Sample();
            Assert.Equal("widget", doc.GetStringOr("name"));
            Assert.Equal("def", doc.GetStringOr("absent", "def"));
            Assert.Equal("def", doc.GetStringOr("nothing", "def"));
            Assert.Equal("def", doc.GetStringOr("count", "def"));
        }

        [Fact]
        public void GetStringOrNull_StrictString_ElseNull()
        {
            BsonDocument doc = Sample();
            Assert.Equal("widget", doc.GetStringOrNull("name"));
            Assert.Null(doc.GetStringOrNull("absent"));
            Assert.Null(doc.GetStringOrNull("nothing"));
            Assert.Null(doc.GetStringOrNull("count"));
        }

        [Fact]
        public void NumericGetters_ReadOrFallBack()
        {
            BsonDocument doc = Sample();
            Assert.Equal(7, doc.GetIntOr("count"));
            Assert.Equal(-1, doc.GetIntOr("name", -1));
            Assert.Equal(9000000000L, doc.GetLongOr("big"));
            Assert.Equal(12.5, doc.GetDoubleOr("price"));
            Assert.Equal(1.0, doc.GetDoubleOr("absent", 1.0));
        }

        [Fact]
        public void GetBoolOr_ReadsOrFallsBack()
        {
            BsonDocument doc = Sample();
            Assert.True(doc.GetBoolOr("active"));
            Assert.True(doc.GetBoolOr("absent", true));
            Assert.False(doc.GetBoolOr("name", false));
        }

        [Fact]
        public void DateGetters_ReadOrFallBack()
        {
            BsonDocument doc = Sample();
            DateTime expected = new DateTime(2026, 7, 30, 0, 0, 0, DateTimeKind.Utc);
            Assert.Equal(expected, doc.GetDateTimeOr("when", DateTime.MinValue));
            Assert.Equal(expected, doc.GetNullableDateTime("when"));
            Assert.Null(doc.GetNullableDateTime("absent"));
            Assert.Equal(DateTime.MinValue, doc.GetDateTimeOr("name", DateTime.MinValue));
        }

        [Fact]
        public void GetTrimmedStringOrNull_TrimsAndCollapsesBlank()
        {
            BsonDocument doc = new BsonDocument
            {
                { "padded", "  hi  " },
                { "blank", "   " },
                { "num", 5 },
                { "nul", BsonNull.Value },
            };
            Assert.Equal("hi", doc.GetTrimmedStringOrNull("padded"));
            Assert.Null(doc.GetTrimmedStringOrNull("blank"));
            Assert.Null(doc.GetTrimmedStringOrNull("num"));
            Assert.Null(doc.GetTrimmedStringOrNull("nul"));
            Assert.Null(doc.GetTrimmedStringOrNull("absent"));
        }

        [Fact]
        public void GetTrimmedStringOrNull_PascalCamel_FirstPresentKeyWins()
        {
            BsonDocument primaryBad = new BsonDocument { { "Sku", 7 }, { "sku", "legacy" } };
            Assert.Null(primaryBad.GetTrimmedStringOrNull("Sku", "sku"));

            BsonDocument fallback = new BsonDocument { { "sku", "  legacy  " } };
            Assert.Equal("legacy", fallback.GetTrimmedStringOrNull("Sku", "sku"));

            BsonDocument primaryGood = new BsonDocument { { "Sku", "pascal" }, { "sku", "legacy" } };
            Assert.Equal("pascal", primaryGood.GetTrimmedStringOrNull("Sku", "sku"));
        }

        [Fact]
        public void GetNumberAsStringOrNull_RendersIntegralForm()
        {
            BsonDocument doc = new BsonDocument { { "id", 4200000000L }, { "d", 12.9 }, { "s", "x" } };
            Assert.Equal("4200000000", doc.GetNumberAsStringOrNull("id"));
            Assert.Equal("12", doc.GetNumberAsStringOrNull("d"));
            Assert.Null(doc.GetNumberAsStringOrNull("s"));
            Assert.Null(doc.GetNumberAsStringOrNull("absent"));
        }

        [Fact]
        public void GetDoubleCoercedOr_PascalCamel_NumericOrString()
        {
            Assert.Equal(12.5, new BsonDocument { { "Price", 12.5 } }.GetDoubleCoercedOr("Price", "price"));
            Assert.Equal(19.99, new BsonDocument { { "price", "19.99" } }.GetDoubleCoercedOr("Price", "price"));
            Assert.Equal(3.0, new BsonDocument { { "price", "n/a" } }.GetDoubleCoercedOr("Price", "price", 3.0));
            Assert.Equal(0.0, new BsonDocument().GetDoubleCoercedOr("Price", "price"));
        }

        [Fact]
        public void GetDoubleCoercedOrNull_NumericStringOrNull()
        {
            Assert.Equal(5.0, new BsonDocument { { "a", 5 } }.GetDoubleCoercedOrNull("a"));
            Assert.Equal(6.25, new BsonDocument { { "a", "6.25" } }.GetDoubleCoercedOrNull("a"));
            Assert.Null(new BsonDocument { { "a", "bad" } }.GetDoubleCoercedOrNull("a"));
            Assert.Null(new BsonDocument { { "a", BsonNull.Value } }.GetDoubleCoercedOrNull("a"));
            Assert.Null(new BsonDocument().GetDoubleCoercedOrNull("a"));
        }

        [Fact]
        public void GetBoolCoercedOrNull_BoolOrStringOrNull()
        {
            Assert.True(new BsonDocument { { "b", true } }.GetBoolCoercedOrNull("b"));
            Assert.True(new BsonDocument { { "b", "true" } }.GetBoolCoercedOrNull("b"));
            Assert.False(new BsonDocument { { "b", "false" } }.GetBoolCoercedOrNull("b"));
            Assert.Null(new BsonDocument { { "b", "maybe" } }.GetBoolCoercedOrNull("b"));
            Assert.Null(new BsonDocument().GetBoolCoercedOrNull("b"));
        }

        [Fact]
        public void BsonValueOverloads_CoerceLikeTheirDocumentCounterparts()
        {
            Assert.Equal("hi", ((BsonValue?)new BsonString("hi")).AsStringOrNull());
            Assert.Equal("7", ((BsonValue?)new BsonInt32(7)).AsStringOrNull());
            Assert.Null(((BsonValue?)null).AsStringOrNull());

            Assert.Equal(9L, ((BsonValue?)new BsonInt64(9L)).AsInt64Or());
            Assert.Equal(3L, ((BsonValue?)new BsonDouble(3.9)).AsInt64Or());
            Assert.Equal(42L, ((BsonValue?)new BsonString("42")).AsInt64Or());
            Assert.Equal(-1L, ((BsonValue?)new BsonString("x")).AsInt64Or(-1));
            Assert.Equal(0L, ((BsonValue?)null).AsInt64Or());

            Assert.True(((BsonValue?)new BsonBoolean(true)).AsBooleanOr());
            Assert.False(((BsonValue?)new BsonString("true")).AsBooleanOr());
            Assert.True(((BsonValue?)null).AsBooleanOr(true));

            DateTime when = new DateTime(2026, 7, 30, 0, 0, 0, DateTimeKind.Utc);
            Assert.Equal(when, ((BsonValue?)new BsonDateTime(when)).AsNullableUtcDateTime());
            Assert.Null(((BsonValue?)new BsonString("nope")).AsNullableUtcDateTime());
            Assert.Null(((BsonValue?)null).AsNullableUtcDateTime());
        }
    }
}
