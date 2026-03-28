// Copyright © Anointed Automation, LLC., 2025. All Rights Reserved. Created by Alexander Fields https://www.alexanderfields.me on 2025-06-08 13:27:40
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnointedAutomation.Optimization.Logging;
using MongoDB.Driver;
using Xunit;
using Moq;

namespace AnointedAutomation.Repository.Mongo.Tests
{
    public class MongoHelperTests
    {
        private class MongoTestDocument
        {
            public string _id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
        }

        [Fact]
        public void DefaultConstructor_CreatesInstance()
        {
            // Act
            MongoHelper mongoHelper = new MongoHelper();

            // Assert
            Assert.NotNull(mongoHelper);
            Assert.Null(mongoHelper.database);
        }

        [Fact]
        public void ParameterizedConstructor_InitializesDatabase()
        {
            // Arrange - This test requires a real MongoDB instance, so we'll skip actual validation
            string dbName = "test-db";
            string connectionString = "mongodb://localhost:27017";

            // Act
            MongoHelper mongoHelper = new MongoHelper(dbName, connectionString);

            // Assert
            Assert.NotNull(mongoHelper);
            Assert.NotNull(mongoHelper.database);
        }

        [Fact]
        public void ConnectionStringBuilder_FormatsConnectionStringCorrectly()
        {
            // Arrange
            string username = "testuser";
            string password = "testpass";
            string cluster = "testcluster";
            string region = "testregion";
            
            // Act
            string connectionString = MongoHelper.ConnectionStringBuilder(username, password, cluster, region);
            
            // Assert
            string expected = $"mongodb+srv://{username}:{Uri.EscapeDataString(password)}@{cluster}.{region}.mongodb.net/?retryWrites=true&w=majority";
            Assert.Equal(expected, connectionString);
        }

        [Fact]
        public void ConnectionStringBuilder_EncodesPasswordWithSpecialCharacters()
        {
            // Arrange
            string username = "testuser";
            string password = "test@pass&";
            string cluster = "testcluster";
            string region = "testregion";
            
            // Act
            string connectionString = MongoHelper.ConnectionStringBuilder(username, password, cluster, region);
            
            // Assert
            string encodedPassword = Uri.EscapeDataString(password);
            string expected = $"mongodb+srv://{username}:{encodedPassword}@{cluster}.{region}.mongodb.net/?retryWrites=true&w=majority";
            Assert.Equal(expected, connectionString);
        }

        [Fact]
        public void GetIdFromObj_ReturnsIdProperty()
        {
            // Arrange
            MongoTestDocument testDoc = new MongoTestDocument { _id = "test-id", Name = "Test", Age = 30 };

            // Act
            object id = MongoHelper.GetIdFromObj(testDoc);

            // Assert
            Assert.Equal("test-id", id);
        }

        [Fact]
        public void GetIdFromObj_ReturnsNull_WhenNoIdProperty()
        {
            // Arrange
            object objWithNoId = new { Name = "Test", Age = 30 };

            // Act
            object id = MongoHelper.GetIdFromObj(objWithNoId);

            // Assert
            Assert.Null(id);
        }

        [Fact]
        public void GetIdFromObjAsString_ReturnsIdAsString()
        {
            // Arrange
            MongoTestDocument testDoc = new MongoTestDocument { _id = "test-id", Name = "Test", Age = 30 };

            // Act
            string id = MongoHelper.GetIdFromObjAsString(testDoc);

            // Assert
            Assert.Equal("test-id", id);
        }

        [Fact]
        public void GetIdFromObjAsString_ReturnsNull_WhenNoIdProperty()
        {
            // Arrange
            object objWithNoId = new { Name = "Test", Age = 30 };

            // Act
            string id = MongoHelper.GetIdFromObjAsString(objWithNoId);

            // Assert
            Assert.Null(id);
        }

        [Fact]
        public void AddLog_AddsLogToQueue_AndTriggersEvent()
        {
            // Arrange
            MongoHelper.ClearLogs();
            bool eventRaised = false;

            EventHandler<LogMessageEventArgs> handler = (sender, e) => eventRaised = true;
            MongoHelper.LogAdded += handler;

            // Use reflection to call the protected method
            MongoHelper mongoHelper = new MongoHelper();
            System.Reflection.MethodInfo methodInfo = typeof(MongoHelper).GetMethod("AddLog", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            object[] parameters = new object[] { LogMessage.Informational("Test log message") };

            // Act
            methodInfo.Invoke(null, parameters);

            // Assert
            IList<LogMessage> logs = MongoHelper.GetLogs();
            Assert.Single(logs);
            Assert.True(eventRaised, "LogAdded event should be triggered");

            // Cleanup
            MongoHelper.LogAdded -= handler;
        }

        [Fact]
        public void ClearLogs_EmptiesLogQueue_AndTriggersEvent()
        {
            // Arrange
            // Add some logs (using reflection to call the protected method)
            System.Reflection.MethodInfo methodInfo = typeof(MongoHelper).GetMethod("AddLog", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            methodInfo.Invoke(null, new object[] { LogMessage.Informational("Test log message 1") });
            methodInfo.Invoke(null, new object[] { LogMessage.Informational("Test log message 2") });

            bool eventRaised = false;
            EventHandler handler = (sender, e) => eventRaised = true;

            MongoHelper.LogCleared += handler;

            // Act
            MongoHelper.ClearLogs();

            // Assert
            IList<LogMessage> logs = MongoHelper.GetLogs();
            Assert.Empty(logs);
            Assert.True(eventRaised, "LogCleared event should be triggered");

            // Cleanup
            MongoHelper.LogCleared -= handler;
        }

        [Fact]
        public void GetLogs_ReturnsAllLogs()
        {
            // Arrange
            MongoHelper.ClearLogs();

            // Add some logs (using reflection to call the protected method)
            System.Reflection.MethodInfo methodInfo = typeof(MongoHelper).GetMethod("AddLog", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            LogMessage log1 = LogMessage.Informational("Test log message 1");
            LogMessage log2 = LogMessage.Informational("Test log message 2");
            LogMessage log3 = LogMessage.Informational("Test log message 3");

            methodInfo.Invoke(null, new object[] { log1 });
            methodInfo.Invoke(null, new object[] { log2 });
            methodInfo.Invoke(null, new object[] { log3 });

            // Act
            IList<LogMessage> logs = MongoHelper.GetLogs();

            // Assert
            Assert.Equal(3, logs.Count);
        }

        [Fact]
        public void CreateMongoDbInstance_ReturnsValidDatabase()
        {
            // Arrange
            MongoHelper mongoHelper = new MongoHelper();
            string dbName = "test-db";
            string connectionString = "mongodb://localhost:27017";

            // Act
            IMongoDatabase database = mongoHelper.CreateMongoDbInstance(dbName, connectionString);

            // Assert
            Assert.NotNull(database);
        }

        [Fact]
        public void TestConnection_ReturnsNull_WhenDatabaseNotSet()
        {
            // Arrange
            MongoHelper mongoHelper = new MongoHelper();
            // database is null

            // Act & Assert - Should return null and log error
            MongoHelper.ClearLogs();
            List<string> result = mongoHelper.TestConnection();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void TestConnection_WithLocalhost_ShouldNotThrowException()
        {
            // Arrange
            MongoHelper mongoHelper = new MongoHelper("test-db", "mongodb://localhost:27017");

            // Act - Verify no unhandled exception is thrown
            // The method handles its own exceptions internally and returns null on failure
            Exception ex = Record.Exception(() => mongoHelper.TestConnection());

            // Assert - Method should not throw (it handles exceptions internally)
            Assert.Null(ex);
        }

        [Fact]
        public void MongoHelperConnector_SetsUpMongoHelper_WithValidCredentials()
        {
            // Arrange
            IMongoHelper mongoHelper = new MongoHelper();
            string dbName = "test-db";
            string username = "testuser";
            string password = "testpass";
            string cluster = "testcluster";
            string region = "testregion";

            MongoHelper.ClearLogs();

            // Act
            IMongoHelper result = MongoHelper.MongoHelperConnector(
                mongoHelper, dbName, username, password, cluster, region);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.database);
            Assert.Equal(dbName, result.dbName);
        }

        [Fact]
        public void MongoHelperConnector_LogsError_WhenConnectionFails()
        {
            // Arrange
            Mock<IMongoHelper> mockHelper = new Mock<IMongoHelper>();

            // Setup CreateMongoDbInstance to return a mock database
            Mock<IMongoDatabase> mockDatabase = new Mock<IMongoDatabase>();
            mockHelper.Setup(x => x.CreateMongoDbInstance(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(mockDatabase.Object);
            mockHelper.SetupProperty(x => x.database);
            mockHelper.SetupProperty(x => x.dbName);

            // Setup TestConnection to throw exception
            mockHelper.Setup(x => x.TestConnection())
                .Throws(new Exception("Connection failed"));

            MongoHelper.ClearLogs();

            // Act
            IMongoHelper result = MongoHelper.MongoHelperConnector(
                mockHelper.Object, "test-db", "user", "pass", "cluster", "region");

            // Assert
            Assert.NotNull(result);
            IList<LogMessage> logs = MongoHelper.GetLogs();
            Assert.True(logs.Count > 0, "Error should be logged when connection fails");
        }

        [Fact]
        public void ConnectionStringBuilder_HandlesEmptyPassword()
        {
            // Arrange
            string username = "testuser";
            string password = "";
            string cluster = "testcluster";
            string region = "testregion";

            // Act
            string connectionString = MongoHelper.ConnectionStringBuilder(username, password, cluster, region);

            // Assert
            string expected = $"mongodb+srv://{username}:@{cluster}.{region}.mongodb.net/?retryWrites=true&w=majority";
            Assert.Equal(expected, connectionString);
        }

        [Fact]
        public void GetIdFromObj_WithNullObject_ThrowsTargetException()
        {
            // Arrange
            MongoTestDocument testDoc = null;

            // Act & Assert
            // When the object is null but the type has an _id property,
            // GetValue throws TargetException because you can't get a property from null
            Exception ex = Record.Exception(() => MongoHelper.GetIdFromObj(testDoc));

            Assert.NotNull(ex);
            Assert.IsType<System.Reflection.TargetException>(ex);
        }

        [Fact]
        public void GetIdFromObjAsString_WithNullObject_ThrowsTargetException()
        {
            // Arrange
            MongoTestDocument testDoc = null;

            // Act & Assert
            // When the object is null but the type has an _id property,
            // GetValue throws TargetException because you can't get a property from null
            Exception ex = Record.Exception(() => MongoHelper.GetIdFromObjAsString(testDoc));

            Assert.NotNull(ex);
            Assert.IsType<System.Reflection.TargetException>(ex);
        }

        [Fact]
        public void Database_Property_CanBeSetAndRetrieved()
        {
            // Arrange
            MongoHelper mongoHelper = new MongoHelper();
            Mock<IMongoDatabase> mockDatabase = new Mock<IMongoDatabase>();

            // Act
            mongoHelper.database = mockDatabase.Object;

            // Assert
            Assert.Equal(mockDatabase.Object, mongoHelper.database);
        }

        [Fact]
        public void DbName_Property_CanBeSetAndRetrieved()
        {
            // Arrange
            MongoHelper mongoHelper = new MongoHelper();
            string expectedDbName = "test-database-name";

            // Act
            mongoHelper.dbName = expectedDbName;

            // Assert
            Assert.Equal(expectedDbName, mongoHelper.dbName);
        }
    }

    /// <summary>
    /// Public test document class for MongoHelper async tests.
    /// Must be public for Moq to create proxies for generic collections.
    /// </summary>
    public class MongoTestDoc
    {
        public string _id { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// Tests for MongoHelper async CRUD operations using mocked MongoDB.
    /// </summary>
    public class MongoHelperAsyncCrudTests
    {

        [Fact]
        public async Task CreateDocumentAsync_InsertsDocument()
        {
            // Arrange
            Mock<IMongoCollection<MongoTestDoc>> mockCollection = new Mock<IMongoCollection<MongoTestDoc>>();
            mockCollection.Setup(c => c.InsertOneAsync(
                It.IsAny<MongoTestDoc>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<System.Threading.CancellationToken>()))
                .Returns(Task.CompletedTask);

            Mock<IMongoDatabase> mockDb = new Mock<IMongoDatabase>();
            mockDb.Setup(d => d.GetCollection<MongoTestDoc>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mockCollection.Object);

            MongoHelper helper = new MongoHelper();
            helper.database = mockDb.Object;

            MongoTestDoc doc = new MongoTestDoc { _id = "test-id", Name = "Test" };

            // Act
            await helper.CreateDocumentAsync("testCollection", doc);

            // Assert
            mockCollection.Verify(c => c.InsertOneAsync(
                doc,
                It.IsAny<InsertOneOptions>(),
                It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteDocumentAsync_DeletesDocument()
        {
            // Arrange
            DeleteResult deleteResult = new DeleteResult.Acknowledged(1);
            Mock<IMongoCollection<MongoTestDoc>> mockCollection = new Mock<IMongoCollection<MongoTestDoc>>();
            mockCollection.Setup(c => c.DeleteOneAsync(
                It.IsAny<FilterDefinition<MongoTestDoc>>(),
                It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(deleteResult);

            Mock<IMongoDatabase> mockDb = new Mock<IMongoDatabase>();
            mockDb.Setup(d => d.GetCollection<MongoTestDoc>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mockCollection.Object);

            MongoHelper helper = new MongoHelper();
            helper.database = mockDb.Object;

            FilterDefinition<MongoTestDoc> filter = Builders<MongoTestDoc>.Filter.Eq(d => d._id, "test-id");

            // Act
            DeleteResult result = await helper.DeleteDocumentAsync("testCollection", filter);

            // Assert
            Assert.Equal(1, result.DeletedCount);
        }

        [Fact]
        public async Task GetAllDocumentsAsync_ReturnsAllDocuments()
        {
            // Arrange
            List<MongoTestDoc> docs = new List<MongoTestDoc>
            {
                new MongoTestDoc { _id = "1", Name = "Doc1" },
                new MongoTestDoc { _id = "2", Name = "Doc2" }
            };

            Mock<IAsyncCursor<MongoTestDoc>> mockCursor = new Mock<IAsyncCursor<MongoTestDoc>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            mockCursor.Setup(c => c.Current).Returns(docs);

            Mock<IFindFluent<MongoTestDoc, MongoTestDoc>> mockFindFluent = new Mock<IFindFluent<MongoTestDoc, MongoTestDoc>>();
            mockFindFluent.Setup(f => f.ToCursorAsync(It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            Mock<IMongoCollection<MongoTestDoc>> mockCollection = new Mock<IMongoCollection<MongoTestDoc>>();
            mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<MongoTestDoc>>(),
                It.IsAny<FindOptions<MongoTestDoc, MongoTestDoc>>(),
                It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            Mock<IMongoDatabase> mockDb = new Mock<IMongoDatabase>();
            mockDb.Setup(d => d.GetCollection<MongoTestDoc>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mockCollection.Object);

            MongoHelper helper = new MongoHelper();
            helper.database = mockDb.Object;

            // Act
            List<MongoTestDoc> result = await helper.GetAllDocumentsAsync<MongoTestDoc>("testCollection");

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetFilteredDocumentsAsync_ReturnsFilteredDocuments()
        {
            // Arrange
            List<MongoTestDoc> docs = new List<MongoTestDoc>
            {
                new MongoTestDoc { _id = "1", Name = "Alpha" }
            };

            Mock<IAsyncCursor<MongoTestDoc>> mockCursor = new Mock<IAsyncCursor<MongoTestDoc>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            mockCursor.Setup(c => c.Current).Returns(docs);

            Mock<IMongoCollection<MongoTestDoc>> mockCollection = new Mock<IMongoCollection<MongoTestDoc>>();
            mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<MongoTestDoc>>(),
                It.IsAny<FindOptions<MongoTestDoc, MongoTestDoc>>(),
                It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            Mock<IMongoDatabase> mockDb = new Mock<IMongoDatabase>();
            mockDb.Setup(d => d.GetCollection<MongoTestDoc>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mockCollection.Object);

            MongoHelper helper = new MongoHelper();
            helper.database = mockDb.Object;

            FilterDefinition<MongoTestDoc> filter = Builders<MongoTestDoc>.Filter.Eq(d => d.Name, "Alpha");

            // Act
            List<MongoTestDoc> result = await helper.GetFilteredDocumentsAsync("testCollection", filter);

            // Assert
            Assert.Single(result);
            Assert.Equal("Alpha", result[0].Name);
        }

        [Fact]
        public async Task ReplaceDocumentAsync_ReplacesDocument()
        {
            // Arrange
            ReplaceOneResult replaceResult = new ReplaceOneResult.Acknowledged(1, 1, null);
            Mock<IMongoCollection<MongoTestDoc>> mockCollection = new Mock<IMongoCollection<MongoTestDoc>>();
            mockCollection.Setup(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<MongoTestDoc>>(),
                It.IsAny<MongoTestDoc>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(replaceResult);

            Mock<IMongoDatabase> mockDb = new Mock<IMongoDatabase>();
            mockDb.Setup(d => d.GetCollection<MongoTestDoc>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mockCollection.Object);

            MongoHelper helper = new MongoHelper();
            helper.database = mockDb.Object;

            MongoTestDoc doc = new MongoTestDoc { _id = "test-id", Name = "Updated" };
            FilterDefinition<MongoTestDoc> filter = Builders<MongoTestDoc>.Filter.Eq(d => d._id, "test-id");

            // Act
            ReplaceOneResult result = await helper.ReplaceDocumentAsync("testCollection", filter, doc);

            // Assert
            Assert.Equal(1, result.ModifiedCount);
        }

        [Fact]
        public async Task UpdateDocumentAsync_UpdatesDocument()
        {
            // Arrange
            UpdateResult updateResult = new UpdateResult.Acknowledged(1, 1, null);
            Mock<IMongoCollection<MongoTestDoc>> mockCollection = new Mock<IMongoCollection<MongoTestDoc>>();
            mockCollection.Setup(c => c.UpdateOneAsync(
                It.IsAny<FilterDefinition<MongoTestDoc>>(),
                It.IsAny<UpdateDefinition<MongoTestDoc>>(),
                It.IsAny<UpdateOptions>(),
                It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(updateResult);

            Mock<IMongoDatabase> mockDb = new Mock<IMongoDatabase>();
            mockDb.Setup(d => d.GetCollection<MongoTestDoc>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mockCollection.Object);

            MongoHelper helper = new MongoHelper();
            helper.database = mockDb.Object;

            FilterDefinition<MongoTestDoc> filter = Builders<MongoTestDoc>.Filter.Eq(d => d._id, "test-id");
            UpdateDefinition<MongoTestDoc> update = Builders<MongoTestDoc>.Update.Set(d => d.Name, "NewName");

            // Act
            UpdateResult result = await helper.UpdateDocumentAsync("testCollection", filter, update);

            // Assert
            Assert.Equal(1, result.ModifiedCount);
        }

        [Fact]
        public async Task GetAllDocumentIdsAsync_ReturnsAllIds()
        {
            // Arrange
            List<MongoDB.Bson.BsonDocument> docs = new List<MongoDB.Bson.BsonDocument>
            {
                new MongoDB.Bson.BsonDocument { { "_id", "id1" } },
                new MongoDB.Bson.BsonDocument { { "_id", "id2" } },
                new MongoDB.Bson.BsonDocument { { "_id", "id3" } }
            };

            Mock<IAsyncCursor<MongoDB.Bson.BsonDocument>> mockCursor = new Mock<IAsyncCursor<MongoDB.Bson.BsonDocument>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            mockCursor.Setup(c => c.Current).Returns(docs);

            Mock<IFindFluent<MongoDB.Bson.BsonDocument, MongoDB.Bson.BsonDocument>> mockFindFluent =
                new Mock<IFindFluent<MongoDB.Bson.BsonDocument, MongoDB.Bson.BsonDocument>>();
            mockFindFluent.Setup(f => f.Project(It.IsAny<ProjectionDefinition<MongoDB.Bson.BsonDocument, MongoDB.Bson.BsonDocument>>()))
                .Returns(mockFindFluent.Object);
            mockFindFluent.Setup(f => f.ToCursorAsync(It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            Mock<IMongoCollection<MongoDB.Bson.BsonDocument>> mockCollection = new Mock<IMongoCollection<MongoDB.Bson.BsonDocument>>();
            mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<MongoDB.Bson.BsonDocument>>(),
                It.IsAny<FindOptions<MongoDB.Bson.BsonDocument, MongoDB.Bson.BsonDocument>>(),
                It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            Mock<IMongoDatabase> mockDb = new Mock<IMongoDatabase>();
            mockDb.Setup(d => d.GetCollection<MongoDB.Bson.BsonDocument>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mockCollection.Object);

            MongoHelper helper = new MongoHelper();
            helper.database = mockDb.Object;

            // Act
            List<string> result = await helper.GetAllDocumentIdsAsync("testCollection");

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Contains("id1", result);
            Assert.Contains("id2", result);
            Assert.Contains("id3", result);
        }

        [Fact]
        public void TestConnection_ReturnsCollectionNames_WhenSuccessful()
        {
            // Arrange
            List<string> collectionNames = new List<string> { "collection1", "collection2" };

            Mock<IAsyncCursor<string>> mockCursor = new Mock<IAsyncCursor<string>>();
            mockCursor.SetupSequence(c => c.MoveNext(It.IsAny<System.Threading.CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCursor.Setup(c => c.Current).Returns(collectionNames);

            Mock<IMongoDatabase> mockDb = new Mock<IMongoDatabase>();
            mockDb.Setup(d => d.ListCollectionNames(It.IsAny<ListCollectionNamesOptions>(), It.IsAny<System.Threading.CancellationToken>()))
                .Returns(mockCursor.Object);

            MongoHelper helper = new MongoHelper();
            helper.database = mockDb.Object;

            // Act
            List<string> result = helper.TestConnection();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        #region Failure Scenarios

        [Fact]
        public async Task CreateDocumentAsync_ThrowsException_WhenDatabaseFails()
        {
            // Arrange
            Mock<IMongoCollection<MongoTestDoc>> mockCollection = new Mock<IMongoCollection<MongoTestDoc>>();
            mockCollection.Setup(c => c.InsertOneAsync(
                It.IsAny<MongoTestDoc>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<System.Threading.CancellationToken>()))
                .ThrowsAsync(new MongoException("Database error"));

            Mock<IMongoDatabase> mockDb = new Mock<IMongoDatabase>();
            mockDb.Setup(d => d.GetCollection<MongoTestDoc>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mockCollection.Object);

            MongoHelper helper = new MongoHelper();
            helper.database = mockDb.Object;

            MongoTestDoc doc = new MongoTestDoc { _id = "test-id", Name = "Test" };

            // Act & Assert
            await Assert.ThrowsAsync<MongoException>(() =>
                helper.CreateDocumentAsync("testCollection", doc));
        }

        [Fact]
        public async Task DeleteDocumentAsync_ReturnsZeroDeleted_WhenDocumentNotFound()
        {
            // Arrange
            DeleteResult deleteResult = new DeleteResult.Acknowledged(0);
            Mock<IMongoCollection<MongoTestDoc>> mockCollection = new Mock<IMongoCollection<MongoTestDoc>>();
            mockCollection.Setup(c => c.DeleteOneAsync(
                It.IsAny<FilterDefinition<MongoTestDoc>>(),
                It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(deleteResult);

            Mock<IMongoDatabase> mockDb = new Mock<IMongoDatabase>();
            mockDb.Setup(d => d.GetCollection<MongoTestDoc>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mockCollection.Object);

            MongoHelper helper = new MongoHelper();
            helper.database = mockDb.Object;

            FilterDefinition<MongoTestDoc> filter = Builders<MongoTestDoc>.Filter.Eq(d => d._id, "nonexistent");

            // Act
            DeleteResult result = await helper.DeleteDocumentAsync("testCollection", filter);

            // Assert
            Assert.Equal(0, result.DeletedCount);
        }

        [Fact]
        public async Task UpdateDocumentAsync_ReturnsZeroModified_WhenDocumentNotFound()
        {
            // Arrange
            UpdateResult updateResult = new UpdateResult.Acknowledged(0, 0, null);
            Mock<IMongoCollection<MongoTestDoc>> mockCollection = new Mock<IMongoCollection<MongoTestDoc>>();
            mockCollection.Setup(c => c.UpdateOneAsync(
                It.IsAny<FilterDefinition<MongoTestDoc>>(),
                It.IsAny<UpdateDefinition<MongoTestDoc>>(),
                It.IsAny<UpdateOptions>(),
                It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(updateResult);

            Mock<IMongoDatabase> mockDb = new Mock<IMongoDatabase>();
            mockDb.Setup(d => d.GetCollection<MongoTestDoc>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mockCollection.Object);

            MongoHelper helper = new MongoHelper();
            helper.database = mockDb.Object;

            FilterDefinition<MongoTestDoc> filter = Builders<MongoTestDoc>.Filter.Eq(d => d._id, "nonexistent");
            UpdateDefinition<MongoTestDoc> update = Builders<MongoTestDoc>.Update.Set(d => d.Name, "NewName");

            // Act
            UpdateResult result = await helper.UpdateDocumentAsync("testCollection", filter, update);

            // Assert
            Assert.Equal(0, result.ModifiedCount);
        }

        [Fact]
        public async Task ReplaceDocumentAsync_ReturnsZeroModified_WhenDocumentNotFound()
        {
            // Arrange
            ReplaceOneResult replaceResult = new ReplaceOneResult.Acknowledged(0, 0, null);
            Mock<IMongoCollection<MongoTestDoc>> mockCollection = new Mock<IMongoCollection<MongoTestDoc>>();
            mockCollection.Setup(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<MongoTestDoc>>(),
                It.IsAny<MongoTestDoc>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(replaceResult);

            Mock<IMongoDatabase> mockDb = new Mock<IMongoDatabase>();
            mockDb.Setup(d => d.GetCollection<MongoTestDoc>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mockCollection.Object);

            MongoHelper helper = new MongoHelper();
            helper.database = mockDb.Object;

            MongoTestDoc doc = new MongoTestDoc { _id = "nonexistent", Name = "Updated" };
            FilterDefinition<MongoTestDoc> filter = Builders<MongoTestDoc>.Filter.Eq(d => d._id, "nonexistent");

            // Act
            ReplaceOneResult result = await helper.ReplaceDocumentAsync("testCollection", filter, doc);

            // Assert
            Assert.Equal(0, result.ModifiedCount);
        }

        #endregion

        #region Edge Case Scenarios

        [Fact]
        public async Task GetAllDocumentsAsync_ReturnsEmptyList_WhenCollectionIsEmpty()
        {
            // Arrange
            List<MongoTestDoc> emptyDocs = new List<MongoTestDoc>();

            Mock<IAsyncCursor<MongoTestDoc>> mockCursor = new Mock<IAsyncCursor<MongoTestDoc>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            mockCursor.Setup(c => c.Current).Returns(emptyDocs);

            Mock<IMongoCollection<MongoTestDoc>> mockCollection = new Mock<IMongoCollection<MongoTestDoc>>();
            mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<MongoTestDoc>>(),
                It.IsAny<FindOptions<MongoTestDoc, MongoTestDoc>>(),
                It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            Mock<IMongoDatabase> mockDb = new Mock<IMongoDatabase>();
            mockDb.Setup(d => d.GetCollection<MongoTestDoc>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mockCollection.Object);

            MongoHelper helper = new MongoHelper();
            helper.database = mockDb.Object;

            // Act
            List<MongoTestDoc> result = await helper.GetAllDocumentsAsync<MongoTestDoc>("testCollection");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetFilteredDocumentsAsync_ReturnsEmptyList_WhenNoMatches()
        {
            // Arrange
            List<MongoTestDoc> emptyDocs = new List<MongoTestDoc>();

            Mock<IAsyncCursor<MongoTestDoc>> mockCursor = new Mock<IAsyncCursor<MongoTestDoc>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            mockCursor.Setup(c => c.Current).Returns(emptyDocs);

            Mock<IMongoCollection<MongoTestDoc>> mockCollection = new Mock<IMongoCollection<MongoTestDoc>>();
            mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<MongoTestDoc>>(),
                It.IsAny<FindOptions<MongoTestDoc, MongoTestDoc>>(),
                It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            Mock<IMongoDatabase> mockDb = new Mock<IMongoDatabase>();
            mockDb.Setup(d => d.GetCollection<MongoTestDoc>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mockCollection.Object);

            MongoHelper helper = new MongoHelper();
            helper.database = mockDb.Object;

            FilterDefinition<MongoTestDoc> filter = Builders<MongoTestDoc>.Filter.Eq(d => d.Name, "NonExistent");

            // Act
            List<MongoTestDoc> result = await helper.GetFilteredDocumentsAsync("testCollection", filter);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllDocumentIdsAsync_ReturnsEmptyList_WhenCollectionIsEmpty()
        {
            // Arrange
            List<MongoDB.Bson.BsonDocument> emptyDocs = new List<MongoDB.Bson.BsonDocument>();

            Mock<IAsyncCursor<MongoDB.Bson.BsonDocument>> mockCursor = new Mock<IAsyncCursor<MongoDB.Bson.BsonDocument>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            mockCursor.Setup(c => c.Current).Returns(emptyDocs);

            Mock<IMongoCollection<MongoDB.Bson.BsonDocument>> mockCollection = new Mock<IMongoCollection<MongoDB.Bson.BsonDocument>>();
            mockCollection.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<MongoDB.Bson.BsonDocument>>(),
                It.IsAny<FindOptions<MongoDB.Bson.BsonDocument, MongoDB.Bson.BsonDocument>>(),
                It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            Mock<IMongoDatabase> mockDb = new Mock<IMongoDatabase>();
            mockDb.Setup(d => d.GetCollection<MongoDB.Bson.BsonDocument>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mockCollection.Object);

            MongoHelper helper = new MongoHelper();
            helper.database = mockDb.Object;

            // Act
            List<string> result = await helper.GetAllDocumentIdsAsync("testCollection");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion
    }
}
