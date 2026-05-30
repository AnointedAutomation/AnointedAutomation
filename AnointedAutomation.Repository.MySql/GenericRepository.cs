// Copyright 2024 Anointed Automation, LLC. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
// Stewarded by Alexander Fields

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AnointedAutomation.Repository.MySql
{
    /// <summary>
    /// Generic repository implementation for CRUD operations on any entity type.
    /// Uses Entity Framework Core for database operations.
    /// </summary>
    /// <typeparam name="T">The entity type this repository manages.</typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;

        /// <summary>
        /// Initializes a new instance of the GenericRepository class.
        /// </summary>
        /// <param name="context">The DbContext to use for database operations.</param>
        public GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Gets an entity by its primary key.
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The entity if found, otherwise null.</returns>
        public async Task<T> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Gets all entities of type T.
        /// </summary>
        /// <returns>An enumerable of all entities.</returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Finds entities that match the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression.</param>
        /// <returns>An enumerable of matching entities.</returns>
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Finds a single entity that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression.</param>
        /// <returns>The first matching entity, or null if none found.</returns>
        public async Task<T> FindSingleAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity with any database-generated values populated.</returns>
        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Adds multiple entities to the database.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        /// <returns>The number of entities added.</returns>
        public async Task<int> AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="entity">The entity with updated values.</param>
        /// <returns>The updated entity.</returns>
        public async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Deletes an entity by its primary key.
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>True if the entity was deleted, false if not found.</returns>
        public async Task<bool> DeleteAsync(object id)
        {
            T entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>True if the entity was deleted.</returns>
        public async Task<bool> DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            int result = await _context.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// Deletes multiple entities that match the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression.</param>
        /// <returns>The number of entities deleted.</returns>
        public async Task<int> DeleteRangeAsync(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> entities = await _dbSet.Where(predicate).ToListAsync();
            _dbSet.RemoveRange(entities);
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Checks if any entity matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression.</param>
        /// <returns>True if any matching entity exists.</returns>
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        /// <summary>
        /// Counts entities that match the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression, or null to count all.</param>
        /// <returns>The count of matching entities.</returns>
        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return await _dbSet.CountAsync();
            }
            return await _dbSet.CountAsync(predicate);
        }
    }
}
