// Copyright © Anointed Automation Corporation., 2024. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-10-12 10:20:40 Jesus is King ✝️
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

using AnointedAutomation.Objects;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AnointedAutomation.Repository.Mongo
{
    /// <summary>
    /// Interface for MongoDB database operations.
    /// Provides methods for CRUD operations, connection management, and database interactions.
    /// Make sure you set the MongoDB Instance before using the implementation.
    /// </summary>
    public interface IMongoHelper
    {
        /// <summary>
        /// Gets or sets the MongoDB database instance.
        /// </summary>
        IMongoDatabase database { get; set; }

        /// <summary>
        /// Gets or sets the name of the MongoDB database.
        /// </summary>
        public string dbName { get; set; }

        /// <summary>
        /// Creates a new document in the specified collection.
        /// </summary>
        /// <typeparam name="T">The type of document to create.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="document">The document to create.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CreateDocumentAsync<T>(string collectionName, T document);

        /// <summary>
        /// Creates a new MongoDB database instance.
        /// </summary>
        /// <param name="dbName">The name of the database.</param>
        /// <param name="connectionString">The MongoDB connection string.</param>
        /// <returns>The MongoDB database instance.</returns>
        IMongoDatabase CreateMongoDbInstance(string dbName, string connectionString);

        /// <summary>
        /// Deletes a document from the specified collection that matches the filter.
        /// </summary>
        /// <typeparam name="T">The type of document to delete.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match documents for deletion.</param>
        /// <returns>The result of the delete operation.</returns>
        Task<DeleteResult> DeleteDocumentAsync<T>(string collectionName, FilterDefinition<T> filter);

        /// <summary>
        /// OBSOLETE: loads EVERY document of the collection, with every field, into application RAM at once.
        /// That breaks the "no RAM-resident data" rule: a growing collection becomes an OutOfMemoryException.
        /// Use a bounded read instead:
        /// <see cref="GetDocumentIdsPageAsync"/> to walk ids page by page,
        /// <see cref="GetPagedAsync{T}"/> to walk documents page by page, or
        /// <see cref="GetFilteredDocumentsAsync{T}(string, FilterDefinition{T}, SortDefinition{T}, int)"/> with a filter and a limit.
        /// </summary>
        /// <typeparam name="T">The type of documents to retrieve.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <returns>A list of all documents in the collection.</returns>
        [Obsolete(MongoObsoleteMessages.WholeCollectionRead)]
        Task<List<T>> GetAllDocumentsAsync<T>(string collectionName);

        /// <summary>
        /// Gets all document IDs from the specified collection WITHOUT loading full documents.
        /// This is the SAFE alternative to GetAllDocumentsAsync - use this when you need to
        /// iterate over all documents but don't need the full document data immediately.
        /// Ids are returned as strings (an ObjectId becomes its 24-char hex form). This still reads every id,
        /// so for large collections use <see cref="GetDocumentIdsPageAsync"/>.
        /// </summary>
        /// <param name="collectionName">The name of the collection.</param>
        /// <returns>A list of all document IDs in the collection.</returns>
        Task<List<string>> GetAllDocumentIdsAsync(string collectionName);

        /// <summary>
        /// Gets one page of document IDs ordered by _id, without loading full documents.
        /// Ids are returned as strings (an ObjectId becomes its 24-char hex form).
        /// </summary>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="skip">The number of ids to skip (0 or more).</param>
        /// <param name="limit">The maximum number of ids to return (1 or more).</param>
        /// <returns>The page of document IDs.</returns>
        Task<List<string>> GetDocumentIdsPageAsync(string collectionName, int skip, int limit);

        /// <summary>
        /// Gets documents from the specified collection that match the filter.
        /// </summary>
        /// <typeparam name="T">The type of documents to retrieve.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match documents.</param>
        /// <returns>A list of matching documents.</returns>
        Task<List<T>> GetFilteredDocumentsAsync<T>(string collectionName, FilterDefinition<T> filter);

        /// <summary>
        /// Replaces a document in the specified collection that matches the filter.
        /// </summary>
        /// <typeparam name="T">The type of document to replace.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match the document to replace.</param>
        /// <param name="document">The new document.</param>
        /// <returns>The result of the replace operation.</returns>
        Task<ReplaceOneResult> ReplaceDocumentAsync<T>(string collectionName, FilterDefinition<T> filter, T document);

        /// <summary>
        /// Tests the connection to the database
        /// </summary>
        /// <returns>Will return a List of Collection Names if it worked otherwise returns null</returns>
        List<string> TestConnection();

        /// <summary>
        /// Updates a document in the specified collection that matches the filter.
        /// </summary>
        /// <typeparam name="T">The type of document to update.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match the document to update.</param>
        /// <param name="updateDefinition">The update definition.</param>
        /// <returns>The result of the update operation.</returns>
        Task<UpdateResult> UpdateDocumentAsync<T>(string collectionName, FilterDefinition<T> filter, UpdateDefinition<T> updateDefinition);

        // ---- Standard read queries (additive) ------------------------------

        /// <summary>
        /// Gets a single document by its <c>_id</c>. Accepts an ObjectId hex string (matched as an
        /// ObjectId) or any other string (matched as-is), so it works for both ObjectId and string keys.
        /// </summary>
        /// <typeparam name="T">The type of document to retrieve.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="id">The document's <c>_id</c> value.</param>
        /// <returns>The matching document, or the type default (null) if none matches.</returns>
        Task<T> GetByIdAsync<T>(string collectionName, string id);

        /// <summary>
        /// Gets the first document matching the filter, or the type default (null) if none matches.
        /// </summary>
        /// <typeparam name="T">The type of document to retrieve.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match.</param>
        /// <returns>The first matching document, or null.</returns>
        Task<T> GetSingleAsync<T>(string collectionName, FilterDefinition<T> filter);

        /// <summary>
        /// Counts documents matching the filter WITHOUT loading them. Pass
        /// <see cref="FilterDefinition{T}.Empty"/> to count the whole collection.
        /// </summary>
        /// <typeparam name="T">The type of document.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match.</param>
        /// <returns>The number of matching documents.</returns>
        Task<long> CountAsync<T>(string collectionName, FilterDefinition<T> filter);

        /// <summary>
        /// Returns true if at least one document matches the filter (stops at the first match).
        /// </summary>
        /// <typeparam name="T">The type of document.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match.</param>
        /// <returns>True if any document matches; otherwise false.</returns>
        Task<bool> ExistsAsync<T>(string collectionName, FilterDefinition<T> filter);

        /// <summary>
        /// Gets a page of documents matching the filter, applying an optional sort then skip/limit.
        /// </summary>
        /// <typeparam name="T">The type of documents to retrieve.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match.</param>
        /// <param name="sort">The sort definition, or null for natural order.</param>
        /// <param name="skip">The number of documents to skip (offset).</param>
        /// <param name="limit">The maximum number of documents to return (1 or more; 0 or less throws).</param>
        /// <returns>The page of matching documents.</returns>
        Task<List<T>> GetPagedAsync<T>(string collectionName, FilterDefinition<T> filter, SortDefinition<T> sort, int skip, int limit);

        /// <summary>
        /// Gets documents matching the filter, projected to a smaller shape (fetch only needed fields).
        /// </summary>
        /// <typeparam name="T">The source document type.</typeparam>
        /// <typeparam name="TProjection">The projected result type.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match.</param>
        /// <param name="projection">The projection definition.</param>
        /// <returns>The projected results.</returns>
        Task<List<TProjection>> GetProjectedAsync<T, TProjection>(string collectionName, FilterDefinition<T> filter, ProjectionDefinition<T, TProjection> projection);

        /// <summary>
        /// Gets at most <paramref name="limit"/> documents matching the filter, in the optional sort order. Use this instead of
        /// the unbounded <see cref="GetFilteredDocumentsAsync{T}(string, FilterDefinition{T})"/> on collections that can grow.
        /// </summary>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match.</param>
        /// <param name="sort">The sort definition, or null for natural order.</param>
        /// <param name="limit">The maximum number of documents to return (1 or more; 0 or less throws).</param>
        Task<List<T>> GetFilteredDocumentsAsync<T>(string collectionName, FilterDefinition<T> filter, SortDefinition<T> sort, int limit);

        /// <summary>
        /// Gets the first document matching the filter under the given collation (for example case-insensitive
        /// matching), or the type default (null) if none matches.
        /// </summary>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match.</param>
        /// <param name="collation">The collation used to compare strings; required.</param>
        async Task<T> GetSingleAsync<T>(string collectionName, FilterDefinition<T> filter, Collation collation)
        {
            ArgumentNullException.ThrowIfNull(collation);
            return await database.GetCollection<T>(collectionName).Find(filter, new FindOptions { Collation = collation }).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets at most <paramref name="limit"/> documents matching the filter under the given collation, in the optional
        /// sort order (the collation also orders strings in the sort).
        /// </summary>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match.</param>
        /// <param name="sort">The sort definition, or null for natural order.</param>
        /// <param name="limit">The maximum number of documents to return (1 or more; 0 or less throws).</param>
        /// <param name="collation">The collation used to compare strings; required.</param>
        async Task<List<T>> GetFilteredDocumentsAsync<T>(string collectionName, FilterDefinition<T> filter, SortDefinition<T> sort, int limit, Collation collation)
        {
            ArgumentNullException.ThrowIfNull(collation);
            ArgumentOutOfRangeException.ThrowIfLessThan(limit, 1);
            IFindFluent<T, T> find = database.GetCollection<T>(collectionName).Find(filter, new FindOptions { Collation = collation });
            if (sort != null)
            {
                find = find.Sort(sort);
            }
            return await find.Limit(limit).ToListAsync();
        }

        /// <summary>
        /// Counts documents matching the filter under the given collation WITHOUT loading them.
        /// </summary>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match.</param>
        /// <param name="collation">The collation used to compare strings; required.</param>
        async Task<long> CountAsync<T>(string collectionName, FilterDefinition<T> filter, Collation collation)
        {
            ArgumentNullException.ThrowIfNull(collation);
            return await database.GetCollection<T>(collectionName).CountDocumentsAsync(filter, new CountOptions { Collation = collation });
        }

        /// <summary>
        /// Returns true if at least one document matches the filter under the given collation (stops at the first match).
        /// </summary>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match.</param>
        /// <param name="collation">The collation used to compare strings; required.</param>
        async Task<bool> ExistsAsync<T>(string collectionName, FilterDefinition<T> filter, Collation collation)
        {
            ArgumentNullException.ThrowIfNull(collation);
            long count = await database.GetCollection<T>(collectionName).CountDocumentsAsync(filter, new CountOptions { Collation = collation, Limit = 1 });
            return count > 0;
        }

        /// <summary>
        /// Gets at most <paramref name="limit"/> documents matching the filter, projected to a smaller shape so only the
        /// needed fields leave the server.
        /// </summary>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match.</param>
        /// <param name="projection">The projection definition.</param>
        /// <param name="sort">The sort definition, or null for natural order.</param>
        /// <param name="limit">The maximum number of documents to return (1 or more; 0 or less throws).</param>
        Task<List<TProjection>> GetProjectedAsync<T, TProjection>(string collectionName, FilterDefinition<T> filter, ProjectionDefinition<T, TProjection> projection, SortDefinition<T> sort, int limit);

        /// <summary>
        /// Runs many inserts, updates, replaces and deletes as bulk writes, <see cref="MongoHelper.MaxBulkWriteBatch"/> models per
        /// round trip. Unordered by default: a failing model does not stop the rest of its batch or the later batches, and
        /// after every batch ran a <see cref="BulkWriteFailedException"/> lists each failed model (index in the whole list) plus
        /// the counts that WERE applied. Ordered stops at the first failing batch with the same exception. An empty list makes
        /// no call.
        /// </summary>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="models">The write models.</param>
        /// <param name="ordered">True to stop at the first failure.</param>
        /// <returns>The summed counts of every batch.</returns>
        Task<BulkWriteSummary> BulkWriteAsync<T>(string collectionName, IEnumerable<WriteModel<T>> models, bool ordered = false);

        /// <summary>
        /// Runs an aggregation pipeline on the collection and returns the results.
        /// </summary>
        /// <typeparam name="T">The source document type.</typeparam>
        /// <typeparam name="TResult">The pipeline result type.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="pipeline">The aggregation pipeline.</param>
        /// <returns>The pipeline results.</returns>
        Task<List<TResult>> AggregateAsync<T, TResult>(string collectionName, PipelineDefinition<T, TResult> pipeline);

        /// <summary>
        /// Gets the distinct values of a field across documents matching the filter.
        /// </summary>
        /// <typeparam name="T">The source document type.</typeparam>
        /// <typeparam name="TField">The field value type.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="field">The field to read distinct values from.</param>
        /// <param name="filter">The filter to match.</param>
        /// <returns>The distinct field values.</returns>
        Task<List<TField>> DistinctAsync<T, TField>(string collectionName, FieldDefinition<T, TField> field, FilterDefinition<T> filter);

        // ---- Standard write queries (additive) -----------------------------

        /// <summary>
        /// Inserts many documents in a single round trip.
        /// </summary>
        /// <typeparam name="T">The type of documents to insert.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="documents">The documents to insert.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CreateManyAsync<T>(string collectionName, IEnumerable<T> documents);

        /// <summary>
        /// Applies an update to EVERY document matching the filter.
        /// </summary>
        /// <typeparam name="T">The type of document to update.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match documents to update.</param>
        /// <param name="update">The update definition.</param>
        /// <returns>The result of the update operation.</returns>
        Task<UpdateResult> UpdateManyAsync<T>(string collectionName, FilterDefinition<T> filter, UpdateDefinition<T> update);

        /// <summary>
        /// Updates the single document matching the filter, inserting it if none exists (upsert).
        /// </summary>
        /// <typeparam name="T">The type of document.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match the document.</param>
        /// <param name="update">The update definition (applied on both update and insert).</param>
        /// <returns>The result of the upsert operation.</returns>
        Task<UpdateResult> UpsertAsync<T>(string collectionName, FilterDefinition<T> filter, UpdateDefinition<T> update);

        /// <summary>
        /// Deletes EVERY document matching the filter.
        /// </summary>
        /// <typeparam name="T">The type of document to delete.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match documents for deletion.</param>
        /// <returns>The result of the delete operation.</returns>
        Task<DeleteResult> DeleteManyAsync<T>(string collectionName, FilterDefinition<T> filter);

        /// <summary>
        /// Atomically finds one document matching the filter and applies an update, optionally
        /// upserting. Returns the document after the update by default; returns the type default (null)
        /// when no document matched and <paramref name="upsert"/> is false.
        /// </summary>
        /// <typeparam name="T">The type of document.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match the document.</param>
        /// <param name="update">The update definition.</param>
        /// <param name="returnUpdated">True to return the post-update document; false for the pre-update document.</param>
        /// <param name="upsert">True to insert the document when none matches.</param>
        /// <returns>The matched document (before or after the update), or null.</returns>
        Task<T> FindOneAndUpdateAsync<T>(string collectionName, FilterDefinition<T> filter, UpdateDefinition<T> update, bool returnUpdated = true, bool upsert = false);

        /// <summary>
        /// Atomically finds one document matching the filter and deletes it, returning the deleted
        /// document (or the type default/null if none matched).
        /// </summary>
        /// <typeparam name="T">The type of document.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match the document.</param>
        /// <returns>The deleted document, or null.</returns>
        Task<T> FindOneAndDeleteAsync<T>(string collectionName, FilterDefinition<T> filter);

        // ---- Convenience: write-by-id (additive) ---------------------------

        /// <summary>
        /// Replaces the document whose <c>_id</c> equals <paramref name="id"/>. An ObjectId hex string is
        /// matched as an ObjectId; any other string is matched as-is, so this works for both ObjectId and
        /// string keys (mirrors <see cref="GetByIdAsync{T}"/>).
        /// </summary>
        /// <typeparam name="T">The type of document to replace.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="id">The document's <c>_id</c> value.</param>
        /// <param name="document">The new document.</param>
        /// <returns>The result of the replace operation.</returns>
        Task<ReplaceOneResult> ReplaceByIdAsync<T>(string collectionName, string id, T document);

        /// <summary>
        /// Updates the document whose <c>_id</c> equals <paramref name="id"/>. Id matching follows
        /// <see cref="GetByIdAsync{T}"/> (ObjectId hex matched as ObjectId, otherwise matched as-is).
        /// </summary>
        /// <typeparam name="T">The type of document to update.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="id">The document's <c>_id</c> value.</param>
        /// <param name="update">The update definition.</param>
        /// <returns>The result of the update operation.</returns>
        Task<UpdateResult> UpdateByIdAsync<T>(string collectionName, string id, UpdateDefinition<T> update);

        /// <summary>
        /// Deletes the document whose <c>_id</c> equals <paramref name="id"/>. Id matching follows
        /// <see cref="GetByIdAsync{T}"/> (ObjectId hex matched as ObjectId, otherwise matched as-is).
        /// </summary>
        /// <typeparam name="T">The type of document to delete.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="id">The document's <c>_id</c> value.</param>
        /// <returns>The result of the delete operation.</returns>
        Task<DeleteResult> DeleteByIdAsync<T>(string collectionName, string id);

        // ---- Convenience: expression-predicate overloads (additive) --------

        /// <summary>
        /// Gets the first document matching the predicate, or the type default (null) if none matches.
        /// Convenience overload so callers can pass <c>x =&gt; x.Field == value</c> instead of building a
        /// <see cref="FilterDefinition{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of document to retrieve.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="predicate">The match predicate.</param>
        /// <returns>The first matching document, or null.</returns>
        Task<T> GetSingleAsync<T>(string collectionName, Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Gets documents matching the predicate. Convenience overload over the
        /// <see cref="FilterDefinition{T}"/> version.
        /// </summary>
        /// <typeparam name="T">The type of documents to retrieve.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="predicate">The match predicate.</param>
        /// <returns>A list of matching documents.</returns>
        Task<List<T>> GetFilteredDocumentsAsync<T>(string collectionName, Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Deletes a single document matching the predicate. Convenience overload over the
        /// <see cref="FilterDefinition{T}"/> version.
        /// </summary>
        /// <typeparam name="T">The type of document to delete.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="predicate">The match predicate.</param>
        /// <returns>The result of the delete operation.</returns>
        Task<DeleteResult> DeleteDocumentAsync<T>(string collectionName, Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Counts documents matching the predicate. Convenience overload over the
        /// <see cref="FilterDefinition{T}"/> version.
        /// </summary>
        /// <typeparam name="T">The type of document.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="predicate">The match predicate.</param>
        /// <returns>The number of matching documents.</returns>
        Task<long> CountAsync<T>(string collectionName, Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Returns true if at least one document matches the predicate. Convenience overload over the
        /// <see cref="FilterDefinition{T}"/> version.
        /// </summary>
        /// <typeparam name="T">The type of document.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="predicate">The match predicate.</param>
        /// <returns>True if any document matches; otherwise false.</returns>
        Task<bool> ExistsAsync<T>(string collectionName, Expression<Func<T, bool>> predicate);

        // ---- Index management (additive) -----------------------------------

        /// <summary>
        /// Ensures the index described by the spec exists on the collection. Uses a deterministic
        /// index name (see <see cref="MongoIndexSpec.ResolveName"/>) so repeated calls with the same
        /// spec are idempotent.
        /// </summary>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="spec">The index specification.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task EnsureIndexAsync(string collectionName, MongoIndexSpec spec);

        /// <summary>
        /// Ensures every index in the sequence exists on the collection. Convenience over calling
        /// <see cref="EnsureIndexAsync"/> per spec.
        /// </summary>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="specs">The index specifications.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task EnsureIndexesAsync(string collectionName, IEnumerable<MongoIndexSpec> specs);

        /// <summary>
        /// Gets one 1-based page of documents matching the filter, in the optional sort order, together with the total
        /// count of the filter (counted on the server, concurrently with the page read) and the page count.
        /// </summary>
        /// <typeparam name="T">The type of documents to retrieve.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match; required.</param>
        /// <param name="sort">The sort definition, or null for natural order.</param>
        /// <param name="page">The 1-based page number (1 or more; less throws).</param>
        /// <param name="pageSize">The number of items per page (1 to <see cref="MongoPaging.MaxPageSize"/>; outside throws).</param>
        /// <returns>The page of documents with currentPage, pageSize, totalItems and totalPages.</returns>
        Task<PaginatedResponse<T>> GetPageAsync<T>(string collectionName, FilterDefinition<T> filter, SortDefinition<T> sort, int page, int pageSize)
        {
            return MongoPaging.GetPageAsync(database.GetCollection<T>(collectionName), filter, sort, page, pageSize);
        }

        /// <summary>
        /// Gets one 1-based page of document ids ordered by _id (projected to _id only), together with the collection's
        /// total and the page count. The total comes from EstimatedDocumentCountAsync (metadata, O(1) per page), so
        /// totalItems and totalPages are an estimate; use <see cref="CountAsync{T}(string, FilterDefinition{T})"/> for an exact count. Named differently from the skip/limit <see cref="GetDocumentIdsPageAsync"/>,
        /// which has the same parameter types.
        /// </summary>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="page">The 1-based page number (1 or more; less throws).</param>
        /// <param name="pageSize">The number of ids per page (1 to <see cref="MongoPaging.MaxPageSize"/>; outside throws).</param>
        /// <returns>The page of ids with currentPage, pageSize, totalItems and totalPages.</returns>
        Task<PaginatedResponse<string>> GetDocumentIdPageAsync(string collectionName, int page, int pageSize)
        {
            return MongoPaging.GetDocumentIdPageAsync(database.GetCollection<MongoDB.Bson.BsonDocument>(collectionName), page, pageSize);
        }
    }
}
