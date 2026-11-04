// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace AnointedAutomation.Repository.Mongo
{
    /// <summary>The summed counts of every batch sent by <see cref="IMongoHelper.BulkWriteAsync{T}"/>.</summary>
    public sealed record BulkWriteSummary(int Batches, long Inserted, long Matched, long Modified, long Upserted, long Deleted)
    {
        /// <summary>No batches sent.</summary>
        public static BulkWriteSummary Empty { get; } = new BulkWriteSummary(0, 0, 0, 0, 0, 0);

        /// <summary>Adds one acknowledged batch result. An unacknowledged write (w:0) counts the batch but no documents.</summary>
        public BulkWriteSummary Add<T>(BulkWriteResult<T> result)
        {
            if (result == null)
            {
                throw new System.ArgumentNullException(nameof(result));
            }
            if (!result.IsAcknowledged)
            {
                return this with { Batches = Batches + 1 };
            }
            return new BulkWriteSummary(Batches + 1, Inserted + result.InsertedCount, Matched + result.MatchedCount,
                Modified + result.ModifiedCount, Upserted + result.Upserts.Count, Deleted + result.DeletedCount);
        }
    }

    /// <summary>One model that failed. <see cref="Index"/> is its position in the whole list passed to BulkWriteAsync
    /// (-1 for a write concern error, which belongs to a batch rather than one model).</summary>
    public sealed record BulkWriteFailure(int Index, int Code, string Message);

    /// <summary>Thrown by <see cref="IMongoHelper.BulkWriteAsync{T}"/> after the batches ran (unordered) or at the first
    /// failing batch (ordered). <see cref="Summary"/> holds what WAS applied; <see cref="Failures"/> lists what was not.</summary>
    public sealed class BulkWriteFailedException : Exception
    {
        public BulkWriteFailedException(string collectionName, BulkWriteSummary summary, IReadOnlyList<BulkWriteFailure> failures)
            : base($"Bulk write to '{collectionName}' had {failures.Count} failed model(s) after {summary.Batches} batch(es)")
        {
            CollectionName = collectionName;
            Summary = summary;
            Failures = failures;
        }

        public string CollectionName { get; }
        public BulkWriteSummary Summary { get; }
        public IReadOnlyList<BulkWriteFailure> Failures { get; }
    }
}
