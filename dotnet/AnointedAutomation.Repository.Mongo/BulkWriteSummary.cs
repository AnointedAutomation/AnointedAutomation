// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using MongoDB.Driver;

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
}
