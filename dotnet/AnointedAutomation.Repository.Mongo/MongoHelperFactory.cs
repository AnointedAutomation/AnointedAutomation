// Copyright © Anointed Automation Corporation., 2024. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-10-12 10:20:40 Jesus is King ✝️
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

        private readonly MongoDurabilityOptions _durability;
        private readonly MongoReadOptions _readOptions;

        /// <summary>Creates a factory whose helpers use no durability or read overrides (the driver defaults).</summary>
        public MongoHelperFactory()
        {
        }

        /// <summary>Creates a factory whose helpers all use <paramref name="durability"/>.</summary>
        /// <param name="durability">The durability options for every helper this factory creates.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="durability"/> is null.</exception>
        public MongoHelperFactory(MongoDurabilityOptions durability)
        {
            if (durability == null)
            {
                throw new System.ArgumentNullException(nameof(durability));
            }
            _durability = durability;
        }

        /// <summary>Creates a factory whose helpers all use <paramref name="durability"/> and <paramref name="readOptions"/>.</summary>
        /// <param name="durability">The durability options.</param>
        /// <param name="readOptions">The read options.</param>
        /// <exception cref="System.ArgumentNullException">An argument is null.</exception>
        public MongoHelperFactory(MongoDurabilityOptions durability, MongoReadOptions readOptions)
            : this(durability)
        {
            if (readOptions == null)
            {
                throw new System.ArgumentNullException(nameof(readOptions));
            }
            _readOptions = readOptions;
        }

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
                MongoHelper mongoHelper = NewHelper();

                mongoHelper.database = mongoHelper.CreateMongoDbInstance(dbName, connectionString);
                mongoHelper.dbName = dbName;

                // Cache the newly created MongoHelper
                _cache.TryAdd(cacheKey, mongoHelper);

                return mongoHelper;
            }

            // If cached instance exists, return it
            return cachedMongoHelper;
        }

        private MongoHelper NewHelper()
        {
            if (_durability == null)
            {
                return new MongoHelper();
            }
            if (_readOptions == null)
            {
                return new MongoHelper(_durability);
            }
            return new MongoHelper(_durability, _readOptions);
        }
    }
}
