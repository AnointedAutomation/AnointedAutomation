// Copyright 2026 Anointed Automation, LLC. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
// Unit tests for MongoIndexSpec construction validation and deterministic name resolution, plus
// integration tests for MongoHelper.EnsureIndexAsync/EnsureIndexesAsync against EphemeralMongo.

using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AnointedAutomation.Repository.Mongo.Tests
{
    public class MongoIndexSpecTests
    {
        [Fact]
        public void Constructor_MissingField_Throws()
        {
            Assert.Throws<ArgumentException>(() => new MongoIndexSpec(""));
            Assert.Throws<ArgumentException>(() => new MongoIndexKey(null));
            Assert.Throws<ArgumentNullException>(() => new MongoIndexSpec((IReadOnlyList<MongoIndexKey>)null));
            Assert.Throws<ArgumentException>(() => new MongoIndexSpec(new List<MongoIndexKey>()));
            Assert.Throws<ArgumentException>(() => new MongoIndexSpec(new List<MongoIndexKey> { null }));
        }

        [Fact]
        public void ResolveName_SingleAscending_IsDeterministic()
        {
            MongoIndexSpec spec = new MongoIndexSpec("Email");
            Assert.Equal("Email_1", spec.ResolveName());
            Assert.Equal(spec.ResolveName(), new MongoIndexSpec("Email").ResolveName());
        }

        [Fact]
        public void ResolveName_DescendingUniqueAndTtl_EncodeIntoName()
        {
            Assert.Equal("CreatedAt_-1", new MongoIndexSpec("CreatedAt", descending: true).ResolveName());
            Assert.Equal("Email_1_unique", new MongoIndexSpec("Email", unique: true).ResolveName());
            Assert.Equal(
                "ExpiresAt_1_ttl86400",
                new MongoIndexSpec("ExpiresAt", expireAfter: TimeSpan.FromDays(1)).ResolveName());
        }

        [Fact]
        public void ResolveName_CompositeKeys_JoinInOrder()
        {
            MongoIndexSpec spec = new MongoIndexSpec(new List<MongoIndexKey>
            {
                new MongoIndexKey("OrderId"),
                new MongoIndexKey("CreatedAt", descending: true)
            });
            Assert.Equal("OrderId_1_CreatedAt_-1", spec.ResolveName());
        }

        [Fact]
        public void ResolveName_ExplicitName_Wins()
        {
            MongoIndexSpec spec = new MongoIndexSpec("Email", unique: true, name: "my_custom_name");
            Assert.Equal("my_custom_name", spec.ResolveName());
        }
    }

    public class EnsureIndexTests : IClassFixture<MongoServerFixture>
    {
        private readonly IMongoHelper _helper;
        private readonly MongoServerFixture _fixture;

        public EnsureIndexTests(MongoServerFixture fixture)
        {
            _fixture = fixture;
            _helper = fixture.Helper;
        }

        private async Task<List<BsonDocument>> ListIndexesAsync(string collectionName)
        {
            IMongoCollection<BsonDocument> collection = _helper.database.GetCollection<BsonDocument>(collectionName);
            return await (await collection.Indexes.ListAsync()).ToListAsync();
        }

        [Fact]
        public async Task EnsureIndexAsync_CreatesIndex_AndIsIdempotent()
        {
            string collectionName = "ensure_index_" + Guid.NewGuid().ToString("N");
            MongoIndexSpec spec = new MongoIndexSpec("Email", unique: true);

            await _helper.EnsureIndexAsync(collectionName, spec);
            await _helper.EnsureIndexAsync(collectionName, spec);

            List<BsonDocument> indexes = await ListIndexesAsync(collectionName);
            List<BsonDocument> matches = indexes.Where(i => i["name"].AsString.Equals("Email_1_unique")).ToList();
            Assert.Single(matches);
            Assert.True(matches[0]["unique"].AsBoolean);
        }

        [Fact]
        public async Task EnsureIndexAsync_Ttl_SetsExpireAfterSeconds()
        {
            string collectionName = "ensure_ttl_" + Guid.NewGuid().ToString("N");
            MongoIndexSpec spec = new MongoIndexSpec("ExpiresAt", expireAfter: TimeSpan.FromHours(2));

            await _helper.EnsureIndexAsync(collectionName, spec);

            List<BsonDocument> indexes = await ListIndexesAsync(collectionName);
            BsonDocument ttlIndex = indexes.Single(i => i["name"].AsString.Equals("ExpiresAt_1_ttl7200"));
            Assert.Equal(7200d, ttlIndex["expireAfterSeconds"].ToDouble());
        }

        [Fact]
        public async Task EnsureIndexesAsync_CreatesAllSpecs()
        {
            string collectionName = "ensure_many_" + Guid.NewGuid().ToString("N");
            List<MongoIndexSpec> specs = new List<MongoIndexSpec>
            {
                new MongoIndexSpec("A"),
                new MongoIndexSpec(new List<MongoIndexKey>
                {
                    new MongoIndexKey("B"),
                    new MongoIndexKey("C", descending: true)
                })
            };

            await _helper.EnsureIndexesAsync(collectionName, specs);

            List<BsonDocument> indexes = await ListIndexesAsync(collectionName);
            List<string> names = indexes.ConvertAll(i => i["name"].AsString);
            Assert.Contains("A_1", names);
            Assert.Contains("B_1_C_-1", names);
        }

        [Fact]
        public async Task EnsureIndexAsync_MissingArguments_Throw()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _helper.EnsureIndexAsync(" ", new MongoIndexSpec("X")));
            await Assert.ThrowsAsync<ArgumentNullException>(() => _helper.EnsureIndexAsync("c", null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => _helper.EnsureIndexesAsync("c", null));
        }
    }
}
