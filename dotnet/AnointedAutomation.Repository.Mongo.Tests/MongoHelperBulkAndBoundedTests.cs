// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using AnointedAutomation.Repository.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;
using MongoDB.Driver.Core.Connections;
using MongoDB.Driver.Core.Servers;
using Moq;
using System.Net;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AnointedAutomation.Repository.Mongo.Tests
{
    public class MongoHelperBulkAndBoundedTests
    {
        private static (MongoHelper helper, Mock<IMongoCollection<MongoTestDoc>> collection) Build()
        {
            Mock<IMongoCollection<MongoTestDoc>> collection = new Mock<IMongoCollection<MongoTestDoc>>();
            Mock<IMongoDatabase> db = new Mock<IMongoDatabase>();
            db.Setup(d => d.GetCollection<MongoTestDoc>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(collection.Object);
            MongoHelper helper = new MongoHelper();
            helper.database = db.Object;
            return (helper, collection);
        }

        private static BulkWriteResult<MongoTestDoc> Acked(int count, List<WriteModel<MongoTestDoc>> requests)
        {
            return new BulkWriteResult<MongoTestDoc>.Acknowledged(count, 0, 0, count, count, requests, new List<BulkWriteUpsert>());
        }

        private static List<WriteModel<MongoTestDoc>> Models(int n)
        {
            return Enumerable.Range(0, n)
                .Select(i => (WriteModel<MongoTestDoc>)new InsertOneModel<MongoTestDoc>(new MongoTestDoc { _id = i.ToString(), Name = "n" }))
                .ToList();
        }

        [Fact]
        public async Task BulkWriteAsync_SplitsIntoBatchesAndSumsCounts()
        {
            (MongoHelper helper, Mock<IMongoCollection<MongoTestDoc>> collection) = Build();
            List<int> sizes = new List<int>();
            List<bool> ordered = new List<bool>();
            collection.Setup(c => c.BulkWriteAsync(It.IsAny<IEnumerable<WriteModel<MongoTestDoc>>>(), It.IsAny<BulkWriteOptions>(), It.IsAny<CancellationToken>()))
                .Returns((IEnumerable<WriteModel<MongoTestDoc>> r, BulkWriteOptions o, CancellationToken _) =>
                {
                    List<WriteModel<MongoTestDoc>> list = r.ToList();
                    sizes.Add(list.Count);
                    ordered.Add(o.IsOrdered);
                    return Task.FromResult(Acked(list.Count, list));
                });

            BulkWriteSummary summary = await helper.BulkWriteAsync("c", Models(MongoHelper.MaxBulkWriteBatch * 2 + 5));

            Assert.Equal(new List<int> { MongoHelper.MaxBulkWriteBatch, MongoHelper.MaxBulkWriteBatch, 5 }, sizes);
            Assert.All(ordered, o => Assert.False(o));
            Assert.Equal(3, summary.Batches);
            Assert.Equal(MongoHelper.MaxBulkWriteBatch * 2 + 5, summary.Inserted);
        }

        [Fact]
        public async Task BulkWriteAsync_Ordered_PassesOrderedOption()
        {
            (MongoHelper helper, Mock<IMongoCollection<MongoTestDoc>> collection) = Build();
            collection.Setup(c => c.BulkWriteAsync(It.IsAny<IEnumerable<WriteModel<MongoTestDoc>>>(), It.Is<BulkWriteOptions>(o => o.IsOrdered), It.IsAny<CancellationToken>()))
                .Returns((IEnumerable<WriteModel<MongoTestDoc>> r, BulkWriteOptions _, CancellationToken __) => Task.FromResult(Acked(r.Count(), r.ToList())));

            BulkWriteSummary summary = await helper.BulkWriteAsync("c", Models(2), ordered: true);

            Assert.Equal(1, summary.Batches);
            Assert.Equal(2, summary.Inserted);
        }

        private static MongoBulkWriteException<MongoTestDoc> BulkFailure(int batchSize, int failedIndex)
        {
            ConstructorInfo errorCtor = typeof(BulkWriteError).GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)[0];
            object[] errorArgs = errorCtor.GetParameters().Select(p => p.ParameterType == typeof(int) && p.Name == "index" ? (object)failedIndex
                : p.ParameterType == typeof(int) ? 11000
                : p.ParameterType == typeof(string) ? "duplicate key"
                : p.ParameterType.IsValueType ? Activator.CreateInstance(p.ParameterType) : null).ToArray();
            BulkWriteError error = (BulkWriteError)errorCtor.Invoke(errorArgs);
            ConnectionId connection = new ConnectionId(new ServerId(new ClusterId(), new DnsEndPoint("localhost", 27017)));
            List<WriteModel<MongoTestDoc>> processed = Models(batchSize);
            return new MongoBulkWriteException<MongoTestDoc>(connection, Acked(batchSize - 1, processed),
                new[] { error }, null, new List<WriteModel<MongoTestDoc>>());
        }

        [Fact]
        public async Task BulkWriteAsync_Unordered_FailedBatch_StillSendsLaterBatches_AndReportsAppliedCounts()
        {
            (MongoHelper helper, Mock<IMongoCollection<MongoTestDoc>> collection) = Build();
            int calls = 0;
            collection.Setup(c => c.BulkWriteAsync(It.IsAny<IEnumerable<WriteModel<MongoTestDoc>>>(), It.IsAny<BulkWriteOptions>(), It.IsAny<CancellationToken>()))
                .Returns((IEnumerable<WriteModel<MongoTestDoc>> r, BulkWriteOptions _, CancellationToken __) =>
                {
                    calls++;
                    List<WriteModel<MongoTestDoc>> list = r.ToList();
                    if (calls == 1)
                    {
                        throw BulkFailure(list.Count, 4);
                    }
                    return Task.FromResult(Acked(list.Count, list));
                });

            BulkWriteFailedException ex = await Assert.ThrowsAsync<BulkWriteFailedException>(() =>
                helper.BulkWriteAsync("c", Models(MongoHelper.MaxBulkWriteBatch + 3)));

            Assert.Equal(2, calls);
            Assert.Equal(2, ex.Summary.Batches);
            Assert.Equal(MongoHelper.MaxBulkWriteBatch - 1 + 3, ex.Summary.Inserted);
            BulkWriteFailure failure = Assert.Single(ex.Failures);
            Assert.Equal(4, failure.Index);
            Assert.Equal("c", ex.CollectionName);
        }

        [Fact]
        public async Task BulkWriteAsync_Unordered_FailureInLaterBatch_IndexIsOffsetIntoWholeList()
        {
            (MongoHelper helper, Mock<IMongoCollection<MongoTestDoc>> collection) = Build();
            int calls = 0;
            collection.Setup(c => c.BulkWriteAsync(It.IsAny<IEnumerable<WriteModel<MongoTestDoc>>>(), It.IsAny<BulkWriteOptions>(), It.IsAny<CancellationToken>()))
                .Returns((IEnumerable<WriteModel<MongoTestDoc>> r, BulkWriteOptions _, CancellationToken __) =>
                {
                    calls++;
                    List<WriteModel<MongoTestDoc>> list = r.ToList();
                    if (calls == 2)
                    {
                        throw BulkFailure(list.Count, 1);
                    }
                    return Task.FromResult(Acked(list.Count, list));
                });

            BulkWriteFailedException ex = await Assert.ThrowsAsync<BulkWriteFailedException>(() =>
                helper.BulkWriteAsync("c", Models(MongoHelper.MaxBulkWriteBatch + 3)));

            Assert.Equal(MongoHelper.MaxBulkWriteBatch + 1, Assert.Single(ex.Failures).Index);
        }

        [Fact]
        public async Task BulkWriteAsync_Ordered_FailedBatch_StopsBeforeLaterBatches()
        {
            (MongoHelper helper, Mock<IMongoCollection<MongoTestDoc>> collection) = Build();
            int calls = 0;
            collection.Setup(c => c.BulkWriteAsync(It.IsAny<IEnumerable<WriteModel<MongoTestDoc>>>(), It.IsAny<BulkWriteOptions>(), It.IsAny<CancellationToken>()))
                .Returns((IEnumerable<WriteModel<MongoTestDoc>> r, BulkWriteOptions _, CancellationToken __) =>
                {
                    calls++;
                    throw BulkFailure(r.Count(), 0);
                });

            BulkWriteFailedException ex = await Assert.ThrowsAsync<BulkWriteFailedException>(() =>
                helper.BulkWriteAsync("c", Models(MongoHelper.MaxBulkWriteBatch * 2), ordered: true));

            Assert.Equal(1, calls);
            Assert.Equal(1, ex.Summary.Batches);
        }

        [Fact]
        public async Task BulkWriteAsync_Empty_MakesNoCall()
        {
            (MongoHelper helper, Mock<IMongoCollection<MongoTestDoc>> collection) = Build();

            BulkWriteSummary summary = await helper.BulkWriteAsync("c", new List<WriteModel<MongoTestDoc>>());

            Assert.Equal(BulkWriteSummary.Empty, summary);
            collection.Verify(c => c.BulkWriteAsync(It.IsAny<IEnumerable<WriteModel<MongoTestDoc>>>(), It.IsAny<BulkWriteOptions>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task BulkWriteAsync_NullModels_Throws()
        {
            (MongoHelper helper, _) = Build();
            await Assert.ThrowsAsync<ArgumentNullException>(() => helper.BulkWriteAsync<MongoTestDoc>("c", null));
        }

        [Fact]
        public async Task BulkWriteAsync_NullModelInList_Throws()
        {
            (MongoHelper helper, _) = Build();
            await Assert.ThrowsAsync<ArgumentException>(() => helper.BulkWriteAsync("c", new List<WriteModel<MongoTestDoc>> { null }));
        }

        [Fact]
        public void BulkWriteSummary_Unacknowledged_CountsBatchOnly()
        {
            BulkWriteSummary summary = BulkWriteSummary.Empty.Add(new BulkWriteResult<MongoTestDoc>.Unacknowledged(3, Models(3)));

            Assert.Equal(1, summary.Batches);
            Assert.Equal(0, summary.Inserted);
        }

        [Fact]
        public void BulkWriteSummary_NullResult_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => BulkWriteSummary.Empty.Add<MongoTestDoc>(null));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetFilteredDocumentsAsync_Bounded_RejectsNonPositiveLimit(int limit)
        {
            (MongoHelper helper, _) = Build();
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                helper.GetFilteredDocumentsAsync("c", Builders<MongoTestDoc>.Filter.Empty, null, limit));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetProjectedAsync_Bounded_RejectsNonPositiveLimit(int limit)
        {
            (MongoHelper helper, _) = Build();
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                helper.GetProjectedAsync("c", Builders<MongoTestDoc>.Filter.Empty, Builders<MongoTestDoc>.Projection.Expression(d => d.Name), null, limit));
        }

        [Fact]
        public async Task GetProjectedAsync_Bounded_NullProjection_Throws()
        {
            (MongoHelper helper, _) = Build();
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                helper.GetProjectedAsync<MongoTestDoc, string>("c", Builders<MongoTestDoc>.Filter.Empty, null, null, 5));
        }

        [Fact]
        public async Task GetFilteredDocumentsAsync_Bounded_SendsLimitAndSort()
        {
            (MongoHelper helper, Mock<IMongoCollection<MongoTestDoc>> collection) = Build();
            FindOptions<MongoTestDoc, MongoTestDoc> seen = null;
            Mock<IAsyncCursor<MongoTestDoc>> cursor = EmptyCursor<MongoTestDoc>();
            collection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<MongoTestDoc>>(), It.IsAny<FindOptions<MongoTestDoc, MongoTestDoc>>(), It.IsAny<CancellationToken>()))
                .Callback((FilterDefinition<MongoTestDoc> _, FindOptions<MongoTestDoc, MongoTestDoc> o, CancellationToken __) => seen = o)
                .ReturnsAsync(cursor.Object);

            await helper.GetFilteredDocumentsAsync("c", Builders<MongoTestDoc>.Filter.Empty, Builders<MongoTestDoc>.Sort.Ascending(d => d.Name), 7);

            Assert.NotNull(seen);
            Assert.Equal(7, seen.Limit);
            Assert.NotNull(seen.Sort);
        }

        [Fact]
        public async Task GetProjectedAsync_Bounded_SendsLimitAndProjection()
        {
            (MongoHelper helper, Mock<IMongoCollection<MongoTestDoc>> collection) = Build();
            FindOptions<MongoTestDoc, string> seen = null;
            Mock<IAsyncCursor<string>> cursor = EmptyCursor<string>();
            collection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<MongoTestDoc>>(), It.IsAny<FindOptions<MongoTestDoc, string>>(), It.IsAny<CancellationToken>()))
                .Callback((FilterDefinition<MongoTestDoc> _, FindOptions<MongoTestDoc, string> o, CancellationToken __) => seen = o)
                .ReturnsAsync(cursor.Object);

            await helper.GetProjectedAsync("c", Builders<MongoTestDoc>.Filter.Empty, Builders<MongoTestDoc>.Projection.Expression(d => d.Name), null, 3);

            Assert.NotNull(seen);
            Assert.Equal(3, seen.Limit);
            Assert.NotNull(seen.Projection);
            Assert.Null(seen.Sort);
        }

        [Fact]
        public async Task DelegatingHelper_ForwardsNewMembers()
        {
            Mock<IMongoHelper> inner = new Mock<IMongoHelper>();
            inner.Setup(h => h.BulkWriteAsync("c", It.IsAny<IEnumerable<WriteModel<MongoTestDoc>>>(), true)).ReturnsAsync(BulkWriteSummary.Empty);
            inner.Setup(h => h.GetFilteredDocumentsAsync("c", It.IsAny<FilterDefinition<MongoTestDoc>>(), null, 4)).ReturnsAsync(new List<MongoTestDoc>());
            inner.Setup(h => h.GetProjectedAsync("c", It.IsAny<FilterDefinition<MongoTestDoc>>(), It.IsAny<ProjectionDefinition<MongoTestDoc, string>>(), null, 4)).ReturnsAsync(new List<string>());
            Forwarder forwarder = new Forwarder(inner.Object);

            await forwarder.BulkWriteAsync("c", Models(1), true);
            await forwarder.GetFilteredDocumentsAsync("c", Builders<MongoTestDoc>.Filter.Empty, null, 4);
            await forwarder.GetProjectedAsync("c", Builders<MongoTestDoc>.Filter.Empty, Builders<MongoTestDoc>.Projection.Expression(d => d.Name), null, 4);

            inner.VerifyAll();
        }

        private sealed class Forwarder : DelegatingMongoHelper
        {
            private readonly IMongoHelper _inner;
            public Forwarder(IMongoHelper inner) { _inner = inner; }
            protected override IMongoHelper Current() => _inner;
        }

        private static Mock<IAsyncCursor<TItem>> EmptyCursor<TItem>()
        {
            Mock<IAsyncCursor<TItem>> cursor = new Mock<IAsyncCursor<TItem>>();
            cursor.Setup(c => c.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);
            cursor.Setup(c => c.MoveNext(It.IsAny<CancellationToken>())).Returns(false);
            return cursor;
        }
    }
}
