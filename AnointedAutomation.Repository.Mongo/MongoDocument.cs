// Copyright 2026 Anointed Automation, LLC. All Rights Reserved.
// Stewarded by Alexander Fields https://www.alexanderfields.me
//
// Document bases, one level per need:
//   MongoDocument            Id
//   CreatedMongoDocument     + CreatedAt      (append-only records: ledgers, audit rows, webhook events)
//   TimestampedMongoDocument + UpdatedAt      (mutable records, latest change only)
//   HistoriedMongoDocument   + UpdateHistory  (mutable records that keep a capped list of changes)
// Every DateTime is UTC. Property names are PascalCase with no [BsonElement] overrides, so element names come
// from the naming convention (HybridElementNameConvention: createdAt, updatedAt, UpdateHistory).
// The matching repositories (CreatedMongoRepository, TimestampedMongoRepository, HistoriedMongoRepository)
// stamp these fields so callers never set them by hand.

using System;
using System.Collections.Generic;
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
        /// MongoDB ObjectId as string. An empty id is left out of the document, so Mongo generates one on insert
        /// instead of rejecting "" as an invalid ObjectId.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonDefaultValue("")]
        [BsonIgnoreIfDefault]
        public string Id { get; set; } = string.Empty;
    }

    /// <summary>
    /// Base class for append-only documents: adds the UTC time the document was created.
    /// </summary>
    [BsonIgnoreExtraElements]
    public abstract class CreatedMongoDocument : MongoDocument
    {
        /// <summary>
        /// When the document was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Base class for mutable documents: adds the UTC time of the latest change. It equals
    /// <see cref="CreatedMongoDocument.CreatedAt"/> on insert, so it is never empty.
    /// </summary>
    [BsonIgnoreExtraElements]
    public abstract class TimestampedMongoDocument : CreatedMongoDocument
    {
        /// <summary>
        /// When the document was last changed (UTC).
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Base class for mutable documents that keep a capped history of their changes. The newest entry always
    /// matches <see cref="TimestampedMongoDocument.UpdatedAt"/>. For a complete, permanent audit trail use a
    /// separate append-only collection; this list is capped to keep the document small.
    /// </summary>
    [BsonIgnoreExtraElements]
    public abstract class HistoriedMongoDocument : TimestampedMongoDocument
    {
        /// <summary>
        /// The most recent changes, oldest first, capped by the repository.
        /// </summary>
        public List<UpdateStamp> UpdateHistory { get; set; } = new List<UpdateStamp>();
    }

    /// <summary>
    /// One recorded change on a <see cref="HistoriedMongoDocument"/>.
    /// </summary>
    [BsonIgnoreExtraElements]
    public sealed class UpdateStamp
    {
        /// <summary>
        /// When the change happened (UTC).
        /// </summary>
        public DateTime At { get; set; }

        /// <summary>
        /// Who made the change: a user id, "system", or a job name.
        /// </summary>
        public string By { get; set; } = string.Empty;

        /// <summary>
        /// The C# property names that changed.
        /// </summary>
        public List<string> Fields { get; set; } = new List<string>();
    }
}
