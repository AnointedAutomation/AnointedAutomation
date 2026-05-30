// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
using Xunit;

namespace AnointedAutomation.Repository.Mongo.Tests
{
    /// <summary>
    /// Tests for the MongoHelperFactory class.
    /// </summary>
    public class MongoHelperFactoryTests
    {
        [Fact]
        public void Create_ReturnsNewMongoHelper_WhenNotCached()
        {
            // Arrange
            MongoHelperFactory factory = new MongoHelperFactory();
            string dbName = "test-db";
            string connectionString = "mongodb://localhost:27017";

            // Act
            IMongoHelper result = factory.Create(dbName, connectionString);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.database);
            Assert.Equal(dbName, result.dbName);
        }

        [Fact]
        public void Create_ReturnsCachedMongoHelper_WhenCalledTwice()
        {
            // Arrange
            MongoHelperFactory factory = new MongoHelperFactory();
            string dbName = "test-db";
            string connectionString = "mongodb://localhost:27017";

            // Act
            IMongoHelper first = factory.Create(dbName, connectionString);
            IMongoHelper second = factory.Create(dbName, connectionString);

            // Assert
            Assert.Same(first, second);
        }

        [Fact]
        public void Create_ReturnsDifferentMongoHelpers_ForDifferentDatabases()
        {
            // Arrange
            MongoHelperFactory factory = new MongoHelperFactory();
            string dbName1 = "test-db-1";
            string dbName2 = "test-db-2";
            string connectionString = "mongodb://localhost:27017";

            // Act
            IMongoHelper first = factory.Create(dbName1, connectionString);
            IMongoHelper second = factory.Create(dbName2, connectionString);

            // Assert
            Assert.NotSame(first, second);
            Assert.Equal(dbName1, first.dbName);
            Assert.Equal(dbName2, second.dbName);
        }

        [Fact]
        public void Create_ReturnsDifferentMongoHelpers_ForDifferentConnectionStrings()
        {
            // Arrange
            MongoHelperFactory factory = new MongoHelperFactory();
            string dbName = "test-db";
            string connectionString1 = "mongodb://localhost:27017";
            string connectionString2 = "mongodb://localhost:27018";

            // Act
            IMongoHelper first = factory.Create(dbName, connectionString1);
            IMongoHelper second = factory.Create(dbName, connectionString2);

            // Assert
            Assert.NotSame(first, second);
        }

        [Fact]
        public void Create_SetsDbNameCorrectly()
        {
            // Arrange
            MongoHelperFactory factory = new MongoHelperFactory();
            string expectedDbName = "my-database";
            string connectionString = "mongodb://localhost:27017";

            // Act
            IMongoHelper result = factory.Create(expectedDbName, connectionString);

            // Assert
            Assert.Equal(expectedDbName, result.dbName);
        }

        [Fact]
        public void Create_SetsDatabaseInstanceCorrectly()
        {
            // Arrange
            MongoHelperFactory factory = new MongoHelperFactory();
            string dbName = "test-db";
            string connectionString = "mongodb://localhost:27017";

            // Act
            IMongoHelper result = factory.Create(dbName, connectionString);

            // Assert
            Assert.NotNull(result.database);
        }

        [Fact]
        public void IMongoHelperFactory_Interface_IsImplemented()
        {
            // Arrange
            MongoHelperFactory factory = new MongoHelperFactory();

            // Assert
            Assert.IsAssignableFrom<IMongoHelperFactory>(factory);
        }
    }
}
