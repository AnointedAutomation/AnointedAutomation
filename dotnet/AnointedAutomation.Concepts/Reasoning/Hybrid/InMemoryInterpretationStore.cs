// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AnointedAutomation.Concepts.Reasoning.Hybrid
{
    /// <summary>
    /// A thread-safe, process-local <see cref="IInterpretationStore"/>. Entries are indexed per owner,
    /// so a read never touches another owner's entries.
    /// </summary>
    public sealed class InMemoryInterpretationStore : IInterpretationStore
    {
        private readonly object gate = new object();
        private readonly Dictionary<string, List<InterpretationEntry>> byOwner = new Dictionary<string, List<InterpretationEntry>>(StringComparer.Ordinal);
        private readonly Dictionary<string, string> ownerById = new Dictionary<string, string>(StringComparer.Ordinal);

        /// <inheritdoc/>
        public Task Add(InterpretationEntry entry, CancellationToken cancellationToken)
        {
            ReasoningGuard.NotNull(entry, nameof(entry));
            cancellationToken.ThrowIfCancellationRequested();
            lock (gate)
            {
                if (ownerById.ContainsKey(entry.Id))
                {
                    throw new InvalidOperationException("An entry with id '" + entry.Id + "' already exists.");
                }

                if (!byOwner.TryGetValue(entry.Owner.OwnerId, out List<InterpretationEntry> list))
                {
                    list = new List<InterpretationEntry>();
                    byOwner.Add(entry.Owner.OwnerId, list);
                }

                list.Add(entry);
                ownerById.Add(entry.Id, entry.Owner.OwnerId);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task<InterpretationEntry> Find(OwnerScope owner, string entryId, CancellationToken cancellationToken)
        {
            ReasoningGuard.NotNull(owner, nameof(owner));
            ReasoningGuard.NotBlank(entryId, nameof(entryId));
            cancellationToken.ThrowIfCancellationRequested();
            lock (gate)
            {
                return Task.FromResult(FindLocked(owner, entryId, out int _));
            }
        }

        /// <inheritdoc/>
        public Task<InterpretationEntry> FindByQuery(OwnerScope owner, string queryKey, string model, CancellationToken cancellationToken)
        {
            ReasoningGuard.NotNull(owner, nameof(owner));
            ReasoningGuard.NotBlank(queryKey, nameof(queryKey));
            ReasoningGuard.NotBlank(model, nameof(model));
            cancellationToken.ThrowIfCancellationRequested();
            lock (gate)
            {
                if (byOwner.TryGetValue(owner.OwnerId, out List<InterpretationEntry> list))
                {
                    foreach (InterpretationEntry entry in list)
                    {
                        if (string.Equals(entry.QueryKey, queryKey, StringComparison.Ordinal)
                            && string.Equals(entry.Provenance.Model, model, StringComparison.Ordinal))
                        {
                            return Task.FromResult(entry);
                        }
                    }
                }

                return Task.FromResult<InterpretationEntry>(null);
            }
        }

        /// <inheritdoc/>
        public Task<InterpretationPage> List(OwnerScope owner, int skip, int limit, CancellationToken cancellationToken)
        {
            ReasoningGuard.NotNull(owner, nameof(owner));
            if (skip < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(skip), skip, "Skip must be zero or more.");
            }

            if (limit < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(limit), limit, "Limit must be one or more.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            lock (gate)
            {
                if (!byOwner.TryGetValue(owner.OwnerId, out List<InterpretationEntry> list))
                {
                    return Task.FromResult(new InterpretationPage(new List<InterpretationEntry>(), 0));
                }

                int start = Math.Min(skip, list.Count);
                int count = Math.Min(limit, list.Count - start);
                return Task.FromResult(new InterpretationPage(list.GetRange(start, count), list.Count));
            }
        }

        /// <inheritdoc/>
        public Task<bool> Replace(InterpretationEntry entry, InterpretationStatus expected, CancellationToken cancellationToken)
        {
            ReasoningGuard.NotNull(entry, nameof(entry));
            cancellationToken.ThrowIfCancellationRequested();
            lock (gate)
            {
                InterpretationEntry stored = FindLocked(entry.Owner, entry.Id, out int index);
                if (stored == null)
                {
                    throw new InvalidOperationException("No entry '" + entry.Id + "' exists for this owner.");
                }

                if (stored.Status != expected)
                {
                    return Task.FromResult(false);
                }

                byOwner[entry.Owner.OwnerId][index] = entry;
                return Task.FromResult(true);
            }
        }

        private InterpretationEntry FindLocked(OwnerScope owner, string entryId, out int index)
        {
            index = -1;
            if (!ownerById.TryGetValue(entryId, out string ownerId) || !string.Equals(ownerId, owner.OwnerId, StringComparison.Ordinal))
            {
                return null;
            }

            List<InterpretationEntry> list = byOwner[ownerId];
            for (int i = 0; i < list.Count; i++)
            {
                if (string.Equals(list[i].Id, entryId, StringComparison.Ordinal))
                {
                    index = i;
                    return list[i];
                }
            }

            throw new InvalidOperationException("The id index points at a missing entry '" + entryId + "'.");
        }
    }
}
