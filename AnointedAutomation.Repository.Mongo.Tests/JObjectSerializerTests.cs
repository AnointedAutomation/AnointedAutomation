// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Created by Alexander Fields https://www.alexanderfields.me
using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json.Linq;
using Xunit;

namespace AnointedAutomation.Repository.Mongo.Tests
{
    /// <summary>
    /// Tests for the JObjectSerializer class.
    /// </summary>
    public class JObjectSerializerTests
    {
        [Fact]
        public void Serialize_WithValidJObject_SerializesBsonDocument()
        {
            // Arrange
            JObjectSerializer serializer = new JObjectSerializer();
            JObject jObject = JObject.Parse("{\"name\":\"test\",\"value\":123}");

            using MemoryStream stream = new MemoryStream();
            using BsonBinaryWriter writer = new BsonBinaryWriter(stream);
            BsonSerializationContext context = BsonSerializationContext.CreateRoot(writer);
            BsonSerializationArgs args = new BsonSerializationArgs();

            // Act
            serializer.Serialize(context, args, jObject);

            // Assert
            stream.Position = 0;
            using BsonBinaryReader reader = new BsonBinaryReader(stream);
            BsonDocument resultDoc = BsonDocumentSerializer.Instance.Deserialize(
                BsonDeserializationContext.CreateRoot(reader));

            Assert.Equal("test", resultDoc["name"].AsString);
            Assert.Equal(123, resultDoc["value"].AsInt32);
        }

        [Fact]
        public void Serialize_WithNullJObject_WritesNull()
        {
            // Arrange
            JObjectSerializer serializer = new JObjectSerializer();
            JObject jObject = null;

            using MemoryStream stream = new MemoryStream();
            using BsonBinaryWriter writer = new BsonBinaryWriter(stream);

            // We need to wrap in a document to write null
            writer.WriteStartDocument();
            writer.WriteName("testField");
            BsonSerializationContext context = BsonSerializationContext.CreateRoot(writer);
            BsonSerializationArgs args = new BsonSerializationArgs();

            // Act
            serializer.Serialize(context, args, jObject);
            writer.WriteEndDocument();

            // Assert
            stream.Position = 0;
            using BsonBinaryReader reader = new BsonBinaryReader(stream);
            BsonDocument resultDoc = BsonDocumentSerializer.Instance.Deserialize(
                BsonDeserializationContext.CreateRoot(reader));

            Assert.True(resultDoc["testField"].IsBsonNull);
        }

        [Fact]
        public void Deserialize_WithValidBsonDocument_ReturnsJObject()
        {
            // Arrange
            JObjectSerializer serializer = new JObjectSerializer();
            BsonDocument bsonDoc = new BsonDocument
            {
                { "name", "test" },
                { "value", 456 }
            };

            using MemoryStream stream = new MemoryStream();
            using (BsonBinaryWriter writer = new BsonBinaryWriter(stream))
            {
                BsonDocumentSerializer.Instance.Serialize(
                    BsonSerializationContext.CreateRoot(writer),
                    new BsonSerializationArgs(),
                    bsonDoc);
            }

            stream.Position = 0;
            using BsonBinaryReader reader = new BsonBinaryReader(stream);
            BsonDeserializationContext context = BsonDeserializationContext.CreateRoot(reader);
            BsonDeserializationArgs args = new BsonDeserializationArgs();

            // Act
            JObject result = serializer.Deserialize(context, args);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test", result["name"].ToString());
            Assert.Equal(456, result["value"].ToObject<int>());
        }

        [Fact]
        public void Deserialize_WithBsonNull_ReturnsNull()
        {
            // Arrange
            JObjectSerializer serializer = new JObjectSerializer();

            // Create a document with a null field
            BsonDocument wrapperDoc = new BsonDocument
            {
                { "testField", BsonNull.Value }
            };

            using MemoryStream stream = new MemoryStream();
            using (BsonBinaryWriter writer = new BsonBinaryWriter(stream))
            {
                BsonDocumentSerializer.Instance.Serialize(
                    BsonSerializationContext.CreateRoot(writer),
                    new BsonSerializationArgs(),
                    wrapperDoc);
            }

            stream.Position = 0;
            using BsonBinaryReader reader = new BsonBinaryReader(stream);

            // Read the wrapper document to get to the null field
            reader.ReadStartDocument();
            reader.ReadBsonType();
            reader.SkipName();

            BsonDeserializationContext context = BsonDeserializationContext.CreateRoot(reader);
            BsonDeserializationArgs args = new BsonDeserializationArgs();

            // Act
            JObject result = serializer.Deserialize(context, args);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Serialize_WithNestedJObject_SerializesNestedStructure()
        {
            // Arrange
            JObjectSerializer serializer = new JObjectSerializer();
            JObject jObject = JObject.Parse("{\"outer\":{\"inner\":\"value\"},\"array\":[1,2,3]}");

            using MemoryStream stream = new MemoryStream();
            using BsonBinaryWriter writer = new BsonBinaryWriter(stream);
            BsonSerializationContext context = BsonSerializationContext.CreateRoot(writer);
            BsonSerializationArgs args = new BsonSerializationArgs();

            // Act
            serializer.Serialize(context, args, jObject);

            // Assert
            stream.Position = 0;
            using BsonBinaryReader reader = new BsonBinaryReader(stream);
            BsonDocument resultDoc = BsonDocumentSerializer.Instance.Deserialize(
                BsonDeserializationContext.CreateRoot(reader));

            Assert.True(resultDoc.Contains("outer"));
            Assert.True(resultDoc["outer"].IsBsonDocument);
            Assert.Equal("value", resultDoc["outer"]["inner"].AsString);
        }

        [Fact]
        public void Serialize_WithEmptyJObject_SerializesEmptyDocument()
        {
            // Arrange
            JObjectSerializer serializer = new JObjectSerializer();
            JObject jObject = new JObject();

            using MemoryStream stream = new MemoryStream();
            using BsonBinaryWriter writer = new BsonBinaryWriter(stream);
            BsonSerializationContext context = BsonSerializationContext.CreateRoot(writer);
            BsonSerializationArgs args = new BsonSerializationArgs();

            // Act
            serializer.Serialize(context, args, jObject);

            // Assert
            stream.Position = 0;
            using BsonBinaryReader reader = new BsonBinaryReader(stream);
            BsonDocument resultDoc = BsonDocumentSerializer.Instance.Deserialize(
                BsonDeserializationContext.CreateRoot(reader));

            Assert.Empty(resultDoc);
        }

        [Fact]
        public void RoundTrip_SerializeAndDeserialize_PreservesData()
        {
            // Arrange
            JObjectSerializer serializer = new JObjectSerializer();
            JObject original = JObject.Parse("{\"id\":\"abc123\",\"count\":42,\"active\":true}");

            // Serialize
            using MemoryStream stream = new MemoryStream();
            using (BsonBinaryWriter writer = new BsonBinaryWriter(stream))
            {
                BsonSerializationContext serContext = BsonSerializationContext.CreateRoot(writer);
                serializer.Serialize(serContext, new BsonSerializationArgs(), original);
            }

            // Deserialize
            stream.Position = 0;
            using BsonBinaryReader reader = new BsonBinaryReader(stream);
            BsonDeserializationContext deserContext = BsonDeserializationContext.CreateRoot(reader);
            JObject result = serializer.Deserialize(deserContext, new BsonDeserializationArgs());

            // Assert
            Assert.Equal(original["id"].ToString(), result["id"].ToString());
            Assert.Equal(original["count"].ToObject<int>(), result["count"].ToObject<int>());
            Assert.Equal(original["active"].ToObject<bool>(), result["active"].ToObject<bool>());
        }
    }
}
