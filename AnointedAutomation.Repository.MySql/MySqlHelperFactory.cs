// Copyright 2024 Anointed Automation, LLC. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
// Stewarded by Alexander Fields

using System.Collections.Concurrent;

namespace AnointedAutomation.Repository.MySql
{
    /// <summary>
    /// Interface for factory creating MySQL helper instances.
    /// </summary>
    public interface IMySqlHelperFactory
    {
        /// <summary>
        /// Creates or retrieves a cached MySQL helper instance for the specified connection string.
        /// </summary>
        /// <param name="connectionString">The MySQL connection string.</param>
        /// <returns>A MySQL helper instance.</returns>
        IMySqlHelper Create(string connectionString);

        /// <summary>
        /// Removes a cached MySQL helper instance.
        /// </summary>
        /// <param name="connectionString">The MySQL connection string used as cache key.</param>
        /// <returns>True if the instance was removed, false if not found.</returns>
        bool Remove(string connectionString);

        /// <summary>
        /// Clears all cached MySQL helper instances.
        /// </summary>
        void ClearCache();
    }

    /// <summary>
    /// Factory class for creating and caching MySQL helper instances.
    /// Ensures only one helper instance exists per database connection string.
    /// Uses ConcurrentDictionary for thread-safe caching.
    /// </summary>
    public class MySqlHelperFactory : IMySqlHelperFactory
    {
        /// <summary>
        /// Cache storing MySQL helper instances keyed by connection string.
        /// </summary>
        private readonly ConcurrentDictionary<string, IMySqlHelper> _cache = new ConcurrentDictionary<string, IMySqlHelper>();

        /// <summary>
        /// Creates or retrieves a cached IMySqlHelper instance for the specified connection string.
        /// </summary>
        /// <param name="connectionString">The MySQL connection string.</param>
        /// <returns>A new or cached MySQL helper instance.</returns>
        public IMySqlHelper Create(string connectionString)
        {
            // Check if the MySqlHelper is already cached for the given connection string
            if (!_cache.TryGetValue(connectionString, out IMySqlHelper cachedMySqlHelper))
            {
                // If not found in cache, create a new instance
                MySqlHelper mySqlHelper = new MySqlHelper(connectionString);

                // Cache the newly created MySqlHelper
                _cache.TryAdd(connectionString, mySqlHelper);

                return mySqlHelper;
            }

            // If cached instance exists, return it
            return cachedMySqlHelper;
        }

        /// <summary>
        /// Removes a cached MySQL helper instance.
        /// </summary>
        /// <param name="connectionString">The MySQL connection string used as cache key.</param>
        /// <returns>True if the instance was removed, false if not found.</returns>
        public bool Remove(string connectionString)
        {
            return _cache.TryRemove(connectionString, out _);
        }

        /// <summary>
        /// Clears all cached MySQL helper instances.
        /// </summary>
        public void ClearCache()
        {
            _cache.Clear();
        }
    }
}
