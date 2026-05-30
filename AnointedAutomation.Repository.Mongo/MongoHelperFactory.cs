// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-10-12 10:20:40
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
using System.Collections.Concurrent;

//Stewarded by Alexander Fields
namespace AnointedAutomation.Repository.Mongo
{
    /// <summary>
    /// Interface for factory creating MongoDB helper instances.
    /// </summary>
    public interface IMongoHelperFactory
    {
        /// <summary>
        /// Creates a MongoDB helper instance for the specified database.
        /// </summary>
        /// <param name="dbName">The name of the database.</param>
        /// <param name="connectionString">The MongoDB connection string.</param>
        /// <returns>A MongoDB helper instance.</returns>
        IMongoHelper Create(string dbName, string connectionString);
    }

    /// <summary>
    /// Factory class for creating and caching MongoDB helper instances.
    /// Ensures only one helper instance exists per database connection.
    /// </summary>
    public class MongoHelperFactory : IMongoHelperFactory
    {
        // Cache to store MongoHelper instances with their respective database and connection strings
        /// <summary>
        /// Cache storing MongoDB helper instances keyed by database connection details.
        /// </summary>
        private readonly ConcurrentDictionary<string, IMongoHelper> _cache = new ConcurrentDictionary<string, IMongoHelper>();

        /// <summary>
        /// Creates or retrieves a cached IMongoHelper instance for the specified database.
        /// </summary>
        /// <param name="dbName">The name of the database to connect to.</param>
        /// <param name="connectionString">The MongoDB connection string.</param>
        /// <returns>A new or cached MongoDB helper instance.</returns>
        public IMongoHelper Create(string dbName, string connectionString)
        {
            // Combine dbName and connectionString as a cache key
            string cacheKey = $"{connectionString}-{dbName}";

            // Check if the MongoHelper is already cached for the given dbName and connectionString
            if (!_cache.TryGetValue(cacheKey, out IMongoHelper cachedMongoHelper))
            {
                // If not found in cache, create a new instance
                MongoHelper mongoHelper = new MongoHelper();

                mongoHelper.database = mongoHelper.CreateMongoDbInstance(dbName, connectionString);
                mongoHelper.dbName = dbName;

                // Cache the newly created MongoHelper
                _cache.TryAdd(cacheKey, mongoHelper);

                return mongoHelper;
            }

            // If cached instance exists, return it
            return cachedMongoHelper;
        }
    }
}
