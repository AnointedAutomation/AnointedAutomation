// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

using System;
using System.Collections.Generic;
using System.Text;

namespace AnointedAutomation.Repository.Mongo
{
    /// <summary>
    /// One field of an index and its direction.
    /// </summary>
    public sealed class MongoIndexKey
    {
        /// <summary>
        /// Initializes an index key.
        /// </summary>
        /// <param name="field">The BSON field name to index.</param>
        /// <param name="descending">True to index descending; false for ascending.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="field"/> is null or whitespace.</exception>
        public MongoIndexKey(string field, bool descending = false)
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                throw new ArgumentException("Index field name is required.", nameof(field));
            }
            Field = field;
            Descending = descending;
        }

        /// <summary>
        /// Gets the BSON field name to index.
        /// </summary>
        public string Field { get; }

        /// <summary>
        /// Gets whether the field is indexed descending (true) or ascending (false).
        /// </summary>
        public bool Descending { get; }
    }

    /// <summary>
    /// Declarative specification of a MongoDB index: one or more keys with direction, optional
    /// uniqueness, optional TTL (expire-after on a date field), and an optional explicit name.
    /// When no name is given, a deterministic name is derived from the keys and options so that
    /// ensuring the same spec repeatedly is idempotent.
    /// </summary>
    public sealed class MongoIndexSpec
    {
        /// <summary>
        /// Initializes a single-field index spec.
        /// </summary>
        /// <param name="field">The BSON field name to index.</param>
        /// <param name="descending">True to index descending; false for ascending.</param>
        /// <param name="unique">True to enforce uniqueness on the indexed value.</param>
        /// <param name="expireAfter">TTL for expire-after on a date field, or null for no TTL.</param>
        /// <param name="name">An explicit index name, or null to derive a deterministic one.</param>
        public MongoIndexSpec(string field, bool descending = false, bool unique = false, TimeSpan? expireAfter = null, string name = null)
            : this(new List<MongoIndexKey> { new MongoIndexKey(field, descending) }, unique, expireAfter, name)
        {
        }

        /// <summary>
        /// Initializes a composite (or single) index spec from explicit keys.
        /// </summary>
        /// <param name="keys">The index keys, in order.</param>
        /// <param name="unique">True to enforce uniqueness on the indexed values.</param>
        /// <param name="expireAfter">TTL for expire-after on a date field, or null for no TTL.</param>
        /// <param name="name">An explicit index name, or null to derive a deterministic one.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="keys"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="keys"/> is empty or contains a null key.</exception>
        public MongoIndexSpec(IReadOnlyList<MongoIndexKey> keys, bool unique = false, TimeSpan? expireAfter = null, string name = null)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            if (keys.Count == 0)
            {
                throw new ArgumentException("At least one index key is required.", nameof(keys));
            }
            foreach (MongoIndexKey key in keys)
            {
                if (key == null)
                {
                    throw new ArgumentException("Index keys must not contain null.", nameof(keys));
                }
            }
            Keys = keys;
            Unique = unique;
            ExpireAfter = expireAfter;
            Name = name;
        }

        /// <summary>
        /// Gets the index keys, in order.
        /// </summary>
        public IReadOnlyList<MongoIndexKey> Keys { get; }

        /// <summary>
        /// Gets whether the index enforces uniqueness.
        /// </summary>
        public bool Unique { get; }

        /// <summary>
        /// Gets the TTL for expire-after on a date field, or null for no TTL.
        /// </summary>
        public TimeSpan? ExpireAfter { get; }

        /// <summary>
        /// Gets the explicit index name, or null when the deterministic name should be used.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Resolves the index name to create: the explicit <see cref="Name"/> when set, otherwise a
        /// deterministic name derived from the keys and options (for example
        /// <c>CreatedAt_-1_ttl86400</c> or <c>Email_1_unique</c>). The same spec always resolves to
        /// the same name, which makes repeated ensure calls idempotent.
        /// </summary>
        /// <returns>The index name to use.</returns>
        public string ResolveName()
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
                return Name;
            }

            StringBuilder builder = new StringBuilder();
            foreach (MongoIndexKey key in Keys)
            {
                if (builder.Length > 0)
                {
                    builder.Append('_');
                }
                builder.Append(key.Field).Append('_').Append(key.Descending ? "-1" : "1");
            }
            if (Unique)
            {
                builder.Append("_unique");
            }
            if (ExpireAfter.HasValue)
            {
                builder.Append("_ttl").Append((long)ExpireAfter.Value.TotalSeconds);
            }
            return builder.ToString();
        }
    }
}
