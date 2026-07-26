// Copyright 2024 Anointed Automation, LLC. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
// Stewarded by Alexander Fields

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AnointedAutomation.Repository.MySql
{
    /// <summary>
    /// Interface for MySQL database operations using Entity Framework Core.
    /// Provides methods for CRUD operations, connection management, and database interactions.
    /// </summary>
    public interface IMySqlHelper
    {
        /// <summary>
        /// Gets or sets the Entity Framework DbContext instance.
        /// </summary>
        DbContext Database { get; set; }

        /// <summary>
        /// Gets or sets the name of the MySQL database.
        /// </summary>
        string DbName { get; set; }

        /// <summary>
        /// Gets or sets the connection string for the MySQL database.
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// Creates a new entity in the database.
        /// </summary>
        /// <typeparam name="T">The type of entity to create.</typeparam>
        /// <param name="entity">The entity to create.</param>
        /// <returns>The created entity with any database-generated values populated.</returns>
        Task<T> CreateAsync<T>(T entity) where T : class;

        /// <summary>
        /// Gets an entity by its primary key.
        /// </summary>
        /// <typeparam name="T">The type of entity to retrieve.</typeparam>
        /// <param name="id">The primary key value.</param>
        /// <returns>The entity if found, otherwise null.</returns>
        Task<T> GetByIdAsync<T>(object id) where T : class;

        /// <summary>
        /// Gets all entities of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of entities to retrieve.</typeparam>
        /// <returns>A list of all entities.</returns>
        Task<List<T>> GetAllAsync<T>() where T : class;

        /// <summary>
        /// Gets entities that match the specified predicate.
        /// </summary>
        /// <typeparam name="T">The type of entities to retrieve.</typeparam>
        /// <param name="predicate">The filter expression.</param>
        /// <returns>A list of matching entities.</returns>
        Task<List<T>> GetFilteredAsync<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <typeparam name="T">The type of entity to update.</typeparam>
        /// <param name="entity">The entity with updated values.</param>
        /// <returns>The updated entity.</returns>
        Task<T> UpdateAsync<T>(T entity) where T : class;

        /// <summary>
        /// Deletes an entity by its primary key.
        /// </summary>
        /// <typeparam name="T">The type of entity to delete.</typeparam>
        /// <param name="id">The primary key value.</param>
        /// <returns>True if the entity was deleted, false if not found.</returns>
        Task<bool> DeleteAsync<T>(object id) where T : class;

        /// <summary>
        /// Checks if any entity matches the specified predicate.
        /// </summary>
        /// <typeparam name="T">The type of entity to check.</typeparam>
        /// <param name="predicate">The filter expression.</param>
        /// <returns>True if any matching entity exists.</returns>
        Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// Counts entities that match the specified predicate.
        /// </summary>
        /// <typeparam name="T">The type of entity to count.</typeparam>
        /// <param name="predicate">The filter expression, or null to count all.</param>
        /// <returns>The count of matching entities.</returns>
        Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class;

        /// <summary>
        /// Creates a new DbContext instance for the specified connection string.
        /// </summary>
        /// <param name="connectionString">The MySQL connection string.</param>
        /// <returns>A new DbContext instance.</returns>
        DbContext CreateDbContext(string connectionString);

        /// <summary>
        /// Tests the connection to the database.
        /// </summary>
        /// <returns>True if the connection is successful, false otherwise.</returns>
        bool TestConnection();

        /// <summary>
        /// Gets a list of all table names in the database.
        /// </summary>
        /// <returns>A list of table names if successful, null otherwise.</returns>
        List<string> GetTableNames();

        // ---- Standard read queries (additive) ------------------------------

        /// <summary>
        /// Gets the first entity matching the predicate, or null if none matches.
        /// </summary>
        /// <typeparam name="T">The type of entity to retrieve.</typeparam>
        /// <param name="predicate">The filter expression.</param>
        /// <returns>The first matching entity, or null.</returns>
        Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// Gets a page of entities: filter (optional), order by a key, then skip/take.
        /// </summary>
        /// <typeparam name="T">The type of entities to retrieve.</typeparam>
        /// <typeparam name="TKey">The ordering key type.</typeparam>
        /// <param name="predicate">The filter expression, or null for no filter.</param>
        /// <param name="orderBy">The key selector to order by.</param>
        /// <param name="descending">True to order descending; false for ascending.</param>
        /// <param name="skip">The number of entities to skip (offset).</param>
        /// <param name="take">The maximum number of entities to return.</param>
        /// <returns>The requested page of entities.</returns>
        Task<List<T>> GetPagedAsync<T, TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderBy, bool descending, int skip, int take) where T : class;

        /// <summary>
        /// Gets entities (optionally filtered) ordered by a key.
        /// </summary>
        /// <typeparam name="T">The type of entities to retrieve.</typeparam>
        /// <typeparam name="TKey">The ordering key type.</typeparam>
        /// <param name="orderBy">The key selector to order by.</param>
        /// <param name="descending">True to order descending; false for ascending.</param>
        /// <param name="predicate">The filter expression, or null for no filter.</param>
        /// <returns>The ordered entities.</returns>
        Task<List<T>> GetOrderedAsync<T, TKey>(Expression<Func<T, TKey>> orderBy, bool descending, Expression<Func<T, bool>> predicate = null) where T : class;

        /// <summary>
        /// Projects matching entities to a smaller shape, selecting only what you need.
        /// </summary>
        /// <typeparam name="T">The source entity type.</typeparam>
        /// <typeparam name="TResult">The projected result type.</typeparam>
        /// <param name="predicate">The filter expression, or null for no filter.</param>
        /// <param name="selector">The projection selector.</param>
        /// <returns>The projected results.</returns>
        Task<List<TResult>> SelectAsync<T, TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector) where T : class;

        /// <summary>
        /// Returns true if any entity exists (optionally matching the predicate).
        /// </summary>
        /// <typeparam name="T">The type of entity to check.</typeparam>
        /// <param name="predicate">The filter expression, or null to test for any row.</param>
        /// <returns>True if any matching entity exists.</returns>
        Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class;

        // ---- Raw SQL and transactions (additive) ---------------------------

        /// <summary>
        /// Executes a raw, parameterized non-query SQL statement (INSERT/UPDATE/DELETE/DDL). Use
        /// parameter placeholders (e.g. <c>{0}</c>) with the supplied <paramref name="parameters"/> so the
        /// values are sent as SQL parameters, never string-concatenated.
        /// </summary>
        /// <param name="sql">The parameterized SQL statement.</param>
        /// <param name="parameters">The parameter values referenced by the placeholders.</param>
        /// <returns>The number of rows affected.</returns>
        Task<int> ExecuteRawSqlAsync(string sql, params object[] parameters);

        /// <summary>
        /// Runs the supplied work inside a single database transaction, committing on success and rolling
        /// back if it throws.
        /// </summary>
        /// <param name="action">The work to run within the transaction.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ExecuteInTransactionAsync(Func<Task> action);
    }
}
