// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System.Threading;
using System.Threading.Tasks;

namespace AnointedAutomation.Concepts.Reasoning.Hybrid
{
    /// <summary>
    /// Storage for the <see cref="InterpretationLedger"/>. Every read is scoped to one owner; there is
    /// deliberately no way to list entries across owners. Core ships
    /// <see cref="InMemoryInterpretationStore"/>; persistent stores live in consumers.
    /// </summary>
    public interface IInterpretationStore
    {
        /// <summary>
        /// Adds a new entry. Throws when an entry with the same id exists.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="cancellationToken">Cancels the write.</param>
        /// <returns>A task that completes when stored.</returns>
        Task Add(InterpretationEntry entry, CancellationToken cancellationToken);

        /// <summary>
        /// Finds one of <paramref name="owner"/>'s entries by id.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="entryId">The entry id.</param>
        /// <param name="cancellationToken">Cancels the read.</param>
        /// <returns>The entry, or null when the owner has no entry with that id (another owner's entry is never returned).</returns>
        Task<InterpretationEntry> Find(OwnerScope owner, string entryId, CancellationToken cancellationToken);

        /// <summary>
        /// Finds <paramref name="owner"/>'s entry for a query hash and model, for the cache.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="queryKey">The query hash.</param>
        /// <param name="model">The model name.</param>
        /// <param name="cancellationToken">Cancels the read.</param>
        /// <returns>The entry, or null when the owner never asked that query of that model.</returns>
        Task<InterpretationEntry> FindByQuery(OwnerScope owner, string queryKey, string model, CancellationToken cancellationToken);

        /// <summary>
        /// Lists one page of <paramref name="owner"/>'s entries, oldest first.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="skip">Entries to skip; zero or more.</param>
        /// <param name="limit">Page size; one or more.</param>
        /// <param name="cancellationToken">Cancels the read.</param>
        /// <returns>The page and the owner's total.</returns>
        Task<InterpretationPage> List(OwnerScope owner, int skip, int limit, CancellationToken cancellationToken);

        /// <summary>
        /// Replaces an entry only if its stored status is still <paramref name="expected"/>, so two
        /// concurrent reviews cannot both win.
        /// </summary>
        /// <param name="entry">The updated entry.</param>
        /// <param name="expected">The status the stored entry must still have.</param>
        /// <param name="cancellationToken">Cancels the write.</param>
        /// <returns>True when replaced; false when the stored status changed.</returns>
        Task<bool> Replace(InterpretationEntry entry, InterpretationStatus expected, CancellationToken cancellationToken);
    }
}
