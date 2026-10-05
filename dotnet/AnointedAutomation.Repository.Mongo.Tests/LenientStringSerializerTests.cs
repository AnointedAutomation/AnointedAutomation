// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from the Anointed API (Tests/Services/Setup/LenientStringSerializerTests.cs).

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Xunit;

namespace LenientStringProbe
{
    public sealed class ImageHashEntryProbe
    {
        [BsonSerializer(typeof(AnointedAutomation.Repository.Mongo.Bson.LenientStringSerializer))]
        public string ProductId { get; set; }
    }

    public sealed class LenientStringSerializerTests
    {
        private static ImageHashEntryProbe Roundtrip(BsonDocument doc) => BsonSerializer.Deserialize<ImageHashEntryProbe>(doc);

        [Fact]
        public void Int32_Deserializes_As_String() =>
            Assert.Equal("2800158", Roundtrip(new BsonDocument { { "ProductId", 2800158 } }).ProductId);

        [Fact]
        public void Int64_Deserializes_As_String() =>
            Assert.Equal("15387259469935", Roundtrip(new BsonDocument { { "ProductId", 15387259469935L } }).ProductId);

        [Fact]
        public void Double_Deserializes_As_RoundTrip_String() =>
            Assert.Equal("1.5", Roundtrip(new BsonDocument { { "ProductId", 1.5 } }).ProductId);

        [Fact]
        public void String_Deserializes_Unchanged() =>
            Assert.Equal("gid://shopify/Product/123", Roundtrip(new BsonDocument { { "ProductId", "gid://shopify/Product/123" } }).ProductId);

        [Fact]
        public void Null_Deserializes_As_Null() =>
            Assert.Null(Roundtrip(new BsonDocument { { "ProductId", BsonNull.Value } }).ProductId);

        [Fact]
        public void Bool_Throws() =>
            Assert.ThrowsAny<System.Exception>(() => Roundtrip(new BsonDocument { { "ProductId", true } }));

        [Fact]
        public void Serializes_Back_As_String()
        {
            BsonDocument doc = new ImageHashEntryProbe { ProductId = "2800158" }.ToBsonDocument();
            Assert.Equal(BsonType.String, doc["ProductId"].BsonType);
            Assert.Equal("2800158", doc["ProductId"].AsString);
            Assert.Equal(BsonType.Null, new ImageHashEntryProbe().ToBsonDocument()["ProductId"].BsonType);
        }
    }
}
