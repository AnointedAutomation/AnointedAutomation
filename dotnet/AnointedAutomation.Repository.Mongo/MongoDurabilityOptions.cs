// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DriverWriteConcern = MongoDB.Driver.WriteConcern;

namespace AnointedAutomation.Repository.Mongo
{
    /// <summary>The write concern a <see cref="MongoHelper"/> asks for.</summary>
    public enum MongoWriteConcernMode
    {
        /// <summary>Leave the write concern alone: the driver or connection string default applies.</summary>
        Default = 0,
        /// <summary>Acknowledged by the primary only (<c>w: 1</c>).</summary>
        W1 = 1,
        /// <summary>Acknowledged by a majority of replica set members (<c>w: "majority"</c>). Needs a replica set or sharded cluster.</summary>
        Majority = 2
    }

    /// <summary>How <see cref="MongoHelper.RunInTransactionAsync(System.Func{IClientSessionHandle, System.Threading.Tasks.Task})"/> uses transactions.</summary>
    public enum MongoTransactionMode
    {
        /// <summary>Never start a session or transaction; the callback runs with a null session.</summary>
        Off = 0,
        /// <summary>Use a transaction only when a replica set or sharded cluster is detected; on a standalone run without one.</summary>
        Auto = 1,
        /// <summary>Always use a transaction; a standalone server makes validation throw.</summary>
        Required = 2
    }

    /// <summary>The server topology detected by the <c>hello</c> command.</summary>
    public enum MongoTopologyKind
    {
        /// <summary>Not detected yet.</summary>
        Unknown = 0,
        /// <summary>A single mongod with no replica set name. No transactions, no majority guarantee.</summary>
        Standalone = 1,
        /// <summary>A replica set member (the reply carries <c>setName</c>).</summary>
        ReplicaSet = 2,
        /// <summary>A mongos router (the reply carries <c>msg: "isdbgrid"</c>).</summary>
        Sharded = 3
    }

    /// <summary>
    /// An immutable write durability policy: a write concern mode plus an optional journal flag.
    /// </summary>
    public sealed class MongoDurabilityPolicy
    {
        /// <summary>No override at all: today's behavior.</summary>
        public static readonly MongoDurabilityPolicy Default = new MongoDurabilityPolicy(MongoWriteConcernMode.Default, null);

        /// <summary>The stricter policy meant for money collections: majority acknowledged and journaled.</summary>
        public static readonly MongoDurabilityPolicy MoneyWrites = new MongoDurabilityPolicy(MongoWriteConcernMode.Majority, true);

        /// <summary>Creates a policy.</summary>
        /// <param name="writeConcern">The write concern mode.</param>
        /// <param name="journal">True or false to set <c>j</c>; null keeps the server default.</param>
        /// <exception cref="ArgumentOutOfRangeException">The mode is not a defined value.</exception>
        public MongoDurabilityPolicy(MongoWriteConcernMode writeConcern, bool? journal)
        {
            if (!Enum.IsDefined(typeof(MongoWriteConcernMode), writeConcern))
            {
                throw new ArgumentOutOfRangeException(nameof(writeConcern), writeConcern, "Unknown write concern mode.");
            }
            WriteConcern = writeConcern;
            Journal = journal;
        }

        /// <summary>The write concern mode.</summary>
        public MongoWriteConcernMode WriteConcern { get; }

        /// <summary>The journal flag; null keeps the server default.</summary>
        public bool? Journal { get; }

        /// <summary>True when this policy changes nothing.</summary>
        public bool IsDefault => WriteConcern == MongoWriteConcernMode.Default && !Journal.HasValue;

        /// <summary>Applies the policy on top of <paramref name="current"/>, keeping what the policy leaves unset (such as wtimeout).</summary>
        /// <param name="current">The write concern the collection has now.</param>
        /// <returns>The resulting write concern.</returns>
        internal DriverWriteConcern ApplyTo(DriverWriteConcern current)
        {
            if (current == null)
            {
                throw new ArgumentNullException(nameof(current));
            }
            DriverWriteConcern result = current;
            if (WriteConcern == MongoWriteConcernMode.W1)
            {
                result = result.With(w: new DriverWriteConcern.WCount(1));
            }
            else if (WriteConcern == MongoWriteConcernMode.Majority)
            {
                result = result.With(w: DriverWriteConcern.WMode.Majority);
            }
            if (Journal.HasValue)
            {
                result = result.With(journal: Journal);
            }
            return result;
        }
    }

    /// <summary>
    /// Immutable durability settings for a <see cref="MongoHelper"/>: a write concern, a journal flag, a transaction
    /// mode, and per-collection policy overrides (for example <see cref="MongoDurabilityPolicy.MoneyWrites"/> on money
    /// collections). <see cref="Default"/> changes nothing.
    /// </summary>
    public sealed class MongoDurabilityOptions
    {
        /// <summary>No overrides: the helper behaves exactly as it does without options.</summary>
        public static readonly MongoDurabilityOptions Default =
            new MongoDurabilityOptions(MongoWriteConcernMode.Default, null, MongoTransactionMode.Off);

        private readonly Dictionary<string, MongoDurabilityPolicy> _collections;

        /// <summary>Creates options.</summary>
        /// <param name="writeConcern">The write concern for every collection without an override.</param>
        /// <param name="journal">The journal flag; null keeps the server default.</param>
        /// <param name="transactions">The transaction mode.</param>
        /// <exception cref="ArgumentOutOfRangeException">A mode is not a defined value.</exception>
        public MongoDurabilityOptions(MongoWriteConcernMode writeConcern, bool? journal, MongoTransactionMode transactions)
            : this(new MongoDurabilityPolicy(writeConcern, journal), ValidTransactions(transactions),
                new Dictionary<string, MongoDurabilityPolicy>(StringComparer.Ordinal))
        {
        }

        private MongoDurabilityOptions(MongoDurabilityPolicy global, MongoTransactionMode transactions, Dictionary<string, MongoDurabilityPolicy> collections)
        {
            Global = global;
            Transactions = transactions;
            _collections = collections;
        }

        private static MongoTransactionMode ValidTransactions(MongoTransactionMode transactions)
        {
            if (!Enum.IsDefined(typeof(MongoTransactionMode), transactions))
            {
                throw new ArgumentOutOfRangeException(nameof(transactions), transactions, "Unknown transaction mode.");
            }
            return transactions;
        }

        /// <summary>The policy for collections without an override.</summary>
        public MongoDurabilityPolicy Global { get; }

        /// <summary>The write concern for collections without an override.</summary>
        public MongoWriteConcernMode WriteConcern => Global.WriteConcern;

        /// <summary>The journal flag for collections without an override.</summary>
        public bool? Journal => Global.Journal;

        /// <summary>The transaction mode.</summary>
        public MongoTransactionMode Transactions { get; }

        /// <summary>The per-collection overrides, keyed by exact (ordinal) collection name.</summary>
        public IReadOnlyDictionary<string, MongoDurabilityPolicy> CollectionPolicies =>
            new ReadOnlyDictionary<string, MongoDurabilityPolicy>(_collections);

        /// <summary>True when nothing is overridden anywhere.</summary>
        public bool IsDefault => Global.IsDefault && Transactions == MongoTransactionMode.Off && _collections.Count == 0;

        /// <summary>True when any policy asks for majority acknowledgement.</summary>
        internal bool UsesMajority
        {
            get
            {
                if (Global.WriteConcern == MongoWriteConcernMode.Majority)
                {
                    return true;
                }
                foreach (MongoDurabilityPolicy policy in _collections.Values)
                {
                    if (policy.WriteConcern == MongoWriteConcernMode.Majority)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>True when the topology must be checked before use (transactions on, or majority anywhere).</summary>
        internal bool NeedsTopology => Transactions != MongoTransactionMode.Off || UsesMajority;

        /// <summary>Returns new options where the named collections use <paramref name="policy"/> instead of the global one.</summary>
        /// <param name="collectionNames">Exact collection names (ordinal match).</param>
        /// <param name="policy">The policy, for example <see cref="MongoDurabilityPolicy.MoneyWrites"/>.</param>
        /// <returns>A new options instance; this one is unchanged.</returns>
        /// <exception cref="ArgumentNullException">An argument is null.</exception>
        /// <exception cref="ArgumentException">A name is null or blank.</exception>
        public MongoDurabilityOptions ForCollections(IEnumerable<string> collectionNames, MongoDurabilityPolicy policy)
        {
            if (collectionNames == null)
            {
                throw new ArgumentNullException(nameof(collectionNames));
            }
            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }
            Dictionary<string, MongoDurabilityPolicy> copy = new Dictionary<string, MongoDurabilityPolicy>(_collections, StringComparer.Ordinal);
            foreach (string name in collectionNames)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("Collection names must not be null or blank.", nameof(collectionNames));
                }
                copy[name] = policy;
            }
            return new MongoDurabilityOptions(Global, Transactions, copy);
        }

        /// <summary>The policy that applies to <paramref name="collectionName"/>.</summary>
        /// <param name="collectionName">The collection name.</param>
        /// <returns>The override when one is set, else <see cref="Global"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collectionName"/> is null.</exception>
        public MongoDurabilityPolicy PolicyFor(string collectionName)
        {
            if (collectionName == null)
            {
                throw new ArgumentNullException(nameof(collectionName));
            }
            if (_collections.TryGetValue(collectionName, out MongoDurabilityPolicy policy))
            {
                return policy;
            }
            return Global;
        }
    }

    /// <summary>
    /// Immutable read settings for a <see cref="MongoHelper"/>. Separate from durability: a read preference picks which
    /// member serves reads and says nothing about how safely writes are stored.
    /// </summary>
    public sealed class MongoReadOptions
    {
        /// <summary>Creates read options.</summary>
        /// <param name="readPreference">The read preference applied to every collection handle.</param>
        /// <exception cref="ArgumentNullException"><paramref name="readPreference"/> is null.</exception>
        public MongoReadOptions(ReadPreference readPreference)
        {
            if (readPreference == null)
            {
                throw new ArgumentNullException(nameof(readPreference));
            }
            ReadPreference = readPreference;
        }

        /// <summary>The read preference.</summary>
        public ReadPreference ReadPreference { get; }
    }
}
