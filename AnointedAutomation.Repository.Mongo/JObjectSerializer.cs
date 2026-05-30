// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json.Linq;

namespace AnointedAutomation.Repository.Mongo
{
    /// <summary>
    /// Custom serializer for converting between JObject and BSON format.
    /// This serializer enables seamless storage and retrieval of Newtonsoft.Json JObjects in MongoDB.
    /// </summary>
    public class JObjectSerializer : SerializerBase<JObject>
    {
        /// <summary>
        /// Deserializes a BSON value into a JObject.
        /// </summary>
        /// <param name="context">The deserialization context.</param>
        /// <param name="args">The deserialization arguments.</param>
        /// <returns>A JObject instance, or null if the BSON value is null.</returns>
        public override JObject Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            BsonType bsonType = context.Reader.GetCurrentBsonType();
            if (bsonType == BsonType.Null)
            {
                context.Reader.ReadNull();
                return null;
            }

            BsonDocument bsonDocument = BsonDocumentSerializer.Instance.Deserialize(context, args);
            return JObject.Parse(bsonDocument.ToJson());
        }

        /// <summary>
        /// Serializes a JObject into BSON format.
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="args">The serialization arguments.</param>
        /// <param name="value">The JObject to serialize.</param>
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JObject value)
        {
            if (value == null)
            {
                context.Writer.WriteNull();
                return;
            }

            BsonDocument bsonDocument = BsonDocument.Parse(value.ToString());
            BsonDocumentSerializer.Instance.Serialize(context, args, bsonDocument);
        }
    }
}
