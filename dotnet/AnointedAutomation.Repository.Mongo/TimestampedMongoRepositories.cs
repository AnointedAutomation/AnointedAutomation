// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Copyright 2026 Anointed Automation, LLC. All Rights Reserved.
// Stewarded by Alexander Fields https://www.alexanderfields.me
//
// Repositories that stamp the timestamp document bases, so no caller ever sets CreatedAt / UpdatedAt /
// UpdateHistory by hand. Each level overrides the base write methods, so a plain CreateAsync or
// UpdateByIdAsync is always stamped:
//   CreatedMongoRepository     CreateAsync sets CreatedAt
//   TimestampedMongoRepository also sets UpdatedAt on create, update, upsert and replace
//   HistoriedMongoRepository   also records an UpdateStamp (capped) on every change
// Callers should skip the write entirely when nothing changed; a write always counts as a change.

using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AnointedAutomation.Repository.Mongo
{
    /// <summary>
    /// Repository for append-only documents. <see cref="CreateAsync"/> stamps
    /// <see cref="CreatedMongoDocument.CreatedAt"/> with the current UTC time.
    /// </summary>
    /// <typeparam name="TDoc">The document type this repository manages.</typeparam>
    public abstract class CreatedMongoRepository<TDoc> : MongoRepository<TDoc>
        where TDoc : CreatedMongoDocument
    {
        /// <summary>
        /// The actor recorded when a caller does not name one.
        /// </summary>
        public const string SystemActor = "system";

        /// <summary>
        /// Initializes the repository with the helper and collection it operates on.
        /// </summary>
        /// <param name="mongo">The Mongo helper to delegate all operations to.</param>
        /// <param name="collectionName">The name of the collection this repository manages.</param>
        protected CreatedMongoRepository(IMongoHelper mongo, string collectionName)
            : base(mongo, collectionName)
        {
        }

        /// <summary>
        /// The current UTC time used for every stamp. Override in tests for a fixed clock.
        /// </summary>
        protected virtual DateTime UtcNow => DateTime.UtcNow;

        /// <summary>
        /// Stamps the document's timestamps, then inserts it.
        /// </summary>
        /// <param name="document">The document to insert.</param>
        /// <returns>A task that completes when the insert is done.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
        public override Task CreateAsync(TDoc document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }
            StampForCreate(document, UtcNow);
            return base.CreateAsync(document);
        }

        /// <summary>
        /// Sets the timestamps a new document carries.
        /// </summary>
        /// <param name="document">The document being inserted.</param>
        /// <param name="now">The current UTC time.</param>
        protected virtual void StampForCreate(TDoc document, DateTime now)
        {
            document.CreatedAt = now;
        }
    }

    /// <summary>
    /// Repository for mutable documents. Every write stamps <see cref="TimestampedMongoDocument.UpdatedAt"/>;
    /// inserts and upsert-inserts also stamp <see cref="CreatedMongoDocument.CreatedAt"/> with the same time.
    /// The caller's update must not set either field itself (MongoDB rejects the conflicting paths).
    /// </summary>
    /// <typeparam name="TDoc">The document type this repository manages.</typeparam>
    public abstract class TimestampedMongoRepository<TDoc> : CreatedMongoRepository<TDoc>
        where TDoc : TimestampedMongoDocument
    {
        /// <summary>
        /// Initializes the repository with the helper and collection it operates on.
        /// </summary>
        /// <param name="mongo">The Mongo helper to delegate all operations to.</param>
        /// <param name="collectionName">The name of the collection this repository manages.</param>
        protected TimestampedMongoRepository(IMongoHelper mongo, string collectionName)
            : base(mongo, collectionName)
        {
        }

        /// <summary>
        /// Applies the update plus the UpdatedAt stamp to the document with this <c>_id</c>.
        /// </summary>
        /// <param name="id">The document's <c>_id</c> value.</param>
        /// <param name="update">The caller's update definition.</param>
        /// <returns>The result of the update operation.</returns>
        public override Task<UpdateResult> UpdateByIdAsync(string id, UpdateDefinition<TDoc> update)
        {
            return base.UpdateByIdAsync(id, StampUpdate(update, UtcNow, SystemActor, Array.Empty<string>()));
        }

        /// <summary>
        /// Upserts with the UpdatedAt stamp, and stamps CreatedAt when the upsert inserts.
        /// </summary>
        /// <param name="filter">The match predicate.</param>
        /// <param name="update">The caller's update definition.</param>
        /// <returns>The result of the upsert operation.</returns>
        public override Task<UpdateResult> UpsertAsync(Expression<Func<TDoc, bool>> filter, UpdateDefinition<TDoc> update)
        {
            return base.UpsertAsync(filter, StampUpsert(update, UtcNow, SystemActor, Array.Empty<string>()));
        }

        /// <summary>
        /// Stamps UpdatedAt on the replacement document, then replaces the stored one.
        /// </summary>
        /// <param name="id">The document's <c>_id</c> value.</param>
        /// <param name="document">The new document.</param>
        /// <returns>The result of the replace operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
        public override Task<ReplaceOneResult> ReplaceByIdAsync(string id, TDoc document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }
            StampForReplace(document, UtcNow, SystemActor, Array.Empty<string>());
            return base.ReplaceByIdAsync(id, document);
        }

        /// <inheritdoc />
        protected override void StampForCreate(TDoc document, DateTime now)
        {
            base.StampForCreate(document, now);
            document.UpdatedAt = now;
        }

        /// <summary>
        /// Adds the change stamp to an update definition.
        /// </summary>
        /// <param name="update">The caller's update definition.</param>
        /// <param name="now">The current UTC time.</param>
        /// <param name="by">Who made the change.</param>
        /// <param name="fields">The property names that changed.</param>
        /// <returns>The combined update definition.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="update"/> is null.</exception>
        protected virtual UpdateDefinition<TDoc> StampUpdate(
            UpdateDefinition<TDoc> update, DateTime now, string by, IReadOnlyList<string> fields)
        {
            if (update == null)
            {
                throw new ArgumentNullException(nameof(update));
            }
            return Builders<TDoc>.Update.Combine(update, Builders<TDoc>.Update.Set(d => d.UpdatedAt, now));
        }

        /// <summary>
        /// Adds the change stamp and the insert-only CreatedAt to an upsert definition.
        /// </summary>
        /// <param name="update">The caller's update definition.</param>
        /// <param name="now">The current UTC time.</param>
        /// <param name="by">Who made the change.</param>
        /// <param name="fields">The property names that changed.</param>
        /// <returns>The combined update definition.</returns>
        protected UpdateDefinition<TDoc> StampUpsert(
            UpdateDefinition<TDoc> update, DateTime now, string by, IReadOnlyList<string> fields)
        {
            return Builders<TDoc>.Update.Combine(
                StampUpdate(update, now, by, fields),
                Builders<TDoc>.Update.SetOnInsert(d => d.CreatedAt, now));
        }

        /// <summary>
        /// Stamps a whole replacement document.
        /// </summary>
        /// <param name="document">The replacement document.</param>
        /// <param name="now">The current UTC time.</param>
        /// <param name="by">Who made the change.</param>
        /// <param name="fields">The property names that changed.</param>
        protected virtual void StampForReplace(TDoc document, DateTime now, string by, IReadOnlyList<string> fields)
        {
            document.UpdatedAt = now;
        }
    }

    /// <summary>
    /// Repository for documents that keep a capped change history. Every write also appends an
    /// <see cref="UpdateStamp"/> to <see cref="HistoriedMongoDocument.UpdateHistory"/>, keeping only the newest
    /// <see cref="UpdateHistoryLimit"/> entries. Use the overloads that take <c>by</c> and <c>fields</c> to record
    /// who changed what; the plain overloads record <see cref="CreatedMongoRepository{TDoc}.SystemActor"/>.
    /// </summary>
    /// <typeparam name="TDoc">The document type this repository manages.</typeparam>
    public abstract class HistoriedMongoRepository<TDoc> : TimestampedMongoRepository<TDoc>
        where TDoc : HistoriedMongoDocument
    {
        /// <summary>
        /// Initializes the repository with the helper and collection it operates on.
        /// </summary>
        /// <param name="mongo">The Mongo helper to delegate all operations to.</param>
        /// <param name="collectionName">The name of the collection this repository manages.</param>
        protected HistoriedMongoRepository(IMongoHelper mongo, string collectionName)
            : base(mongo, collectionName)
        {
        }

        /// <summary>
        /// How many history entries a document keeps. Older entries are dropped. Default 50.
        /// </summary>
        protected virtual int UpdateHistoryLimit => 50;

        /// <summary>
        /// Applies the update, stamping UpdatedAt and recording who changed which fields.
        /// </summary>
        /// <param name="id">The document's <c>_id</c> value.</param>
        /// <param name="update">The caller's update definition.</param>
        /// <param name="by">Who made the change: a user id, "system", or a job name.</param>
        /// <param name="fields">The property names that changed.</param>
        /// <returns>The result of the update operation.</returns>
        public Task<UpdateResult> UpdateByIdAsync(string id, UpdateDefinition<TDoc> update, string by, params string[] fields)
        {
            return Mongo.UpdateByIdAsync(CollectionName, id, StampUpdate(update, UtcNow, by, fields));
        }

        /// <summary>
        /// Upserts, stamping UpdatedAt (and CreatedAt on insert) and recording who changed which fields.
        /// </summary>
        /// <param name="filter">The match predicate.</param>
        /// <param name="update">The caller's update definition.</param>
        /// <param name="by">Who made the change.</param>
        /// <param name="fields">The property names that changed.</param>
        /// <returns>The result of the upsert operation.</returns>
        public Task<UpdateResult> UpsertAsync(
            Expression<Func<TDoc, bool>> filter, UpdateDefinition<TDoc> update, string by, params string[] fields)
        {
            return Mongo.UpsertAsync(CollectionName, Builders<TDoc>.Filter.Where(filter), StampUpsert(update, UtcNow, by, fields));
        }

        /// <summary>
        /// Replaces the document, stamping UpdatedAt and recording who changed which fields.
        /// </summary>
        /// <param name="id">The document's <c>_id</c> value.</param>
        /// <param name="document">The new document.</param>
        /// <param name="by">Who made the change.</param>
        /// <param name="fields">The property names that changed.</param>
        /// <returns>The result of the replace operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
        public Task<ReplaceOneResult> ReplaceByIdAsync(string id, TDoc document, string by, params string[] fields)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }
            StampForReplace(document, UtcNow, by, fields);
            return Mongo.ReplaceByIdAsync(CollectionName, id, document);
        }

        /// <inheritdoc />
        protected override UpdateDefinition<TDoc> StampUpdate(
            UpdateDefinition<TDoc> update, DateTime now, string by, IReadOnlyList<string> fields)
        {
            UpdateStamp stamp = NewStamp(now, by, fields);
            return Builders<TDoc>.Update.Combine(
                base.StampUpdate(update, now, by, fields),
                Builders<TDoc>.Update.PushEach(d => d.UpdateHistory, new[] { stamp }, slice: -UpdateHistoryLimit));
        }

        /// <inheritdoc />
        protected override void StampForReplace(TDoc document, DateTime now, string by, IReadOnlyList<string> fields)
        {
            base.StampForReplace(document, now, by, fields);
            List<UpdateStamp> history = document.UpdateHistory ?? new List<UpdateStamp>();
            history.Add(NewStamp(now, by, fields));
            if (history.Count > UpdateHistoryLimit)
            {
                history.RemoveRange(0, history.Count - UpdateHistoryLimit);
            }
            document.UpdateHistory = history;
        }

        private static UpdateStamp NewStamp(DateTime now, string by, IReadOnlyList<string> fields)
        {
            return new UpdateStamp
            {
                At = now,
                By = string.IsNullOrWhiteSpace(by) ? SystemActor : by,
                Fields = fields?.ToList() ?? new List<string>(),
            };
        }
    }
}
