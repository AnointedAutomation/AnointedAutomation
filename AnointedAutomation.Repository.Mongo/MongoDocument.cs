// Copyright 2026 Anointed Automation, LLC. All Rights Reserved.
// Stewarded by Alexander Fields https://www.alexanderfields.me

using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AnointedAutomation.Repository.Mongo
{
    /// <summary>
    /// Base class for MongoDB documents with common BSON attributes.
    /// </summary>
    [BsonIgnoreExtraElements]
    public abstract class MongoDocument
    {
        /// <summary>
        /// MongoDB ObjectId as string.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
    }

    /// <summary>
    /// Base class for MongoDB documents with audit timestamps.
    /// </summary>
    [BsonIgnoreExtraElements]
    public abstract class AuditableMongoDocument : MongoDocument
    {
        /// <summary>
        /// When the document was created.
        /// </summary>
        [BsonElement("createdAt")]
        public DateTime createdAt { get; set; }

        /// <summary>
        /// When the document was last updated.
        /// </summary>
        [BsonElement("updatedAt")]
        public DateTime updatedAt { get; set; }
    }
}
