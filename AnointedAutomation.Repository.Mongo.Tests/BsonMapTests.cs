// Copyright 2026 Anointed Automation, LLC. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
// Unit tests for BsonMap: pure BSON-to-plain-object conversion, no database required.

using MongoDB.Bson;
using System;
using System.Collections.Generic;
using Xunit;

namespace AnointedAutomation.Repository.Mongo.Tests
{
    public class BsonMapTests
    {
        [Fact]
        public void ToPlain_Null_ReturnsNull()
        {
            Assert.Null(BsonMap.ToPlain(null));
            Assert.Null(BsonMap.ToPlain(BsonNull.Value));
            Assert.Null(BsonMap.ToPlain(BsonUndefined.Value));
        }

        [Fact]
        public void ToPlain_Scalars_ConvertToClrTypes()
        {
            Assert.Equal("hello", BsonMap.ToPlain(new BsonString("hello")));
            Assert.Equal(42, BsonMap.ToPlain(new BsonInt32(42)));
            Assert.Equal(42L, BsonMap.ToPlain(new BsonInt64(42L)));
            Assert.Equal(3.5d, BsonMap.ToPlain(new BsonDouble(3.5d)));
            Assert.Equal(19.99m, BsonMap.ToPlain(new BsonDecimal128(19.99m)));
            Assert.Equal(true, BsonMap.ToPlain(BsonBoolean.True));
        }

        [Fact]
        public void ToPlain_DateTime_ReturnsUtcDateTime()
        {
            DateTime utc = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            object result = BsonMap.ToPlain(new BsonDateTime(utc));
            Assert.Equal(utc, Assert.IsType<DateTime>(result));
        }

        [Fact]
        public void ToPlain_ObjectId_ReturnsHexString()
        {
            ObjectId id = ObjectId.GenerateNewId();
            Assert.Equal(id.ToString(), BsonMap.ToPlain(new BsonObjectId(id)));
        }

        [Fact]
        public void ToPlain_UnknownType_FallsBackToString()
        {
            object result = BsonMap.ToPlain(new BsonRegularExpression("abc", "i"));
            Assert.IsType<string>(result);
        }

        [Fact]
        public void ToPlain_Array_ReturnsListRecursively()
        {
            BsonArray array = new BsonArray { 1, "two", new BsonDocument("x", 3), BsonNull.Value };
            List<object> list = Assert.IsType<List<object>>(BsonMap.ToPlain(array));
            Assert.Equal(4, list.Count);
            Assert.Equal(1, list[0]);
            Assert.Equal("two", list[1]);
            Dictionary<string, object> nested = Assert.IsType<Dictionary<string, object>>(list[2]);
            Assert.Equal(3, nested["x"]);
            Assert.Null(list[3]);
        }

        [Fact]
        public void ToDictionary_NestedDocument_ConvertsRecursively()
        {
            BsonDocument document = new BsonDocument
            {
                { "name", "widget" },
                { "count", 7 },
                { "child", new BsonDocument { { "flag", true } } },
                { "tags", new BsonArray { "a", "b" } }
            };

            Dictionary<string, object> result = BsonMap.ToDictionary(document);

            Assert.Equal("widget", result["name"]);
            Assert.Equal(7, result["count"]);
            Dictionary<string, object> child = Assert.IsType<Dictionary<string, object>>(result["child"]);
            Assert.Equal(true, child["flag"]);
            List<object> tags = Assert.IsType<List<object>>(result["tags"]);
            Assert.Equal(new List<object> { "a", "b" }, tags);
        }

        [Fact]
        public void ToDictionary_NullDocument_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => BsonMap.ToDictionary(null));
        }
    }
}
