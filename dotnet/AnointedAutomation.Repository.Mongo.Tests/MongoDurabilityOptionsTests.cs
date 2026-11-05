// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using AnointedAutomation.Repository.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;
using MongoDB.Driver.Core.Connections;
using MongoDB.Driver.Core.Servers;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AnointedAutomation.Repository.Mongo.Tests
{
    public class MongoDurabilityOptionsTests
    {
        private sealed class Rig
        {
            public Mock<IMongoDatabase> Db = new Mock<IMongoDatabase>();
            public Mock<IMongoCollection<MongoTestDoc>> Raw = new Mock<IMongoCollection<MongoTestDoc>>();
            public Mock<IMongoCollection<MongoTestDoc>> Applied = new Mock<IMongoCollection<MongoTestDoc>>();
            public List<WriteConcern> Concerns = new List<WriteConcern>();
            public List<ReadPreference> Reads = new List<ReadPreference>();
            public int HelloCalls;
        }

        private static Rig Build(BsonDocument hello)
        {
            Rig rig = new Rig();
            rig.Db.Setup(d => d.GetCollection<MongoTestDoc>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(rig.Raw.Object);
            rig.Db.SetupGet(d => d.Settings).Returns(new MongoDatabaseSettings { WriteConcern = WriteConcern.Acknowledged });
            rig.Raw.SetupGet(c => c.Settings).Returns(new MongoCollectionSettings { WriteConcern = WriteConcern.Acknowledged });
            rig.Raw.Setup(c => c.WithWriteConcern(It.IsAny<WriteConcern>()))
                .Callback((WriteConcern w) => rig.Concerns.Add(w))
                .Returns(rig.Applied.Object);
            rig.Raw.Setup(c => c.WithReadPreference(It.IsAny<ReadPreference>()))
                .Callback((ReadPreference r) => rig.Reads.Add(r))
                .Returns(rig.Applied.Object);
            rig.Applied.Setup(c => c.WithReadPreference(It.IsAny<ReadPreference>()))
                .Callback((ReadPreference r) => rig.Reads.Add(r))
                .Returns(rig.Applied.Object);
            rig.Db.Setup(d => d.RunCommandAsync(It.IsAny<Command<BsonDocument>>(), It.IsAny<ReadPreference>(), It.IsAny<CancellationToken>()))
                .Callback(() => rig.HelloCalls++)
                .ReturnsAsync(hello);
            return rig;
        }

        private static BsonDocument Standalone() => new BsonDocument { { "isWritablePrimary", true }, { "ok", 1 } };
        private static BsonDocument ReplicaSet() => new BsonDocument { { "isWritablePrimary", true }, { "setName", "rs0" }, { "ok", 1 } };

        private static MongoHelper Helper(Rig rig, MongoHelper helper)
        {
            helper.database = rig.Db.Object;
            helper.dbName = "db";
            return helper;
        }

        [Fact]
        public async Task Defaults_ApplyNothingAndRunNoHello()
        {
            Rig rig = Build(Standalone());
            MongoHelper helper = Helper(rig, new MongoHelper());

            await helper.CreateDocumentAsync("c", new MongoTestDoc { _id = "1" });

            rig.Raw.Verify(c => c.WithWriteConcern(It.IsAny<WriteConcern>()), Times.Never);
            rig.Raw.Verify(c => c.WithReadPreference(It.IsAny<ReadPreference>()), Times.Never);
            rig.Raw.Verify(c => c.InsertOneAsync(It.IsAny<MongoTestDoc>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(0, rig.HelloCalls);
            Assert.True(helper.Durability.IsDefault);
            Assert.Null(helper.ReadOptions);
        }

        [Fact]
        public async Task ExplicitDefaultOptions_ApplyNothing()
        {
            Rig rig = Build(Standalone());
            MongoHelper helper = Helper(rig, new MongoHelper(MongoDurabilityOptions.Default));

            await helper.CreateDocumentAsync("c", new MongoTestDoc { _id = "1" });

            rig.Raw.Verify(c => c.WithWriteConcern(It.IsAny<WriteConcern>()), Times.Never);
            Assert.Equal(0, rig.HelloCalls);
        }

        [Fact]
        public async Task W1WithJournal_IsApplied()
        {
            Rig rig = Build(Standalone());
            MongoHelper helper = Helper(rig, new MongoHelper(new MongoDurabilityOptions(MongoWriteConcernMode.W1, true, MongoTransactionMode.Off)));

            await helper.CreateDocumentAsync("c", new MongoTestDoc { _id = "1" });

            WriteConcern applied = Assert.Single(rig.Concerns);
            Assert.Equal(new WriteConcern.WCount(1), applied.W);
            Assert.True(applied.Journal);
            rig.Applied.Verify(c => c.InsertOneAsync(It.IsAny<MongoTestDoc>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(0, rig.HelloCalls);
        }

        [Fact]
        public async Task Majority_OnReplicaSet_IsAppliedAfterOneHello()
        {
            Rig rig = Build(ReplicaSet());
            MongoHelper helper = Helper(rig, new MongoHelper(new MongoDurabilityOptions(MongoWriteConcernMode.Majority, null, MongoTransactionMode.Off)));

            await helper.CreateDocumentAsync("c", new MongoTestDoc { _id = "1" });
            await helper.CreateDocumentAsync("c", new MongoTestDoc { _id = "2" });

            Assert.Equal(2, rig.Concerns.Count);
            Assert.All(rig.Concerns, w => Assert.Equal(WriteConcern.WMode.Majority, w.W));
            Assert.All(rig.Concerns, w => Assert.Null(w.Journal));
            Assert.Equal(1, rig.HelloCalls);
            Assert.Equal(MongoTopologyKind.ReplicaSet, helper.DetectedTopology);
        }

        [Fact]
        public async Task ReadOptions_AreAppliedSeparately()
        {
            Rig rig = Build(Standalone());
            MongoHelper helper = Helper(rig, new MongoHelper(MongoDurabilityOptions.Default, new MongoReadOptions(ReadPreference.SecondaryPreferred)));
            rig.Applied.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<MongoTestDoc>>(), It.IsAny<FindOptions<MongoTestDoc, MongoTestDoc>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("reached"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => helper.GetSingleAsync("c", Builders<MongoTestDoc>.Filter.Empty));

            Assert.Equal(ReadPreference.SecondaryPreferred, Assert.Single(rig.Reads));
            rig.Raw.Verify(c => c.WithWriteConcern(It.IsAny<WriteConcern>()), Times.Never);
        }

        [Fact]
        public async Task MoneyPolicy_AppliesOnlyToListedCollections()
        {
            Rig rig = Build(ReplicaSet());
            MongoDurabilityOptions options = MongoDurabilityOptions.Default
                .ForCollections(new[] { "payments" }, MongoDurabilityPolicy.MoneyWrites);
            MongoHelper helper = Helper(rig, new MongoHelper(options));

            await helper.CreateDocumentAsync("notes", new MongoTestDoc { _id = "1" });
            Assert.Empty(rig.Concerns);

            await helper.CreateDocumentAsync("payments", new MongoTestDoc { _id = "2" });
            WriteConcern applied = Assert.Single(rig.Concerns);
            Assert.Equal(WriteConcern.WMode.Majority, applied.W);
            Assert.True(applied.Journal);
            Assert.True(MongoDurabilityOptions.Default.IsDefault);
        }

        [Fact]
        public async Task Required_OnStandalone_Throws()
        {
            Rig rig = Build(Standalone());
            MongoHelper helper = Helper(rig, new MongoHelper(new MongoDurabilityOptions(MongoWriteConcernMode.Default, null, MongoTransactionMode.Required)));

            InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() => helper.ValidateAsync());
            Assert.Contains("standalone", ex.Message);
            await Assert.ThrowsAsync<InvalidOperationException>(() => helper.RunInTransactionAsync(_ => Task.CompletedTask));
        }

        [Fact]
        public async Task Majority_OnStandalone_ThrowsOnFirstWrite()
        {
            Rig rig = Build(Standalone());
            MongoHelper helper = Helper(rig, new MongoHelper(new MongoDurabilityOptions(MongoWriteConcernMode.Majority, null, MongoTransactionMode.Off)));

            await Assert.ThrowsAsync<InvalidOperationException>(() => helper.CreateDocumentAsync("c", new MongoTestDoc { _id = "1" }));

            rig.Raw.Verify(c => c.InsertOneAsync(It.IsAny<MongoTestDoc>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()), Times.Never);
            rig.Applied.Verify(c => c.InsertOneAsync(It.IsAny<MongoTestDoc>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task MoneyPolicy_OnStandalone_Throws()
        {
            Rig rig = Build(Standalone());
            MongoHelper helper = Helper(rig, new MongoHelper(MongoDurabilityOptions.Default.ForCollections(new[] { "payments" }, MongoDurabilityPolicy.MoneyWrites)));

            await Assert.ThrowsAsync<InvalidOperationException>(() => helper.ValidateAsync());
        }

        [Fact]
        public async Task Auto_OnStandalone_RunsWithoutSession()
        {
            Rig rig = Build(Standalone());
            MongoHelper helper = Helper(rig, new MongoHelper(new MongoDurabilityOptions(MongoWriteConcernMode.Default, null, MongoTransactionMode.Auto)));
            IClientSessionHandle seen = new Mock<IClientSessionHandle>().Object;

            int result = await helper.RunInTransactionAsync(session =>
            {
                seen = session;
                return Task.FromResult(7);
            });

            Assert.Equal(7, result);
            Assert.Null(seen);
            Assert.Equal(MongoTransactionMode.Off, helper.EffectiveTransactionMode);
            rig.Db.Verify(d => d.Client, Times.Never);
        }

        [Fact]
        public async Task Off_RunsWithoutSessionOrHello()
        {
            Rig rig = Build(ReplicaSet());
            MongoHelper helper = Helper(rig, new MongoHelper());
            bool ran = false;

            await helper.RunInTransactionAsync(session =>
            {
                ran = session == null;
                return Task.CompletedTask;
            });

            Assert.True(ran);
            Assert.Equal(0, rig.HelloCalls);
        }

        [Fact]
        public async Task Required_OnReplicaSet_UsesSessionTransaction()
        {
            Rig rig = Build(ReplicaSet());
            Mock<IClientSessionHandle> session = new Mock<IClientSessionHandle>();
            session.Setup(s => s.WithTransactionAsync(It.IsAny<Func<IClientSessionHandle, CancellationToken, Task<int>>>(), It.IsAny<TransactionOptions>(), It.IsAny<CancellationToken>()))
                .Returns((Func<IClientSessionHandle, CancellationToken, Task<int>> cb, TransactionOptions _, CancellationToken ct) => cb(session.Object, ct));
            Mock<IMongoClient> client = new Mock<IMongoClient>();
            client.Setup(c => c.StartSessionAsync(It.IsAny<ClientSessionOptions>(), It.IsAny<CancellationToken>())).ReturnsAsync(session.Object);
            rig.Db.SetupGet(d => d.Client).Returns(client.Object);
            MongoHelper helper = Helper(rig, new MongoHelper(new MongoDurabilityOptions(MongoWriteConcernMode.Default, null, MongoTransactionMode.Required)));

            IClientSessionHandle seen = null;
            int result = await helper.RunInTransactionAsync(s =>
            {
                seen = s;
                return Task.FromResult(3);
            });

            Assert.Equal(3, result);
            Assert.Same(session.Object, seen);
            session.Verify(s => s.Dispose(), Times.Once);
        }

        [Fact]
        public async Task Transaction_UsesStrictestPolicyWriteConcern()
        {
            Rig rig = Build(ReplicaSet());
            TransactionOptions captured = null;
            Mock<IClientSessionHandle> session = new Mock<IClientSessionHandle>();
            session.Setup(s => s.WithTransactionAsync(It.IsAny<Func<IClientSessionHandle, CancellationToken, Task<int>>>(), It.IsAny<TransactionOptions>(), It.IsAny<CancellationToken>()))
                .Returns((Func<IClientSessionHandle, CancellationToken, Task<int>> cb, TransactionOptions o, CancellationToken ct) =>
                {
                    captured = o;
                    return cb(session.Object, ct);
                });
            Mock<IMongoClient> client = new Mock<IMongoClient>();
            client.Setup(c => c.StartSessionAsync(It.IsAny<ClientSessionOptions>(), It.IsAny<CancellationToken>())).ReturnsAsync(session.Object);
            rig.Db.SetupGet(d => d.Client).Returns(client.Object);
            MongoDurabilityOptions options = new MongoDurabilityOptions(MongoWriteConcernMode.W1, null, MongoTransactionMode.Required)
                .ForCollections(new[] { "payments" }, MongoDurabilityPolicy.MoneyWrites);
            MongoHelper helper = Helper(rig, new MongoHelper(options));

            await helper.RunInTransactionAsync(_ => Task.FromResult(1));

            Assert.NotNull(captured);
            Assert.Equal(WriteConcern.WMode.Majority, captured.WriteConcern.W);
            Assert.True(captured.WriteConcern.Journal);
            Assert.Equal(ReadConcern.Majority, captured.ReadConcern);
            Assert.Equal(ReadPreference.Primary, captured.ReadPreference);
        }

        [Fact]
        public void TransactionOptions_KeepsConfiguredWTimeout()
        {
            Rig rig = Build(ReplicaSet());
            rig.Db.SetupGet(d => d.Settings).Returns(new MongoDatabaseSettings { WriteConcern = WriteConcern.Acknowledged.With(wTimeout: TimeSpan.FromSeconds(5)) });
            MongoHelper helper = Helper(rig, new MongoHelper(new MongoDurabilityOptions(MongoWriteConcernMode.Majority, true, MongoTransactionMode.Required)));

            TransactionOptions options = helper.BuildTransactionOptions();

            Assert.Equal(WriteConcern.WMode.Majority, options.WriteConcern.W);
            Assert.True(options.WriteConcern.Journal);
            Assert.Equal(TimeSpan.FromSeconds(5), options.WriteConcern.WTimeout);
        }

        [Fact]
        public void TransactionOptions_NullWhenNoPolicy()
        {
            Rig rig = Build(ReplicaSet());
            MongoHelper helper = Helper(rig, new MongoHelper(new MongoDurabilityOptions(MongoWriteConcernMode.Default, null, MongoTransactionMode.Required)));
            Assert.Null(helper.BuildTransactionOptions());

            MongoHelper w1 = Helper(rig, new MongoHelper(new MongoDurabilityOptions(MongoWriteConcernMode.W1, false, MongoTransactionMode.Required)));
            TransactionOptions options = w1.BuildTransactionOptions();
            Assert.Equal(new WriteConcern.WCount(1), options.WriteConcern.W);
            Assert.False(options.WriteConcern.Journal);
            Assert.Null(options.ReadConcern);
        }

        [Fact]
        public async Task WriteConcernError_Propagates()
        {
            Rig rig = Build(Standalone());
            MongoHelper helper = Helper(rig, new MongoHelper(new MongoDurabilityOptions(MongoWriteConcernMode.W1, true, MongoTransactionMode.Off)));
            ConnectionId connection = new ConnectionId(new ServerId(new ClusterId(), new DnsEndPoint("localhost", 27017)));
            MongoWriteConcernException failure = new MongoWriteConcernException(connection, "waiting for replication timed out",
                new WriteConcernResult(new BsonDocument { { "ok", 1 }, { "writeConcernError", new BsonDocument("code", 64) } }));
            rig.Applied.Setup(c => c.InsertOneAsync(It.IsAny<MongoTestDoc>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(failure);

            MongoWriteConcernException thrown = await Assert.ThrowsAsync<MongoWriteConcernException>(
                () => helper.CreateDocumentAsync("c", new MongoTestDoc { _id = "1" }));
            Assert.Same(failure, thrown);
        }

        [Fact]
        public void Options_AreImmutableAndValidated()
        {
            MongoDurabilityOptions baseOptions = new MongoDurabilityOptions(MongoWriteConcernMode.W1, null, MongoTransactionMode.Off);
            MongoDurabilityOptions money = baseOptions.ForCollections(new[] { "ledger" }, MongoDurabilityPolicy.MoneyWrites);

            Assert.Empty(baseOptions.CollectionPolicies);
            Assert.Same(MongoDurabilityPolicy.MoneyWrites, money.PolicyFor("ledger"));
            Assert.Same(money.Global, money.PolicyFor("Ledger"));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MongoDurabilityOptions((MongoWriteConcernMode)99, null, MongoTransactionMode.Off));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MongoDurabilityOptions(MongoWriteConcernMode.W1, null, (MongoTransactionMode)99));
            Assert.Throws<ArgumentException>(() => baseOptions.ForCollections(new[] { " " }, MongoDurabilityPolicy.MoneyWrites));
            Assert.Throws<ArgumentNullException>(() => new MongoReadOptions(null));
            Assert.Throws<ArgumentNullException>(() => new MongoHelper((MongoDurabilityOptions)null));
        }

        [Fact]
        public void Factory_PassesOptionsToHelpers()
        {
            MongoDurabilityOptions options = new MongoDurabilityOptions(MongoWriteConcernMode.W1, null, MongoTransactionMode.Off);
            MongoReadOptions read = new MongoReadOptions(ReadPreference.Primary);
            MongoHelperFactory factory = new MongoHelperFactory(options, read);

            MongoHelper helper = (MongoHelper)factory.Create("db", "mongodb://localhost:27017");

            Assert.Same(options, helper.Durability);
            Assert.Same(read, helper.ReadOptions);
            MongoHelper plain = (MongoHelper)new MongoHelperFactory().Create("db", "mongodb://localhost:27017");
            Assert.True(plain.Durability.IsDefault);
            Assert.Null(plain.ReadOptions);
        }
    }
}
