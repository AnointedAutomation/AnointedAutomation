// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-10-12 10:20:40
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

using MongoDB.Driver;
using System.Collections.Generic;
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
        /// WARNING: THIS METHOD IS DANGEROUS AND CAN CAUSE OutOfMemoryException!
        /// Loads ALL documents with ALL fields into memory. For large collections or
        /// collections with binary data, this WILL crash your application.
        ///
        /// USE INSTEAD:
        /// - GetAllDocumentIdsAsync() to get only IDs (safe)
        /// - GetFilteredDocumentsAsync() with a filter to limit results
        /// - Use projection to exclude large fields like binary content
        ///
        /// Only use this method for small collections with small documents.
        /// </summary>
        /// <typeparam name="T">The type of documents to retrieve.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <returns>A list of all documents in the collection.</returns>
        Task<List<T>> GetAllDocumentsAsync<T>(string collectionName);

        /// <summary>
        /// Gets all document IDs from the specified collection WITHOUT loading full documents.
        /// This is the SAFE alternative to GetAllDocumentsAsync - use this when you need to
        /// iterate over all documents but don't need the full document data immediately.
        /// </summary>
        /// <param name="collectionName">The name of the collection.</param>
        /// <returns>A list of all document IDs in the collection.</returns>
        Task<List<string>> GetAllDocumentIdsAsync(string collectionName);

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
        /// <param name="limit">The maximum number of documents to return.</param>
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
    }
}
