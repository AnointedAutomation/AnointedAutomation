// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Integration tests for GetPageAsync / GetDocumentIdPageAsync against a real in-process MongoDB (EphemeralMongo).

using AnointedAutomation.Objects;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AnointedAutomation.Repository.Mongo.Tests
{
    public class PageAsyncTests : IClassFixture<MongoServerFixture>
    {
        private readonly IMongoHelper _helper;

        public PageAsyncTests(MongoServerFixture fixture)
        {
            _helper = fixture.Helper;
        }

        private static string NewCollection() => "p_" + Guid.NewGuid().ToString("N");

        private static readonly SortDefinition<TestDoc> ByValue = Builders<TestDoc>.Sort.Ascending(d => d.Value);

        private async Task<string> SeedValuesAsync(int count, string name = "n")
        {
            string coll = NewCollection();
            TestDoc[] docs = Enumerable.Range(1, count).Select(i => new TestDoc { Name = name, Value = i }).Reverse().ToArray();
            await _helper.CreateManyAsync(coll, docs);
            return coll;
        }

        [Theory]
        [InlineData(1, new[] { 1, 2, 3 }, false, true)]
        [InlineData(2, new[] { 4, 5, 6 }, true, true)]
        [InlineData(3, new[] { 7 }, true, false)]
        public async Task GetPageAsync_ReturnsPageAndTotals(int page, int[] expected, bool hasPrev, bool hasNext)
        {
            string coll = await SeedValuesAsync(7);

            PaginatedResponse<TestDoc> result = await _helper.GetPageAsync(coll, FilterDefinition<TestDoc>.Empty, ByValue, page, 3);

            Assert.Equal(expected, result.Data.Select(d => d.Value).ToArray());
            Assert.Equal(page, result.currentPage);
            Assert.Equal(3, result.pageSize);
            Assert.Equal(7L, result.totalItems);
            Assert.Equal(3, result.totalPages);
            Assert.Equal(hasPrev, result.hasPreviousPage);
            Assert.Equal(hasNext, result.hasNextPage);
        }

        [Fact]
        public async Task GetPageAsync_ExactMultiple_UsesIntegerCeiling()
        {
            string coll = await SeedValuesAsync(6);

            PaginatedResponse<TestDoc> result = await _helper.GetPageAsync(coll, FilterDefinition<TestDoc>.Empty, ByValue, 1, 3);

            Assert.Equal(2, result.totalPages);
        }

        [Fact]
        public async Task GetPageAsync_EmptyCollection_ZeroTotalZeroPages()
        {
            PaginatedResponse<TestDoc> result = await _helper.GetPageAsync(NewCollection(), FilterDefinition<TestDoc>.Empty, ByValue, 1, 10);

            Assert.Empty(result.Data);
            Assert.Equal(0L, result.totalItems);
            Assert.Equal(0, result.totalPages);
            Assert.False(result.hasNextPage);
        }

        [Fact]
        public async Task GetPageAsync_PastTheEnd_EmptyDataButRealTotals()
        {
            string coll = await SeedValuesAsync(4);

            PaginatedResponse<TestDoc> result = await _helper.GetPageAsync(coll, FilterDefinition<TestDoc>.Empty, ByValue, 5, 2);

            Assert.Empty(result.Data);
            Assert.Equal(4L, result.totalItems);
            Assert.Equal(2, result.totalPages);
            Assert.False(result.hasNextPage);
        }

        [Fact]
        public async Task GetPageAsync_TotalCountsFilterNotWholeCollection_AndSortRespected()
        {
            string coll = NewCollection();
            await _helper.CreateManyAsync(coll, new[]
            {
                new TestDoc { Name = "keep", Value = 5 },
                new TestDoc { Name = "drop", Value = 1 },
                new TestDoc { Name = "keep", Value = 9 },
                new TestDoc { Name = "keep", Value = 2 },
                new TestDoc { Name = "drop", Value = 3 },
            });
            FilterDefinition<TestDoc> filter = Builders<TestDoc>.Filter.Eq(d => d.Name, "keep");
            SortDefinition<TestDoc> desc = Builders<TestDoc>.Sort.Descending(d => d.Value);

            PaginatedResponse<TestDoc> result = await _helper.GetPageAsync(coll, filter, desc, 1, 2);

            Assert.Equal(new[] { 9, 5 }, result.Data.Select(d => d.Value).ToArray());
            Assert.All(result.Data, d => Assert.Equal("keep", d.Name));
            Assert.Equal(3L, result.totalItems);
            Assert.Equal(2, result.totalPages);
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(-1, 10)]
        [InlineData(1, 0)]
        [InlineData(1, MongoPaging.MaxPageSize + 1)]
        [InlineData(int.MaxValue, MongoPaging.MaxPageSize)]
        public async Task GetPageAsync_InvalidArguments_Throw(int page, int pageSize)
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => _helper.GetPageAsync(NewCollection(), FilterDefinition<TestDoc>.Empty, ByValue, page, pageSize));
        }

        [Fact]
        public async Task GetPageAsync_MaxPageSize_IsAccepted()
        {
            PaginatedResponse<TestDoc> result = await _helper.GetPageAsync(NewCollection(), FilterDefinition<TestDoc>.Empty, ByValue, 1, MongoPaging.MaxPageSize);

            Assert.Equal(MongoPaging.MaxPageSize, result.pageSize);
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(1, 0)]
        [InlineData(1, MongoPaging.MaxPageSize + 1)]
        public async Task GetDocumentIdPageAsync_InvalidArguments_Throw(int page, int pageSize)
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _helper.GetDocumentIdPageAsync(NewCollection(), page, pageSize));
        }

        [Fact]
        public async Task GetDocumentIdPageAsync_ReturnsOrderedIdsAndTotals()
        {
            string coll = NewCollection();
            List<string> ids = Enumerable.Range(0, 5).Select(_ => ObjectId.GenerateNewId().ToString()).ToList();
            await _helper.CreateManyAsync(coll, ids.Select(id => new TestDoc { Id = id, Name = "secret", Value = 1 }).ToArray());
            List<string> sorted = ids.OrderBy(i => i, StringComparer.Ordinal).ToList();

            PaginatedResponse<string> page2 = await _helper.GetDocumentIdPageAsync(coll, 2, 2);

            Assert.Equal(sorted.Skip(2).Take(2).ToList(), page2.Data);
            Assert.Equal(5L, page2.totalItems);
            Assert.Equal(3, page2.totalPages);
            Assert.All(page2.Data, id => Assert.DoesNotContain("secret", id));
        }

        [Fact]
        public async Task GetDocumentIdPageAsync_EmptyCollection_ZeroTotalZeroPages()
        {
            PaginatedResponse<string> result = await _helper.GetDocumentIdPageAsync(NewCollection(), 1, 10);

            Assert.Empty(result.Data);
            Assert.Equal(0L, result.totalItems);
            Assert.Equal(0, result.totalPages);
        }

        [Fact]
        public async Task DelegatingHelper_ForwardsBothPageReads()
        {
            string coll = await SeedValuesAsync(3);
            Forward forward = new Forward(_helper);

            PaginatedResponse<TestDoc> docs = await forward.GetPageAsync(coll, FilterDefinition<TestDoc>.Empty, ByValue, 2, 2);
            PaginatedResponse<string> idPage = await forward.GetDocumentIdPageAsync(coll, 1, 2);

            Assert.Equal(new[] { 3 }, docs.Data.Select(d => d.Value).ToArray());
            Assert.Equal(3L, idPage.totalItems);
            Assert.Equal(2, idPage.Data.Count);
        }

        private sealed class Forward : DelegatingMongoHelper
        {
            private readonly IMongoHelper _inner;
            public Forward(IMongoHelper inner) { _inner = inner; }
            protected override IMongoHelper Current() => _inner;
        }
    }
}
