// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AnointedAutomation.Repository.Mongo
{
    /// <summary>
    /// An <see cref="IMongoHelper"/> that forwards every call to the helper returned by <see cref="Current"/> at call
    /// time (for example a per-tenant or per-request database). Derive from it instead of implementing
    /// <see cref="IMongoHelper"/> by hand, so members added to the interface later are forwarded without code changes.
    /// The database is chosen by routing, so the <c>database</c> and <c>dbName</c> setters throw by default.
    /// </summary>
    public abstract class DelegatingMongoHelper : IMongoHelper
    {
        /// <summary>The helper every call goes to right now. Must not return null.</summary>
        protected abstract IMongoHelper Current();

        /// <summary>The message of the <see cref="InvalidOperationException"/> thrown by the default setters.</summary>
        protected virtual string NeverSetMessage =>
            GetType().Name + " routes each call to its current helper; database and dbName are never set on it.";

        public virtual IMongoDatabase database
        {
            get => Current().database;
            set => throw new InvalidOperationException(NeverSetMessage);
        }

        public virtual string dbName
        {
            get => Current().dbName;
            set => throw new InvalidOperationException(NeverSetMessage);
        }

        public virtual Task CreateDocumentAsync<T>(string collectionName, T document) => Current().CreateDocumentAsync(collectionName, document);
        public virtual IMongoDatabase CreateMongoDbInstance(string dbName, string connectionString) => Current().CreateMongoDbInstance(dbName, connectionString);
        public virtual Task<DeleteResult> DeleteDocumentAsync<T>(string collectionName, FilterDefinition<T> filter) => Current().DeleteDocumentAsync(collectionName, filter);
        [Obsolete("Loads a whole collection into memory; page with GetDocumentIdsPageAsync / GetPagedAsync instead.")]
        public virtual Task<List<T>> GetAllDocumentsAsync<T>(string collectionName) => Current().GetAllDocumentsAsync<T>(collectionName);
        public virtual Task<List<string>> GetAllDocumentIdsAsync(string collectionName) => Current().GetAllDocumentIdsAsync(collectionName);
        public virtual Task<List<string>> GetDocumentIdsPageAsync(string collectionName, int skip, int limit) => Current().GetDocumentIdsPageAsync(collectionName, skip, limit);
        public virtual Task<List<T>> GetFilteredDocumentsAsync<T>(string collectionName, FilterDefinition<T> filter) => Current().GetFilteredDocumentsAsync(collectionName, filter);
        public virtual Task<ReplaceOneResult> ReplaceDocumentAsync<T>(string collectionName, FilterDefinition<T> filter, T document) => Current().ReplaceDocumentAsync(collectionName, filter, document);
        public virtual List<string> TestConnection() => Current().TestConnection();
        public virtual Task<UpdateResult> UpdateDocumentAsync<T>(string collectionName, FilterDefinition<T> filter, UpdateDefinition<T> updateDefinition) => Current().UpdateDocumentAsync(collectionName, filter, updateDefinition);
        public virtual Task<T> GetByIdAsync<T>(string collectionName, string id) => Current().GetByIdAsync<T>(collectionName, id);
        public virtual Task<T> GetSingleAsync<T>(string collectionName, FilterDefinition<T> filter) => Current().GetSingleAsync(collectionName, filter);
        public virtual Task<long> CountAsync<T>(string collectionName, FilterDefinition<T> filter) => Current().CountAsync(collectionName, filter);
        public virtual Task<bool> ExistsAsync<T>(string collectionName, FilterDefinition<T> filter) => Current().ExistsAsync(collectionName, filter);
        public virtual Task<List<T>> GetPagedAsync<T>(string collectionName, FilterDefinition<T> filter, SortDefinition<T> sort, int skip, int limit) => Current().GetPagedAsync(collectionName, filter, sort, skip, limit);
        public virtual Task<List<TProjection>> GetProjectedAsync<T, TProjection>(string collectionName, FilterDefinition<T> filter, ProjectionDefinition<T, TProjection> projection) => Current().GetProjectedAsync(collectionName, filter, projection);
        public virtual Task<List<TResult>> AggregateAsync<T, TResult>(string collectionName, PipelineDefinition<T, TResult> pipeline) => Current().AggregateAsync(collectionName, pipeline);
        public virtual Task<List<TField>> DistinctAsync<T, TField>(string collectionName, FieldDefinition<T, TField> field, FilterDefinition<T> filter) => Current().DistinctAsync(collectionName, field, filter);
        public virtual Task CreateManyAsync<T>(string collectionName, IEnumerable<T> documents) => Current().CreateManyAsync(collectionName, documents);
        public virtual Task<UpdateResult> UpdateManyAsync<T>(string collectionName, FilterDefinition<T> filter, UpdateDefinition<T> update) => Current().UpdateManyAsync(collectionName, filter, update);
        public virtual Task<UpdateResult> UpsertAsync<T>(string collectionName, FilterDefinition<T> filter, UpdateDefinition<T> update) => Current().UpsertAsync(collectionName, filter, update);
        public virtual Task<DeleteResult> DeleteManyAsync<T>(string collectionName, FilterDefinition<T> filter) => Current().DeleteManyAsync(collectionName, filter);
        public virtual Task<T> FindOneAndUpdateAsync<T>(string collectionName, FilterDefinition<T> filter, UpdateDefinition<T> update, bool returnUpdated = true, bool upsert = false) => Current().FindOneAndUpdateAsync(collectionName, filter, update, returnUpdated, upsert);
        public virtual Task<T> FindOneAndDeleteAsync<T>(string collectionName, FilterDefinition<T> filter) => Current().FindOneAndDeleteAsync(collectionName, filter);
        public virtual Task<ReplaceOneResult> ReplaceByIdAsync<T>(string collectionName, string id, T document) => Current().ReplaceByIdAsync(collectionName, id, document);
        public virtual Task<UpdateResult> UpdateByIdAsync<T>(string collectionName, string id, UpdateDefinition<T> update) => Current().UpdateByIdAsync(collectionName, id, update);
        public virtual Task<DeleteResult> DeleteByIdAsync<T>(string collectionName, string id) => Current().DeleteByIdAsync<T>(collectionName, id);
        public virtual Task<T> GetSingleAsync<T>(string collectionName, Expression<Func<T, bool>> predicate) => Current().GetSingleAsync(collectionName, predicate);
        public virtual Task<List<T>> GetFilteredDocumentsAsync<T>(string collectionName, Expression<Func<T, bool>> predicate) => Current().GetFilteredDocumentsAsync(collectionName, predicate);
        public virtual Task<DeleteResult> DeleteDocumentAsync<T>(string collectionName, Expression<Func<T, bool>> predicate) => Current().DeleteDocumentAsync(collectionName, predicate);
        public virtual Task<long> CountAsync<T>(string collectionName, Expression<Func<T, bool>> predicate) => Current().CountAsync(collectionName, predicate);
        public virtual Task<bool> ExistsAsync<T>(string collectionName, Expression<Func<T, bool>> predicate) => Current().ExistsAsync(collectionName, predicate);
        public virtual Task EnsureIndexAsync(string collectionName, MongoIndexSpec spec) => Current().EnsureIndexAsync(collectionName, spec);
        public virtual Task EnsureIndexesAsync(string collectionName, IEnumerable<MongoIndexSpec> specs) => Current().EnsureIndexesAsync(collectionName, specs);
    }
}
