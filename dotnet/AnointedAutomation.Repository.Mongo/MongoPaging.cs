// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using AnointedAutomation.Objects;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnointedAutomation.Repository.Mongo
{
    /// <summary>
    /// The one implementation of page-numbered reads that return a <see cref="PaginatedResponse{T}"/>: the page Find and a
    /// server-side count of the same filter run concurrently, so callers get the page and its totals from one call.
    /// </summary>
    public static class MongoPaging
    {
        /// <summary>The largest page size a page-numbered read accepts.</summary>
        public const int MaxPageSize = 1000;

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> unless page is 1 or more, pageSize is between 1 and
        /// <see cref="MaxPageSize"/>, and the resulting offset fits in an int.
        /// </summary>
        /// <param name="page">The 1-based page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>The number of items to skip.</returns>
        public static int ValidateAndGetSkip(int page, int pageSize)
        {
            if (page < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(page), page, "page must be 1 or more (pages are 1-based)");
            }
            if (pageSize < 1 || pageSize > MaxPageSize)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, "pageSize must be between 1 and " + MaxPageSize);
            }
            long skip = (long)(page - 1) * pageSize;
            if (skip > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(page), page, "page * pageSize is too large");
            }
            return (int)skip;
        }

        /// <summary>
        /// Computes the page count with integer ceiling; 0 items gives 0 pages.
        /// </summary>
        /// <param name="totalItems">The total number of matching items.</param>
        /// <param name="pageSize">The number of items per page (1 or more).</param>
        /// <returns>The number of pages.</returns>
        public static int TotalPages(long totalItems, int pageSize)
        {
            return (int)((totalItems + pageSize - 1) / pageSize);
        }

        /// <summary>
        /// Reads one page of documents matching the filter in the optional sort order, with the total count of the filter.
        /// </summary>
        /// <typeparam name="T">The document type.</typeparam>
        /// <param name="collection">The collection to read.</param>
        /// <param name="filter">The filter to match; required.</param>
        /// <param name="sort">The sort definition, or null for natural order.</param>
        /// <param name="page">The 1-based page number.</param>
        /// <param name="pageSize">The number of items per page (1 to <see cref="MaxPageSize"/>).</param>
        /// <returns>The page and its totals.</returns>
        public static async Task<PaginatedResponse<T>> GetPageAsync<T>(IMongoCollection<T> collection, FilterDefinition<T> filter, SortDefinition<T> sort, int page, int pageSize)
        {
            ArgumentNullException.ThrowIfNull(collection);
            ArgumentNullException.ThrowIfNull(filter);
            int skip = ValidateAndGetSkip(page, pageSize);
            IFindFluent<T, T> find = collection.Find(filter);
            if (sort != null)
            {
                find = find.Sort(sort);
            }
            Task<List<T>> itemsTask = find.Skip(skip).Limit(pageSize).ToListAsync();
            Task<long> countTask = collection.CountDocumentsAsync(filter);
            await Task.WhenAll(itemsTask, countTask);
            return Build(itemsTask.Result, page, pageSize, countTask.Result);
        }

        /// <summary>
        /// Reads one page of document ids ordered by _id (projected to _id only), with the collection's total count.
        /// Ids are strings (an ObjectId becomes its 24-char hex form).
        /// </summary>
        /// <param name="collection">The collection to read.</param>
        /// <param name="page">The 1-based page number.</param>
        /// <param name="pageSize">The number of ids per page (1 to <see cref="MaxPageSize"/>).</param>
        /// <returns>The page of ids and its totals.</returns>
        public static async Task<PaginatedResponse<string>> GetDocumentIdPageAsync(IMongoCollection<BsonDocument> collection, int page, int pageSize)
        {
            ArgumentNullException.ThrowIfNull(collection);
            int skip = ValidateAndGetSkip(page, pageSize);
            ProjectionDefinition<BsonDocument> projection = Builders<BsonDocument>.Projection.Include("_id");
            SortDefinition<BsonDocument> sort = Builders<BsonDocument>.Sort.Ascending("_id");
            Task<List<BsonDocument>> idsTask = collection.Find(FilterDefinition<BsonDocument>.Empty)
                .Sort(sort)
                .Skip(skip)
                .Limit(pageSize)
                .Project(projection)
                .ToListAsync();
            Task<long> countTask = collection.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty);
            await Task.WhenAll(idsTask, countTask);
            List<string> ids = idsTask.Result.ConvertAll(doc => doc["_id"].ToString());
            return Build(ids, page, pageSize, countTask.Result);
        }

        private static PaginatedResponse<TItem> Build<TItem>(List<TItem> items, int page, int pageSize, long totalItems)
        {
            return new PaginatedResponse<TItem>
            {
                Data = items,
                currentPage = page,
                pageSize = pageSize,
                totalItems = totalItems,
                totalPages = TotalPages(totalItems, pageSize)
            };
        }
    }
}
