// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Copyright 2026 Anointed Automation, LLC. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
// Unit tests for the stamping repositories: every write sets the timestamps, and the historied repository records
// a capped UpdateStamp. Uses a Moq substitute for IMongoHelper and a fixed clock, no database.

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace AnointedAutomation.Repository.Mongo.Tests
{
    public class StampCreatedDoc : CreatedMongoDocument
    {
        public string Name { get; set; }
    }

    public class StampTimestampedDoc : TimestampedMongoDocument
    {
        public string Name { get; set; }
    }

    public class StampHistoriedDoc : HistoriedMongoDocument
    {
        public string Name { get; set; }
    }

    public class TimestampedMongoRepositoriesTests
    {
        private const string Collection = "stamp_docs";
        private static readonly DateTime Now = new DateTime(2026, 9, 27, 12, 0, 0, DateTimeKind.Utc);

        private sealed class CreatedRepo : CreatedMongoRepository<StampCreatedDoc>
        {
            public CreatedRepo(IMongoHelper mongo) : base(mongo, Collection) { }
            protected override DateTime UtcNow => Now;
        }

        private sealed class TimestampedRepo : TimestampedMongoRepository<StampTimestampedDoc>
        {
            public TimestampedRepo(IMongoHelper mongo) : base(mongo, Collection) { }
            protected override DateTime UtcNow => Now;
        }

        private sealed class HistoriedRepo : HistoriedMongoRepository<StampHistoriedDoc>
        {
            public HistoriedRepo(IMongoHelper mongo) : base(mongo, Collection) { }
            protected override DateTime UtcNow => Now;
            protected override int UpdateHistoryLimit => 3;
        }

        private static BsonDocument Render<TDoc>(UpdateDefinition<TDoc> update)
        {
            IBsonSerializer<TDoc> serializer = BsonSerializer.SerializerRegistry.GetSerializer<TDoc>();
            return update.Render(new RenderArgs<TDoc>(serializer, BsonSerializer.SerializerRegistry)).AsBsonDocument;
        }

        private static string ElementName(Type declaringType, string memberName)
        {
            return BsonClassMap.LookupClassMap(declaringType).GetMemberMap(memberName).ElementName;
        }

        #region CreatedMongoRepository

        [Fact]
        public async Task Created_CreateAsync_StampsCreatedAt()
        {
            Mock<IMongoHelper> mongo = new Mock<IMongoHelper>(MockBehavior.Strict);
            StampCreatedDoc doc = new StampCreatedDoc { Name = "a" };
            mongo.Setup(m => m.CreateDocumentAsync(Collection, doc)).Returns(Task.CompletedTask);

            await new CreatedRepo(mongo.Object).CreateAsync(doc);

            Assert.Equal(Now, doc.CreatedAt);
            mongo.VerifyAll();
        }

        [Fact]
        public async Task Created_CreateAsync_NullDocument_Throws()
        {
            CreatedRepo repo = new CreatedRepo(new Mock<IMongoHelper>(MockBehavior.Strict).Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => repo.CreateAsync(null));
        }

        #endregion

        #region TimestampedMongoRepository

        [Fact]
        public async Task Timestamped_CreateAsync_StampsBothTimestampsEqual()
        {
            Mock<IMongoHelper> mongo = new Mock<IMongoHelper>(MockBehavior.Strict);
            StampTimestampedDoc doc = new StampTimestampedDoc();
            mongo.Setup(m => m.CreateDocumentAsync(Collection, doc)).Returns(Task.CompletedTask);

            await new TimestampedRepo(mongo.Object).CreateAsync(doc);

            Assert.Equal(Now, doc.CreatedAt);
            Assert.Equal(Now, doc.UpdatedAt);
        }

        [Fact]
        public async Task Timestamped_UpdateByIdAsync_AddsUpdatedAtToCallerUpdate()
        {
            Mock<IMongoHelper> mongo = new Mock<IMongoHelper>(MockBehavior.Strict);
            UpdateDefinition<StampTimestampedDoc> captured = null;
            UpdateResult result = new UpdateResult.Acknowledged(1, 1, null);
            mongo.Setup(m => m.UpdateByIdAsync(Collection, "id1", It.IsAny<UpdateDefinition<StampTimestampedDoc>>()))
                .Callback<string, string, UpdateDefinition<StampTimestampedDoc>>((_, _, u) => captured = u)
                .ReturnsAsync(result);

            UpdateResult actual = await new TimestampedRepo(mongo.Object)
                .UpdateByIdAsync("id1", Builders<StampTimestampedDoc>.Update.Set(d => d.Name, "b"));

            Assert.Same(result, actual);
            BsonDocument set = Render(captured)["$set"].AsBsonDocument;
            Assert.Equal("b", set[ElementName(typeof(StampTimestampedDoc), nameof(StampTimestampedDoc.Name))].AsString);
            Assert.Equal(Now, set[ElementName(typeof(TimestampedMongoDocument), nameof(TimestampedMongoDocument.UpdatedAt))].ToUniversalTime());
        }

        [Fact]
        public async Task Timestamped_UpsertAsync_SetsUpdatedAtAndCreatedAtOnInsert()
        {
            Mock<IMongoHelper> mongo = new Mock<IMongoHelper>(MockBehavior.Strict);
            UpdateDefinition<StampTimestampedDoc> captured = null;
            mongo.Setup(m => m.UpsertAsync(Collection, It.IsAny<FilterDefinition<StampTimestampedDoc>>(), It.IsAny<UpdateDefinition<StampTimestampedDoc>>()))
                .Callback<string, FilterDefinition<StampTimestampedDoc>, UpdateDefinition<StampTimestampedDoc>>((_, _, u) => captured = u)
                .ReturnsAsync(new UpdateResult.Acknowledged(0, 0, null));

            await new TimestampedRepo(mongo.Object)
                .UpsertAsync(d => d.Name == "a", Builders<StampTimestampedDoc>.Update.Set(d => d.Name, "a"));

            BsonDocument rendered = Render(captured);
            string createdName = ElementName(typeof(CreatedMongoDocument), nameof(CreatedMongoDocument.CreatedAt));
            string updatedName = ElementName(typeof(TimestampedMongoDocument), nameof(TimestampedMongoDocument.UpdatedAt));
            Assert.Equal(Now, rendered["$set"][updatedName].ToUniversalTime());
            Assert.Equal(Now, rendered["$setOnInsert"][createdName].ToUniversalTime());
        }

        [Fact]
        public async Task Timestamped_ReplaceByIdAsync_StampsUpdatedAtKeepsCreatedAt()
        {
            Mock<IMongoHelper> mongo = new Mock<IMongoHelper>(MockBehavior.Strict);
            DateTime created = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            StampTimestampedDoc doc = new StampTimestampedDoc { CreatedAt = created };
            mongo.Setup(m => m.ReplaceByIdAsync(Collection, "id1", doc))
                .ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, null));

            await new TimestampedRepo(mongo.Object).ReplaceByIdAsync("id1", doc);

            Assert.Equal(created, doc.CreatedAt);
            Assert.Equal(Now, doc.UpdatedAt);
        }

        [Fact]
        public async Task Timestamped_UpdateByIdAsync_NullUpdate_Throws()
        {
            TimestampedRepo repo = new TimestampedRepo(new Mock<IMongoHelper>(MockBehavior.Strict).Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => repo.UpdateByIdAsync("id1", null));
        }

        #endregion

        #region HistoriedMongoRepository

        [Fact]
        public async Task Historied_UpdateByIdAsync_PushesCappedStampWithActorAndFields()
        {
            Mock<IMongoHelper> mongo = new Mock<IMongoHelper>(MockBehavior.Strict);
            UpdateDefinition<StampHistoriedDoc> captured = null;
            mongo.Setup(m => m.UpdateByIdAsync(Collection, "id1", It.IsAny<UpdateDefinition<StampHistoriedDoc>>()))
                .Callback<string, string, UpdateDefinition<StampHistoriedDoc>>((_, _, u) => captured = u)
                .ReturnsAsync(new UpdateResult.Acknowledged(1, 1, null));

            await new HistoriedRepo(mongo.Object).UpdateByIdAsync(
                "id1", Builders<StampHistoriedDoc>.Update.Set(d => d.Name, "b"), "user-7", nameof(StampHistoriedDoc.Name));

            BsonDocument push = Render(captured)["$push"]
                [ElementName(typeof(HistoriedMongoDocument), nameof(HistoriedMongoDocument.UpdateHistory))].AsBsonDocument;
            Assert.Equal(-3, push["$slice"].AsInt32);
            BsonDocument stamp = push["$each"].AsBsonArray.Single().AsBsonDocument;
            Assert.Equal("user-7", stamp[ElementName(typeof(UpdateStamp), nameof(UpdateStamp.By))].AsString);
            Assert.Equal(Now, stamp[ElementName(typeof(UpdateStamp), nameof(UpdateStamp.At))].ToUniversalTime());
            Assert.Equal("Name", stamp[ElementName(typeof(UpdateStamp), nameof(UpdateStamp.Fields))].AsBsonArray.Single().AsString);
        }

        [Fact]
        public async Task Historied_PlainUpdateByIdAsync_RecordsSystemActor()
        {
            Mock<IMongoHelper> mongo = new Mock<IMongoHelper>(MockBehavior.Strict);
            UpdateDefinition<StampHistoriedDoc> captured = null;
            mongo.Setup(m => m.UpdateByIdAsync(Collection, "id1", It.IsAny<UpdateDefinition<StampHistoriedDoc>>()))
                .Callback<string, string, UpdateDefinition<StampHistoriedDoc>>((_, _, u) => captured = u)
                .ReturnsAsync(new UpdateResult.Acknowledged(1, 1, null));

            await new HistoriedRepo(mongo.Object).UpdateByIdAsync("id1", Builders<StampHistoriedDoc>.Update.Set(d => d.Name, "b"));

            BsonDocument stamp = Render(captured)["$push"]
                [ElementName(typeof(HistoriedMongoDocument), nameof(HistoriedMongoDocument.UpdateHistory))]["$each"]
                .AsBsonArray.Single().AsBsonDocument;
            Assert.Equal(CreatedMongoRepository<StampHistoriedDoc>.SystemActor,
                stamp[ElementName(typeof(UpdateStamp), nameof(UpdateStamp.By))].AsString);
        }

        [Fact]
        public async Task Historied_ReplaceByIdAsync_AppendsStampAndTrimsToLimit()
        {
            Mock<IMongoHelper> mongo = new Mock<IMongoHelper>(MockBehavior.Strict);
            StampHistoriedDoc doc = new StampHistoriedDoc
            {
                UpdateHistory = Enumerable.Range(1, 3)
                    .Select(i => new UpdateStamp { At = Now.AddDays(-i), By = "old-" + i })
                    .ToList(),
            };
            mongo.Setup(m => m.ReplaceByIdAsync(Collection, "id1", doc))
                .ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, null));

            await new HistoriedRepo(mongo.Object).ReplaceByIdAsync("id1", doc, "user-7", "Name");

            Assert.Equal(3, doc.UpdateHistory.Count);
            Assert.Equal("old-2", doc.UpdateHistory[0].By);
            Assert.Equal("user-7", doc.UpdateHistory.Last().By);
            Assert.Equal(Now, doc.UpdateHistory.Last().At);
            Assert.Equal(Now, doc.UpdatedAt);
        }

        [Fact]
        public async Task Historied_ReplaceByIdAsync_NullHistory_StartsList()
        {
            Mock<IMongoHelper> mongo = new Mock<IMongoHelper>(MockBehavior.Strict);
            StampHistoriedDoc doc = new StampHistoriedDoc { UpdateHistory = null };
            mongo.Setup(m => m.ReplaceByIdAsync(Collection, "id1", doc))
                .ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, null));

            await new HistoriedRepo(mongo.Object).ReplaceByIdAsync("id1", doc, null);

            UpdateStamp stamp = Assert.Single(doc.UpdateHistory);
            Assert.Equal(CreatedMongoRepository<StampHistoriedDoc>.SystemActor, stamp.By);
            Assert.Empty(stamp.Fields);
        }

        #endregion
    }
}
