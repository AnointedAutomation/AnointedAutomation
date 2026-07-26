// Copyright 2024 Anointed Automation, LLC. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
// Stewarded by Alexander Fields

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AnointedAutomation.Repository.MySql
{
    /// <summary>
    /// Generic repository interface for CRUD operations on any entity type.
    /// Provides a consistent API for database operations regardless of entity type.
    /// </summary>
    /// <typeparam name="T">The entity type this repository manages.</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Gets an entity by its primary key.
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The entity if found, otherwise null.</returns>
        Task<T> GetByIdAsync(object id);

        /// <summary>
        /// Gets all entities of type T.
        /// </summary>
        /// <returns>An enumerable of all entities.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Finds entities that match the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression.</param>
        /// <returns>An enumerable of matching entities.</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Finds a single entity that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression.</param>
        /// <returns>The first matching entity, or null if none found.</returns>
        Task<T> FindSingleAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity with any database-generated values populated.</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Adds multiple entities to the database.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        /// <returns>The number of entities added.</returns>
        Task<int> AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="entity">The entity with updated values.</param>
        /// <returns>The updated entity.</returns>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Deletes an entity by its primary key.
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>True if the entity was deleted, false if not found.</returns>
        Task<bool> DeleteAsync(object id);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>True if the entity was deleted.</returns>
        Task<bool> DeleteAsync(T entity);

        /// <summary>
        /// Deletes multiple entities that match the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression.</param>
        /// <returns>The number of entities deleted.</returns>
        Task<int> DeleteRangeAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Checks if any entity matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression.</param>
        /// <returns>True if any matching entity exists.</returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Counts entities that match the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression, or null to count all.</param>
        /// <returns>The count of matching entities.</returns>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);

        // ---- Standard read queries (additive) ------------------------------

        /// <summary>
        /// Gets a page of entities: filter (optional), order by a key, then skip/take.
        /// </summary>
        /// <typeparam name="TKey">The ordering key type.</typeparam>
        /// <param name="predicate">The filter expression, or null for no filter.</param>
        /// <param name="orderBy">The key selector to order by.</param>
        /// <param name="descending">True to order descending; false for ascending.</param>
        /// <param name="skip">The number of entities to skip (offset).</param>
        /// <param name="take">The maximum number of entities to return.</param>
        /// <returns>The requested page of entities.</returns>
        Task<IEnumerable<T>> GetPagedAsync<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderBy, bool descending, int skip, int take);

        /// <summary>
        /// Gets entities (optionally filtered) ordered by a key.
        /// </summary>
        /// <typeparam name="TKey">The ordering key type.</typeparam>
        /// <param name="orderBy">The key selector to order by.</param>
        /// <param name="descending">True to order descending; false for ascending.</param>
        /// <param name="predicate">The filter expression, or null for no filter.</param>
        /// <returns>The ordered entities.</returns>
        Task<IEnumerable<T>> GetOrderedAsync<TKey>(Expression<Func<T, TKey>> orderBy, bool descending, Expression<Func<T, bool>> predicate = null);

        /// <summary>
        /// Projects matching entities to a smaller shape, selecting only what you need.
        /// </summary>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="predicate">The filter expression, or null for no filter.</param>
        /// <param name="selector">The projection selector.</param>
        /// <returns>The projected results.</returns>
        Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector);

        /// <summary>
        /// Returns true if any entity exists (optionally matching the predicate).
        /// </summary>
        /// <param name="predicate">The filter expression, or null to test for any row.</param>
        /// <returns>True if any matching entity exists.</returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate = null);

        // ---- Standard write queries (additive) -----------------------------

        /// <summary>
        /// Updates multiple entities in a single SaveChanges call.
        /// </summary>
        /// <param name="entities">The entities with updated values.</param>
        /// <returns>The number of state entries written to the database.</returns>
        Task<int> UpdateRangeAsync(IEnumerable<T> entities);
    }
}
