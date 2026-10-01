// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Reads a BSON field as string even when older writers stored it numerically (e.g. a Python-built corpus storing
// productId as Int32 while the native pipeline writes Shopify gid strings). Without it, deserializing such a document
// throws FormatException. Moved from the Anointed API (AnointedAutomation.API.Objects.LenientStringSerializer).

using System.Globalization;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace AnointedAutomation.Repository.Mongo.Bson
{
    /// <summary>
    /// String serializer tolerant of numeric legacy values. Reads String verbatim, Int32/Int64 as invariant digits,
    /// Double as invariant round-trip ("R") text, Null as null; any other BSON type throws
    /// <see cref="BsonSerializationException"/>. Always writes a string (or null).
    /// Use with <c>[BsonSerializer(typeof(LenientStringSerializer))]</c>.
    /// </summary>
    public sealed class LenientStringSerializer : SerializerBase<string>
    {
        /// <inheritdoc />
        public override string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            IBsonReader reader = context.Reader;
            switch (reader.GetCurrentBsonType())
            {
                case BsonType.String:
                    return reader.ReadString();
                case BsonType.Int32:
                    return reader.ReadInt32().ToString(CultureInfo.InvariantCulture);
                case BsonType.Int64:
                    return reader.ReadInt64().ToString(CultureInfo.InvariantCulture);
                case BsonType.Double:
                    return reader.ReadDouble().ToString("R", CultureInfo.InvariantCulture);
                case BsonType.Null:
                    reader.ReadNull();
                    return null;
                default:
                    throw new BsonSerializationException(
                        $"Cannot deserialize a string from BsonType {reader.GetCurrentBsonType()}.");
            }
        }

        /// <inheritdoc />
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string value)
        {
            if (value is null)
            {
                context.Writer.WriteNull();
            }
            else
            {
                context.Writer.WriteString(value);
            }
        }
    }
}
