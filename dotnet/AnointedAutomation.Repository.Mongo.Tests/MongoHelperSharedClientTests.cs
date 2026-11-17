// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-17 Jesus is King ✝️
using MongoDB.Driver;
using System;
using Xunit;

namespace AnointedAutomation.Repository.Mongo.Tests
{
    /// <summary>
    /// Tests that MongoHelper reuses one MongoClient (one connection pool) per connection string.
    /// Constructing a MongoClient does not connect, so these run without a server.
    /// </summary>
    public class MongoHelperSharedClientTests
    {
        /// <summary>The same connection string returns the same client instance.</summary>
        [Fact]
        public void GetSharedClient_SameConnectionString_ReturnsSameClient()
        {
            string connectionString = "mongodb://shared-client-test-a:27017";

            MongoClient first = MongoHelper.GetSharedClient(connectionString);
            MongoClient second = MongoHelper.GetSharedClient(connectionString);

            Assert.Same(first, second);
        }

        /// <summary>Different connection strings get different clients.</summary>
        [Fact]
        public void GetSharedClient_DifferentConnectionStrings_ReturnDifferentClients()
        {
            MongoClient first = MongoHelper.GetSharedClient("mongodb://shared-client-test-b:27017");
            MongoClient second = MongoHelper.GetSharedClient("mongodb://shared-client-test-c:27017");

            Assert.NotSame(first, second);
        }

        /// <summary>A null or empty connection string fails fast.</summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GetSharedClient_MissingConnectionString_Throws(string connectionString)
        {
            Assert.Throws<ArgumentException>(() => MongoHelper.GetSharedClient(connectionString));
        }

        /// <summary>CreateMongoDbInstance called repeatedly shares the client behind the returned databases.</summary>
        [Fact]
        public void CreateMongoDbInstance_CalledTwice_SharesOneClient()
        {
            MongoHelper helper = new MongoHelper();
            string connectionString = "mongodb://shared-client-test-d:27017";

            IMongoDatabase first = helper.CreateMongoDbInstance("db1", connectionString);
            IMongoDatabase second = helper.CreateMongoDbInstance("db2", connectionString);

            Assert.Same(first.Client, second.Client);
        }

        /// <summary>The connection-string constructor shares the client too.</summary>
        [Fact]
        public void Constructor_WithConnectionString_SharesClientWithCreateMongoDbInstance()
        {
            string connectionString = "mongodb://shared-client-test-e:27017";

            MongoHelper helper = new MongoHelper("dbx", connectionString);
            IMongoDatabase other = new MongoHelper().CreateMongoDbInstance("dby", connectionString);

            Assert.Same(helper.database.Client, other.Client);
        }
    }
}
