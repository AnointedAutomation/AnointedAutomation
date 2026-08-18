// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AnointedAutomation.Repository.Mongo
{
    /// <summary>
    /// Abstract generic repository base class for a single MongoDB collection. Subclass this per
    /// document type instead of hand-writing the same CRUD delegation in every repository. All
    /// methods delegate to the injected <see cref="IMongoHelper"/> using the collection name given
    /// at construction, so subclasses only add their document-specific queries.
    /// </summary>
    /// <typeparam name="TDoc">The document type this repository manages.</typeparam>
    public abstract class MongoRepository<TDoc>
    {
        /// <summary>
        /// Initializes the repository with the helper and collection it operates on.
        /// </summary>
        /// <param name="mongo">The Mongo helper to delegate all operations to.</param>
        /// <param name="collectionName">The name of the collection this repository manages.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mongo"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="collectionName"/> is null or whitespace.</exception>
        protected MongoRepository(IMongoHelper mongo, string collectionName)
        {
            if (mongo == null)
            {
                throw new ArgumentNullException(nameof(mongo));
            }
            if (string.IsNullOrWhiteSpace(collectionName))
            {
                throw new ArgumentException("Collection name is required.", nameof(collectionName));
            }
            Mongo = mongo;
            CollectionName = collectionName;
        }

        /// <summary>
        /// Gets the Mongo helper this repository delegates to.
        /// </summary>
        protected IMongoHelper Mongo { get; }

        /// <summary>
        /// Gets the name of the collection this repository manages.
        /// </summary>
        protected string CollectionName { get; }

        /// <summary>
        /// Gets a single document by its <c>_id</c> (ObjectId- or string-key safe).
        /// </summary>
        /// <param name="id">The document's <c>_id</c> value.</param>
        /// <returns>The matching document, or null if none matches.</returns>
        public Task<TDoc> GetByIdAsync(string id)
        {
            return Mongo.GetByIdAsync<TDoc>(CollectionName, id);
        }

        /// <summary>
        /// Gets the first document matching the predicate, or null if none matches.
        /// </summary>
        /// <param name="filter">The match predicate.</param>
        /// <returns>The first matching document, or null.</returns>
        public Task<TDoc> GetSingleAsync(Expression<Func<TDoc, bool>> filter)
        {
            return Mongo.GetSingleAsync(CollectionName, filter);
        }

        /// <summary>
        /// Gets documents matching the predicate.
        /// </summary>
        /// <param name="filter">The match predicate.</param>
        /// <returns>A list of matching documents.</returns>
        public Task<List<TDoc>> GetFilteredAsync(Expression<Func<TDoc, bool>> filter)
        {
            return Mongo.GetFilteredDocumentsAsync(CollectionName, filter);
        }

        /// <summary>
        /// WARNING: loads EVERY document in the collection into memory. Only use for small
        /// collections (see <see cref="IMongoHelper.GetAllDocumentsAsync{T}"/>).
        /// </summary>
        /// <returns>A list of all documents in the collection.</returns>
        public Task<List<TDoc>> GetAllAsync()
        {
            return Mongo.GetAllDocumentsAsync<TDoc>(CollectionName);
        }

        /// <summary>
        /// Gets a page of documents matching the filter, applying an optional sort then skip/limit.
        /// </summary>
        /// <param name="filter">The filter to match.</param>
        /// <param name="sort">The sort definition, or null for natural order.</param>
        /// <param name="skip">The number of documents to skip (offset).</param>
        /// <param name="limit">The maximum number of documents to return.</param>
        /// <returns>The page of matching documents.</returns>
        public Task<List<TDoc>> GetPagedAsync(FilterDefinition<TDoc> filter, SortDefinition<TDoc> sort, int skip, int limit)
        {
            return Mongo.GetPagedAsync(CollectionName, filter, sort, skip, limit);
        }

        /// <summary>
        /// Counts documents matching the predicate WITHOUT loading them.
        /// </summary>
        /// <param name="filter">The match predicate.</param>
        /// <returns>The number of matching documents.</returns>
        public Task<long> CountAsync(Expression<Func<TDoc, bool>> filter)
        {
            return Mongo.CountAsync(CollectionName, filter);
        }

        /// <summary>
        /// Returns true if at least one document matches the predicate (stops at the first match).
        /// </summary>
        /// <param name="filter">The match predicate.</param>
        /// <returns>True if any document matches; otherwise false.</returns>
        public Task<bool> ExistsAsync(Expression<Func<TDoc, bool>> filter)
        {
            return Mongo.ExistsAsync(CollectionName, filter);
        }

        /// <summary>
        /// Creates a new document in the collection.
        /// </summary>
        /// <param name="document">The document to create.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task CreateAsync(TDoc document)
        {
            return Mongo.CreateDocumentAsync(CollectionName, document);
        }

        /// <summary>
        /// Updates the single document matching the predicate, inserting it if none exists (upsert).
        /// </summary>
        /// <param name="filter">The match predicate.</param>
        /// <param name="update">The update definition (applied on both update and insert).</param>
        /// <returns>The result of the upsert operation.</returns>
        public Task<UpdateResult> UpsertAsync(Expression<Func<TDoc, bool>> filter, UpdateDefinition<TDoc> update)
        {
            return Mongo.UpsertAsync(CollectionName, Builders<TDoc>.Filter.Where(filter), update);
        }

        /// <summary>
        /// Replaces the document whose <c>_id</c> equals <paramref name="id"/> (ObjectId- or string-key safe).
        /// </summary>
        /// <param name="id">The document's <c>_id</c> value.</param>
        /// <param name="document">The new document.</param>
        /// <returns>The result of the replace operation.</returns>
        public Task<ReplaceOneResult> ReplaceByIdAsync(string id, TDoc document)
        {
            return Mongo.ReplaceByIdAsync(CollectionName, id, document);
        }

        /// <summary>
        /// Updates the document whose <c>_id</c> equals <paramref name="id"/> (ObjectId- or string-key safe).
        /// </summary>
        /// <param name="id">The document's <c>_id</c> value.</param>
        /// <param name="update">The update definition.</param>
        /// <returns>The result of the update operation.</returns>
        public Task<UpdateResult> UpdateByIdAsync(string id, UpdateDefinition<TDoc> update)
        {
            return Mongo.UpdateByIdAsync(CollectionName, id, update);
        }

        /// <summary>
        /// Deletes the document whose <c>_id</c> equals <paramref name="id"/> (ObjectId- or string-key safe).
        /// </summary>
        /// <param name="id">The document's <c>_id</c> value.</param>
        /// <returns>The result of the delete operation.</returns>
        public Task<DeleteResult> DeleteByIdAsync(string id)
        {
            return Mongo.DeleteByIdAsync<TDoc>(CollectionName, id);
        }

        /// <summary>
        /// Deletes EVERY document matching the predicate.
        /// </summary>
        /// <param name="filter">The match predicate.</param>
        /// <returns>The result of the delete operation.</returns>
        public Task<DeleteResult> DeleteManyAsync(Expression<Func<TDoc, bool>> filter)
        {
            return Mongo.DeleteManyAsync(CollectionName, Builders<TDoc>.Filter.Where(filter));
        }
    }
}
