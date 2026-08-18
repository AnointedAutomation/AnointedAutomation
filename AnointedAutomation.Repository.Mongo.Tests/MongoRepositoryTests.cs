// Copyright 2026 Anointed Automation, LLC. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
// Unit tests for the MongoRepository base class: constructor validation and verification that every
// method delegates to IMongoHelper with the repository's collection name (Moq substitute, no database).

using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace AnointedAutomation.Repository.Mongo.Tests
{
    /// <summary>Concrete subclass so the abstract base can be instantiated in tests.</summary>
    public class TestDocRepository : MongoRepository<TestDoc>
    {
        public TestDocRepository(IMongoHelper mongo, string collectionName)
            : base(mongo, collectionName)
        {
        }
    }

    public class MongoRepositoryTests
    {
        private const string Collection = "test_docs";

        private readonly Mock<IMongoHelper> _mongo;
        private readonly TestDocRepository _repository;

        public MongoRepositoryTests()
        {
            _mongo = new Mock<IMongoHelper>(MockBehavior.Strict);
            _repository = new TestDocRepository(_mongo.Object, Collection);
        }

        [Fact]
        public void Constructor_NullHelper_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new TestDocRepository(null, Collection));
        }

        [Fact]
        public void Constructor_MissingCollectionName_Throws()
        {
            Assert.Throws<ArgumentException>(() => new TestDocRepository(_mongo.Object, null));
            Assert.Throws<ArgumentException>(() => new TestDocRepository(_mongo.Object, "  "));
        }

        [Fact]
        public async Task GetByIdAsync_DelegatesWithCollectionName()
        {
            TestDoc doc = new TestDoc { Name = "a" };
            _mongo.Setup(m => m.GetByIdAsync<TestDoc>(Collection, "id1")).ReturnsAsync(doc);

            Assert.Same(doc, await _repository.GetByIdAsync("id1"));
            _mongo.VerifyAll();
        }

        [Fact]
        public async Task GetSingleAsync_DelegatesWithCollectionName()
        {
            Expression<Func<TestDoc, bool>> filter = x => x.Value == 1;
            TestDoc doc = new TestDoc();
            _mongo.Setup(m => m.GetSingleAsync(Collection, filter)).ReturnsAsync(doc);

            Assert.Same(doc, await _repository.GetSingleAsync(filter));
            _mongo.VerifyAll();
        }

        [Fact]
        public async Task GetFilteredAsync_DelegatesWithCollectionName()
        {
            Expression<Func<TestDoc, bool>> filter = x => x.Value > 0;
            List<TestDoc> docs = new List<TestDoc>();
            _mongo.Setup(m => m.GetFilteredDocumentsAsync(Collection, filter)).ReturnsAsync(docs);

            Assert.Same(docs, await _repository.GetFilteredAsync(filter));
            _mongo.VerifyAll();
        }

        [Fact]
        public async Task GetAllAsync_DelegatesWithCollectionName()
        {
            List<TestDoc> docs = new List<TestDoc>();
            _mongo.Setup(m => m.GetAllDocumentsAsync<TestDoc>(Collection)).ReturnsAsync(docs);

            Assert.Same(docs, await _repository.GetAllAsync());
            _mongo.VerifyAll();
        }

        [Fact]
        public async Task GetPagedAsync_DelegatesWithCollectionName()
        {
            FilterDefinition<TestDoc> filter = FilterDefinition<TestDoc>.Empty;
            SortDefinition<TestDoc> sort = Builders<TestDoc>.Sort.Ascending(x => x.Value);
            List<TestDoc> docs = new List<TestDoc>();
            _mongo.Setup(m => m.GetPagedAsync(Collection, filter, sort, 5, 10)).ReturnsAsync(docs);

            Assert.Same(docs, await _repository.GetPagedAsync(filter, sort, 5, 10));
            _mongo.VerifyAll();
        }

        [Fact]
        public async Task CountAsync_DelegatesWithCollectionName()
        {
            Expression<Func<TestDoc, bool>> filter = x => x.Value == 2;
            _mongo.Setup(m => m.CountAsync(Collection, filter)).ReturnsAsync(3L);

            Assert.Equal(3L, await _repository.CountAsync(filter));
            _mongo.VerifyAll();
        }

        [Fact]
        public async Task ExistsAsync_DelegatesWithCollectionName()
        {
            Expression<Func<TestDoc, bool>> filter = x => x.Name == "a";
            _mongo.Setup(m => m.ExistsAsync(Collection, filter)).ReturnsAsync(true);

            Assert.True(await _repository.ExistsAsync(filter));
            _mongo.VerifyAll();
        }

        [Fact]
        public async Task CreateAsync_DelegatesWithCollectionName()
        {
            TestDoc doc = new TestDoc();
            _mongo.Setup(m => m.CreateDocumentAsync(Collection, doc)).Returns(Task.CompletedTask);

            await _repository.CreateAsync(doc);
            _mongo.VerifyAll();
        }

        [Fact]
        public async Task UpsertAsync_ConvertsPredicateAndDelegates()
        {
            UpdateDefinition<TestDoc> update = Builders<TestDoc>.Update.Set(x => x.Value, 9);
            UpdateResult result = new UpdateResult.Acknowledged(1, 1, null);
            _mongo
                .Setup(m => m.UpsertAsync(Collection, It.IsAny<FilterDefinition<TestDoc>>(), update))
                .ReturnsAsync(result);

            Assert.Same(result, await _repository.UpsertAsync(x => x.Name == "a", update));
            _mongo.VerifyAll();
        }

        [Fact]
        public async Task ReplaceByIdAsync_DelegatesWithCollectionName()
        {
            TestDoc doc = new TestDoc();
            ReplaceOneResult result = new ReplaceOneResult.Acknowledged(1, 1, null);
            _mongo.Setup(m => m.ReplaceByIdAsync(Collection, "id1", doc)).ReturnsAsync(result);

            Assert.Same(result, await _repository.ReplaceByIdAsync("id1", doc));
            _mongo.VerifyAll();
        }

        [Fact]
        public async Task UpdateByIdAsync_DelegatesWithCollectionName()
        {
            UpdateDefinition<TestDoc> update = Builders<TestDoc>.Update.Set(x => x.Value, 1);
            UpdateResult result = new UpdateResult.Acknowledged(1, 1, null);
            _mongo.Setup(m => m.UpdateByIdAsync(Collection, "id1", update)).ReturnsAsync(result);

            Assert.Same(result, await _repository.UpdateByIdAsync("id1", update));
            _mongo.VerifyAll();
        }

        [Fact]
        public async Task DeleteByIdAsync_DelegatesWithCollectionName()
        {
            DeleteResult result = new DeleteResult.Acknowledged(1);
            _mongo.Setup(m => m.DeleteByIdAsync<TestDoc>(Collection, "id1")).ReturnsAsync(result);

            Assert.Same(result, await _repository.DeleteByIdAsync("id1"));
            _mongo.VerifyAll();
        }

        [Fact]
        public async Task DeleteManyAsync_ConvertsPredicateAndDelegates()
        {
            DeleteResult result = new DeleteResult.Acknowledged(2);
            _mongo
                .Setup(m => m.DeleteManyAsync(Collection, It.IsAny<FilterDefinition<TestDoc>>()))
                .ReturnsAsync(result);

            Assert.Same(result, await _repository.DeleteManyAsync(x => x.Value < 0));
            _mongo.VerifyAll();
        }
    }
}
