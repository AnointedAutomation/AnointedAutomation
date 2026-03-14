// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Created by Alexander Fields https://www.alexanderfields.me on 2024-10-12 10:20:40
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
using AnointedAutomation.Optimization.Logging;

//Created by Alexander Fields
using MongoDB.Driver;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace AnointedAutomation.Repository.Mongo
{
    /// <summary>
    /// Make sure you set the MongoDB Instance before calling the classes in this
    /// </summary>
    /// <summary>
    /// Helper class for MongoDB operations. Set the MongoDB Instance before using this class.
    /// </summary>
    public class MongoHelper : IMongoHelper
    {
        private static readonly ConcurrentQueue<LogMessage> mongoLogs = new ConcurrentQueue<LogMessage>();

        /// <summary>
        /// default Constrcutor
        /// </summary>
        public MongoHelper()
        { }

        /// <summary>
        /// Constructor that will get Database
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="connectionString"></param>
        public MongoHelper(string dbName, string connectionString)
        {
            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase db = client.GetDatabase(dbName);
            database = db;
        }

        /// <summary>
        /// Finalizer for the MongoHelper class.
        /// Cleans up any remaining logs.
        /// </summary>
        ~MongoHelper()
        {
            ClearLogs();
        }

        /// <summary>
        /// Event triggered when a log message is added.
        /// </summary>
        public static event System.EventHandler<LogMessageEventArgs> LogAdded;

        /// <summary>
        /// Event triggered when logs are cleared.
        /// </summary>
        public static event System.EventHandler LogCleared;

        /// <summary>
        /// Gets or sets the MongoDB database instance.
        /// </summary>
        public IMongoDatabase database { get; set; }

        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        public string dbName { get; set; }

        /// <summary>
        /// Clears all MongoDB operation logs and triggers the LogCleared event.
        /// </summary>
        public static void ClearLogs()
        {
            mongoLogs?.Clear();
            LogCleared?.Invoke(null, System.EventArgs.Empty);
        }

        /// <summary>
        /// Connection string builder
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="cluster"></param>
        /// <param name="region">I have no idea if it's actually the region its just an assumption but its different on like all my databases</param>
        /// <returns></returns>
        public static string ConnectionStringBuilder(
            string username,
            string password,
            string cluster,
            string region
        )
        {
            string encodedPassword = System.Net.WebUtility.UrlEncode(password);

            //string connectionString = $"mongodb+srv://{username}:{encodedPassword}@{cluster}.vc4onns.mongodb.net/?retryWrites=true&w=majority";
            string connectionString =
                $"mongodb+srv://{username}:{encodedPassword}@{cluster}.{region}.mongodb.net/?retryWrites=true&w=majority";
            return connectionString;
        }

        /// <summary>
        /// Get the id for when you don't have an object class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>object</returns>
        public static object GetIdFromObj<T>(T obj)
        {
            System.Type typeInfo = typeof(T);
            PropertyInfo idProperty = typeInfo.GetProperty("_id");

            if (idProperty != null)
            {
                return idProperty.GetValue(obj, null);
            }

            return null;
        }

        /// <summary>
        /// Get the id for when you don't have an object class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>object</returns>
        public static string GetIdFromObjAsString<T>(T obj)
        {
            System.Type typeInfo = typeof(T);
            PropertyInfo idProperty = typeInfo.GetProperty("_id");

            if (idProperty != null)
            {
                string idValue = (string)idProperty.GetValue(obj, null);
                return idValue;
            }

            return null;
        }

        /// <summary>
        /// Gets all MongoDB operation logs.
        /// </summary>
        /// <returns>A list of all log messages.</returns>
        public static IList<LogMessage> GetLogs()
        {
            return mongoLogs.ToArray();
        }

        /// <summary>
        /// Helper for connecting to database
        /// </summary>
        /// <param name="mongoHelper">instance of the mongo helper that will connect</param>
        /// <param name="dbName"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="cluster"></param>
        /// <param name="region">I have no idea if it's actually the region its just an assumption but its different on like all my databases</param>
        /// <returns>mongo helper</returns>
        public static IMongoHelper MongoHelperConnector(
            IMongoHelper mongoHelper,
            string dbName,
            string username,
            string password,
            string cluster,
            string region
        )
        {
            string connectionString = ConnectionStringBuilder(username, password, cluster, region);

            mongoHelper.database = mongoHelper.CreateMongoDbInstance(dbName, connectionString);
            try
            {
                List<string> collections = mongoHelper.TestConnection();
            }
            catch (System.Exception theseHands)
            {
                AddLog(LogMessage.Error(theseHands.Message));
            }

            mongoHelper.dbName = dbName;

            return mongoHelper;
        }

        /// <summary>
        /// Creates a new document in the specified collection.
        /// </summary>
        /// <typeparam name="T">The type of document to create.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="document">The document to create.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task CreateDocumentAsync<T>(string collectionName, T document)
        {
            await GetCollection<T>(collectionName).InsertOneAsync(document);
        }

        /// <summary>
        /// For if already constructed to get Database
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public IMongoDatabase CreateMongoDbInstance(string dbName, string connectionString)
        {
            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase db = client.GetDatabase(dbName);
            return db;
        }

        /// <summary>
        /// Deletes a document from the specified collection that matches the filter.
        /// </summary>
        /// <typeparam name="T">The type of document to delete.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match documents for deletion.</param>
        /// <returns>The result of the delete operation.</returns>
        public async Task<DeleteResult> DeleteDocumentAsync<T>(string collectionName, FilterDefinition<T> filter)
        {
            return await GetCollection<T>(collectionName).DeleteOneAsync(filter);
        }

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
        public async Task<List<T>> GetAllDocumentsAsync<T>(string collectionName)
        {
            IMongoCollection<T> collection = GetCollection<T>(collectionName);
            return await collection.Find(x => true).ToListAsync();
        }

        /// <summary>
        /// Gets all document IDs from the specified collection WITHOUT loading full documents.
        /// This is the SAFE alternative to GetAllDocumentsAsync - use this when you need to
        /// iterate over all documents but don't need the full document data immediately.
        /// Uses projection to only fetch the _id field, preventing memory issues.
        /// </summary>
        /// <param name="collectionName">The name of the collection.</param>
        /// <returns>A list of all document IDs in the collection.</returns>
        public async Task<List<string>> GetAllDocumentIdsAsync(string collectionName)
        {
            IMongoCollection<MongoDB.Bson.BsonDocument> collection = database.GetCollection<MongoDB.Bson.BsonDocument>(collectionName);
            ProjectionDefinition<MongoDB.Bson.BsonDocument> projection = Builders<MongoDB.Bson.BsonDocument>.Projection.Include("_id");
            List<MongoDB.Bson.BsonDocument> documents = await collection.Find(FilterDefinition<MongoDB.Bson.BsonDocument>.Empty)
                .Project(projection)
                .ToListAsync();
            return documents.ConvertAll(doc => doc["_id"].AsString);
        }

        /// <summary>
        /// Gets documents from the specified collection that match the filter.
        /// </summary>
        /// <typeparam name="T">The type of documents to retrieve.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match documents.</param>
        /// <returns>A list of matching documents.</returns>
        public async Task<List<T>> GetFilteredDocumentsAsync<T>(
                    string collectionName,
                    FilterDefinition<T> filter
                )
        {
            return await GetCollection<T>(collectionName).Find(filter).ToListAsync();
        }

        /// <summary>
        /// Replaces a document in the specified collection that matches the filter.
        /// </summary>
        /// <typeparam name="T">The type of document to replace.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match the document to replace.</param>
        /// <param name="document">The new document.</param>
        /// <returns>The result of the replace operation.</returns>
        public async Task<ReplaceOneResult> ReplaceDocumentAsync<T>(
                    string collectionName,
                    FilterDefinition<T> filter,
                    T document
                )
        {
            return await GetCollection<T>(collectionName).ReplaceOneAsync(filter, document);
        }

        /// <summary>
        /// Tests the connection to the database
        /// </summary>
        /// <returns>Will return a List of Collection Names if it worked otherwise returns null</returns>
        public List<string> TestConnection()
        {
            try
            {
                List<string> collectionNames = database.ListCollectionNames().ToList();
                return collectionNames;
            }
            catch (System.Exception e)
            {
                AddLog(
                    LogMessage.Error(
                        "Was unable to properly connect to the database!" + e.ToString()
                    )
                );
                return null;
            }
        }

        /// <summary>
        /// Updates a document in the specified collection that matches the filter.
        /// </summary>
        /// <typeparam name="T">The type of document to update.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="filter">The filter to match the document to update.</param>
        /// <param name="document">The update definition.</param>
        /// <returns>The result of the update operation.</returns>
        public async Task<UpdateResult> UpdateDocumentAsync<T>(
            string collectionName,
            FilterDefinition<T> filter,
            UpdateDefinition<T> document
        )
        {
            return await GetCollection<T>(collectionName).UpdateOneAsync(filter, document);
        }

        /// <summary>
        /// Adds a log message to the MongoDB operation logs.
        /// </summary>
        /// <param name="logMessage">The log message to add.</param>
        protected static void AddLog(LogMessage logMessage)
        {
            mongoLogs.Enqueue(logMessage);
            LogAdded?.Invoke(null, new LogMessageEventArgs(logMessage));
        }

        /// <summary>
        /// Gets a typed collection from the database.
        /// </summary>
        /// <typeparam name="T">The type of documents in the collection.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <returns>The MongoDB collection.</returns>
        private IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return database.GetCollection<T>(collectionName);
        }
    }
}
