// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

#nullable enable

using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace AnointedAutomation.Repository.Mongo
{
    /// <summary>
    /// Converts BSON values into plain .NET objects (dictionaries, lists, and scalars) suitable for
    /// JSON serialization or general-purpose consumption without a BSON dependency.
    /// </summary>
    public static class BsonMap
    {
        /// <summary>
        /// Recursively converts a <see cref="BsonValue"/> to a plain .NET object:
        /// <see cref="BsonDocument"/> becomes <c>Dictionary&lt;string, object?&gt;</c>,
        /// <see cref="BsonArray"/> becomes <c>List&lt;object?&gt;</c>, scalars become their CLR
        /// equivalents, ObjectId becomes its hex string, and Null/Undefined become null. Any other
        /// BSON type falls back to its string representation.
        /// </summary>
        /// <param name="value">The BSON value to convert, or null.</param>
        /// <returns>The plain .NET representation, or null.</returns>
        public static object? ToPlain(BsonValue? value)
        {
            if (value == null)
            {
                return null;
            }

            switch (value.BsonType)
            {
                case BsonType.Null:
                case BsonType.Undefined:
                    return null;

                case BsonType.Document:
                    return ToDictionary(value.AsBsonDocument);

                case BsonType.Array:
                    List<object?> list = new List<object?>(value.AsBsonArray.Count);
                    foreach (BsonValue element in value.AsBsonArray)
                    {
                        list.Add(ToPlain(element));
                    }
                    return list;

                case BsonType.String:
                    return value.AsString;

                case BsonType.Int32:
                    return value.AsInt32;

                case BsonType.Int64:
                    return value.AsInt64;

                case BsonType.Double:
                    return value.AsDouble;

                case BsonType.Decimal128:
                    return Decimal128.ToDecimal(value.AsDecimal128);

                case BsonType.Boolean:
                    return value.AsBoolean;

                case BsonType.DateTime:
                    return value.ToUniversalTime();

                case BsonType.ObjectId:
                    return value.AsObjectId.ToString();

                default:
                    return value.ToString();
            }
        }

        /// <summary>
        /// Converts a <see cref="BsonDocument"/> to a <c>Dictionary&lt;string, object?&gt;</c>,
        /// recursively converting every element via <see cref="ToPlain(BsonValue?)"/>.
        /// </summary>
        /// <param name="document">The BSON document to convert.</param>
        /// <returns>A dictionary of element names to plain .NET values.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
        public static Dictionary<string, object?> ToDictionary(BsonDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            Dictionary<string, object?> result = new Dictionary<string, object?>(document.ElementCount);
            foreach (BsonElement element in document.Elements)
            {
                result[element.Name] = ToPlain(element.Value);
            }
            return result;
        }
    }
}
