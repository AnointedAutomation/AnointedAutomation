// Copyright 2026 Anointed Automation, LLC. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
// Integration tests for the standard query methods on MongoHelper, run against a real in-process MongoDB
// (EphemeralMongo). One mongod is started for the whole class; each test uses a unique collection so the
// tests stay isolated without tearing the server down between them.

using EphemeralMongo;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AnointedAutomation.Repository.Mongo.Tests
{
    /// <summary>A minimal document for exercising the query methods.</summary>
    public class TestDoc
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public int Value { get; set; }
    }

    /// <summary>Projection shape used by GetProjectedAsync tests.</summary>
    public class NameOnly
    {
        public string Name { get; set; }
    }

    /// <summary>Starts a single ephemeral mongod for the test class and exposes a wired MongoHelper.</summary>
    public class MongoServerFixture : IAsyncLifetime
    {
        public IMongoRunner Runner { get; private set; }

        public IMongoHelper Helper { get; private set; }

        public async Task InitializeAsync()
        {
            Runner = await MongoRunner.RunAsync(new MongoRunnerOptions { UseSingleNodeReplicaSet = false });
            Helper = new MongoHelper("standard_query_tests", Runner.ConnectionString);
        }

        public Task DisposeAsync()
        {
            Runner?.Dispose();
            return Task.CompletedTask;
        }
    }

    public class StandardQueryTests : IClassFixture<MongoServerFixture>
    {
        private readonly IMongoHelper _helper;

        public StandardQueryTests(MongoServerFixture fixture)
        {
            _helper = fixture.Helper;
        }

        // Unique collection per test so the shared server stays isolated.
        private static string NewCollection() => "c_" + Guid.NewGuid().ToString("N");

        private async Task<string> SeedAsync(string collection, params TestDoc[] docs)
        {
            await _helper.CreateManyAsync(collection, docs);
            return collection;
        }

        [Fact]
        public async Task GetCollection_ReturnsUsableTypedCollection()
        {
            string coll = NewCollection();
            IMongoCollection<TestDoc> collection = _helper.GetCollection<TestDoc>(coll);

            Assert.NotNull(collection);
            Assert.Equal(coll, collection.CollectionNamespace.CollectionName);

            await collection.InsertOneAsync(new TestDoc { Name = "direct", Value = 7 });
            long count = await collection.CountDocumentsAsync(Builders<TestDoc>.Filter.Eq(d => d.Name, "direct"));
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsMatchingDocument()
        {
            string coll = NewCollection();
            TestDoc doc = new TestDoc { Id = ObjectId.GenerateNewId().ToString(), Name = "alpha", Value = 1 };
            await _helper.CreateDocumentAsync(coll, doc);

            TestDoc result = await _helper.GetByIdAsync<TestDoc>(coll, doc.Id);

            Assert.NotNull(result);
            Assert.Equal("alpha", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_UnknownId_ReturnsNull()
        {
            string coll = NewCollection();
            TestDoc result = await _helper.GetByIdAsync<TestDoc>(coll, ObjectId.GenerateNewId().ToString());
            Assert.Null(result);
        }

        [Fact]
        public async Task GetSingleAsync_ReturnsFirstMatch_OrNull()
        {
            string coll = NewCollection();
            await SeedAsync(coll,
                new TestDoc { Name = "x", Value = 1 },
                new TestDoc { Name = "x", Value = 2 });

            TestDoc match = await _helper.GetSingleAsync(coll, Builders<TestDoc>.Filter.Eq(d => d.Name, "x"));
            TestDoc none = await _helper.GetSingleAsync(coll, Builders<TestDoc>.Filter.Eq(d => d.Name, "nope"));

            Assert.NotNull(match);
            Assert.Equal("x", match.Name);
            Assert.Null(none);
        }

        [Fact]
        public async Task CountAsync_CountsMatching_AndAll()
        {
            string coll = NewCollection();
            await SeedAsync(coll,
                new TestDoc { Name = "a", Value = 1 },
                new TestDoc { Name = "a", Value = 2 },
                new TestDoc { Name = "b", Value = 3 });

            long a = await _helper.CountAsync(coll, Builders<TestDoc>.Filter.Eq(d => d.Name, "a"));
            long all = await _helper.CountAsync(coll, FilterDefinition<TestDoc>.Empty);

            Assert.Equal(2, a);
            Assert.Equal(3, all);
        }

        [Fact]
        public async Task ExistsAsync_TrueWhenMatch_FalseOtherwise()
        {
            string coll = NewCollection();
            await SeedAsync(coll, new TestDoc { Name = "here", Value = 1 });

            Assert.True(await _helper.ExistsAsync(coll, Builders<TestDoc>.Filter.Eq(d => d.Name, "here")));
            Assert.False(await _helper.ExistsAsync(coll, Builders<TestDoc>.Filter.Eq(d => d.Name, "gone")));
        }

        [Fact]
        public async Task GetPagedAsync_SortsThenSkipsAndLimits()
        {
            string coll = NewCollection();
            await SeedAsync(coll,
                new TestDoc { Name = "n", Value = 3 },
                new TestDoc { Name = "n", Value = 1 },
                new TestDoc { Name = "n", Value = 2 },
                new TestDoc { Name = "n", Value = 4 });

            SortDefinition<TestDoc> sort = Builders<TestDoc>.Sort.Ascending(d => d.Value);
            List<TestDoc> page = await _helper.GetPagedAsync(coll, FilterDefinition<TestDoc>.Empty, sort, 1, 2);

            Assert.Equal(2, page.Count);
            Assert.Equal(2, page[0].Value); // skipped Value=1, took 2 and 3
            Assert.Equal(3, page[1].Value);
        }

        [Fact]
        public async Task GetPagedAsync_NullSort_StillReturnsPage()
        {
            string coll = NewCollection();
            await SeedAsync(coll,
                new TestDoc { Name = "n", Value = 1 },
                new TestDoc { Name = "n", Value = 2 },
                new TestDoc { Name = "n", Value = 3 });

            List<TestDoc> page = await _helper.GetPagedAsync(coll, FilterDefinition<TestDoc>.Empty, null, 0, 2);
            Assert.Equal(2, page.Count);
        }

        [Fact]
        public async Task GetProjectedAsync_ReturnsProjectedShape()
        {
            string coll = NewCollection();
            await SeedAsync(coll, new TestDoc { Name = "proj", Value = 9 });

            ProjectionDefinition<TestDoc, NameOnly> projection =
                Builders<TestDoc>.Projection.Expression(d => new NameOnly { Name = d.Name });
            List<NameOnly> results = await _helper.GetProjectedAsync(coll, FilterDefinition<TestDoc>.Empty, projection);

            Assert.Single(results);
            Assert.Equal("proj", results[0].Name);
        }

        [Fact]
        public async Task AggregateAsync_GroupsAndSums()
        {
            string coll = NewCollection();
            await SeedAsync(coll,
                new TestDoc { Name = "g", Value = 10 },
                new TestDoc { Name = "g", Value = 5 },
                new TestDoc { Name = "h", Value = 1 });

            PipelineDefinition<TestDoc, BsonDocument> pipeline = new EmptyPipelineDefinition<TestDoc>()
                .Group(new BsonDocument
                {
                    { "_id", "$Name" },
                    { "total", new BsonDocument("$sum", "$Value") }
                });

            List<BsonDocument> results = await _helper.AggregateAsync(coll, pipeline);

            BsonDocument g = results.First(r => r["_id"].AsString == "g");
            Assert.Equal(15, g["total"].AsInt32);
        }

        [Fact]
        public async Task DistinctAsync_ReturnsDistinctValues()
        {
            string coll = NewCollection();
            await SeedAsync(coll,
                new TestDoc { Name = "d1", Value = 1 },
                new TestDoc { Name = "d1", Value = 2 },
                new TestDoc { Name = "d2", Value = 3 });

            List<string> names = await _helper.DistinctAsync<TestDoc, string>(
                coll, "Name", FilterDefinition<TestDoc>.Empty);

            Assert.Equal(2, names.Count);
            Assert.Contains("d1", names);
            Assert.Contains("d2", names);
        }

        [Fact]
        public async Task CreateManyAsync_InsertsAll()
        {
            string coll = NewCollection();
            await _helper.CreateManyAsync(coll, new[]
            {
                new TestDoc { Name = "m", Value = 1 },
                new TestDoc { Name = "m", Value = 2 }
            });

            Assert.Equal(2, await _helper.CountAsync(coll, FilterDefinition<TestDoc>.Empty));
        }

        [Fact]
        public async Task UpdateManyAsync_UpdatesEveryMatch()
        {
            string coll = NewCollection();
            await SeedAsync(coll,
                new TestDoc { Name = "u", Value = 1 },
                new TestDoc { Name = "u", Value = 1 },
                new TestDoc { Name = "keep", Value = 1 });

            UpdateResult result = await _helper.UpdateManyAsync(coll,
                Builders<TestDoc>.Filter.Eq(d => d.Name, "u"),
                Builders<TestDoc>.Update.Set(d => d.Value, 99));

            Assert.Equal(2, result.ModifiedCount);
            long updated = await _helper.CountAsync(coll, Builders<TestDoc>.Filter.Eq(d => d.Value, 99));
            Assert.Equal(2, updated);
        }

        [Fact]
        public async Task UpsertAsync_InsertsWhenMissing_UpdatesWhenPresent()
        {
            string coll = NewCollection();
            FilterDefinition<TestDoc> filter = Builders<TestDoc>.Filter.Eq(d => d.Name, "up");

            // Missing -> insert.
            UpdateResult inserted = await _helper.UpsertAsync(coll, filter,
                Builders<TestDoc>.Update.Set(d => d.Name, "up").Set(d => d.Value, 1));
            Assert.NotNull(inserted.UpsertedId);
            Assert.Equal(1, await _helper.CountAsync(coll, filter));

            // Present -> update, no new doc.
            await _helper.UpsertAsync(coll, filter, Builders<TestDoc>.Update.Set(d => d.Value, 2));
            Assert.Equal(1, await _helper.CountAsync(coll, filter));
            TestDoc doc = await _helper.GetSingleAsync(coll, filter);
            Assert.Equal(2, doc.Value);
        }

        [Fact]
        public async Task DeleteManyAsync_DeletesEveryMatch()
        {
            string coll = NewCollection();
            await SeedAsync(coll,
                new TestDoc { Name = "del", Value = 1 },
                new TestDoc { Name = "del", Value = 2 },
                new TestDoc { Name = "stay", Value = 3 });

            DeleteResult result = await _helper.DeleteManyAsync(coll, Builders<TestDoc>.Filter.Eq(d => d.Name, "del"));

            Assert.Equal(2, result.DeletedCount);
            Assert.Equal(1, await _helper.CountAsync(coll, FilterDefinition<TestDoc>.Empty));
        }

        [Fact]
        public async Task FindOneAndUpdateAsync_ReturnsUpdatedByDefault_AndAtomicallyClaims()
        {
            string coll = NewCollection();
            await SeedAsync(coll, new TestDoc { Name = "claim", Value = 0 });

            TestDoc after = await _helper.FindOneAndUpdateAsync(coll,
                Builders<TestDoc>.Filter.Eq(d => d.Name, "claim"),
                Builders<TestDoc>.Update.Set(d => d.Value, 7));

            Assert.NotNull(after);
            Assert.Equal(7, after.Value); // returnUpdated default = true
        }

        [Fact]
        public async Task FindOneAndUpdateAsync_ReturnUpdatedFalse_ReturnsPreImage()
        {
            string coll = NewCollection();
            await SeedAsync(coll, new TestDoc { Name = "pre", Value = 5 });

            TestDoc before = await _helper.FindOneAndUpdateAsync(coll,
                Builders<TestDoc>.Filter.Eq(d => d.Name, "pre"),
                Builders<TestDoc>.Update.Set(d => d.Value, 6),
                returnUpdated: false);

            Assert.Equal(5, before.Value);
        }

        [Fact]
        public async Task FindOneAndUpdateAsync_NoMatchNoUpsert_ReturnsNull()
        {
            string coll = NewCollection();
            TestDoc result = await _helper.FindOneAndUpdateAsync(coll,
                Builders<TestDoc>.Filter.Eq(d => d.Name, "absent"),
                Builders<TestDoc>.Update.Set(d => d.Value, 1));
            Assert.Null(result);
        }

        [Fact]
        public async Task FindOneAndDeleteAsync_ReturnsAndRemovesDocument()
        {
            string coll = NewCollection();
            await SeedAsync(coll, new TestDoc { Name = "pop", Value = 42 });

            TestDoc deleted = await _helper.FindOneAndDeleteAsync(coll,
                Builders<TestDoc>.Filter.Eq(d => d.Name, "pop"));

            Assert.NotNull(deleted);
            Assert.Equal(42, deleted.Value);
            Assert.Equal(0, await _helper.CountAsync(coll, FilterDefinition<TestDoc>.Empty));
        }
    }
}
