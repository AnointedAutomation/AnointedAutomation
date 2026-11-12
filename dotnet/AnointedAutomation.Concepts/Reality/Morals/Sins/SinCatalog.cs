// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System;
using System.Collections.Generic;
using AnointedAutomation.Enums;

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// The catalog of sins, each grounded in Scripture. Built once; lookups by id, category and
    /// coarse <see cref="Sin"/> are precomputed dictionaries. Engine agnostic: consequences attach
    /// through <see cref="SinConsequenceRegistry{TTarget}"/>.
    /// </summary>
    public static partial class SinCatalog
    {
        private static readonly IReadOnlyList<SinEntry> AllEntries;
        private static readonly Dictionary<SinId, SinEntry> ById;
        private static readonly Dictionary<SinCategory, IReadOnlyList<SinEntry>> ByCategory;
        private static readonly Dictionary<Sin, IReadOnlyList<SinEntry>> ByCoarse;
        private static readonly Dictionary<SinId, CataloguedSin> Concepts;
        private static readonly IReadOnlyList<SinEntry> Empty = Array.AsReadOnly(new SinEntry[0]);

        static SinCatalog()
        {
            List<SinEntry> entries = BuildEntries();
            ById = new Dictionary<SinId, SinEntry>(entries.Count);
            Concepts = new Dictionary<SinId, CataloguedSin>(entries.Count);
            Dictionary<SinCategory, List<SinEntry>> cat = new Dictionary<SinCategory, List<SinEntry>>();
            Dictionary<Sin, List<SinEntry>> coarse = new Dictionary<Sin, List<SinEntry>>();
            foreach (SinEntry entry in entries)
            {
                // Add throws on a duplicate id, so a bad edit fails at type load, not silently.
                ById.Add(entry.Id, entry);
                if (entry.IsOmitted)
                {
                    continue;
                }

                if (!cat.TryGetValue(entry.Category, out List<SinEntry> inCat))
                {
                    inCat = new List<SinEntry>();
                    cat.Add(entry.Category, inCat);
                }

                inCat.Add(entry);
                Concepts.Add(entry.Id, new CataloguedSin(entry));
                if (entry.CoarseSin.HasValue)
                {
                    if (!coarse.TryGetValue(entry.CoarseSin.Value, out List<SinEntry> inCoarse))
                    {
                        inCoarse = new List<SinEntry>();
                        coarse.Add(entry.CoarseSin.Value, inCoarse);
                    }

                    inCoarse.Add(entry);
                }
            }

            AllEntries = entries.AsReadOnly();
            ByCategory = new Dictionary<SinCategory, IReadOnlyList<SinEntry>>();
            foreach (KeyValuePair<SinCategory, List<SinEntry>> pair in cat)
            {
                ByCategory.Add(pair.Key, pair.Value.AsReadOnly());
            }

            ByCoarse = new Dictionary<Sin, IReadOnlyList<SinEntry>>();
            foreach (KeyValuePair<Sin, List<SinEntry>> pair in coarse)
            {
                ByCoarse.Add(pair.Key, pair.Value.AsReadOnly());
            }
        }

        /// <summary>Every entry in checklist order, including the omitted placeholder.</summary>
        public static IReadOnlyList<SinEntry> All => AllEntries;

        /// <summary>
        /// Gets an entry by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The entry.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the id is not catalogued (including Unknown).</exception>
        public static SinEntry Get(SinId id)
        {
            if (!ById.TryGetValue(id, out SinEntry entry))
            {
                throw new KeyNotFoundException("No catalogued sin has id " + (uint)id + ".");
            }

            return entry;
        }

        /// <summary>
        /// Gets a sin as a <see cref="MoralConcept"/> to put in an <see cref="Act"/> for
        /// <see cref="Reality.Witness(Act)"/>, e.g. <c>new Act("...", SinCatalog.Concept(SinId.Stealing))</c>.
        /// </summary>
        /// <param name="id">A catalogued, non-omitted id.</param>
        /// <returns>The concept (one shared instance per id).</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the id is not catalogued or is the omitted placeholder.</exception>
        public static CataloguedSin Concept(SinId id)
        {
            if (!Concepts.TryGetValue(id, out CataloguedSin concept))
            {
                throw new KeyNotFoundException("No witnessable sin has id " + (uint)id + ".");
            }

            return concept;
        }

        /// <summary>
        /// Tries to get an entry by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="entry">The entry when found.</param>
        /// <returns>True when found.</returns>
        public static bool TryGet(SinId id, out SinEntry entry) => ById.TryGetValue(id, out entry);

        /// <summary>
        /// Gets the (non-omitted) entries in a category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>The entries; empty when none.</returns>
        public static IReadOnlyList<SinEntry> InCategory(SinCategory category) =>
            ByCategory.TryGetValue(category, out IReadOnlyList<SinEntry> list) ? list : Empty;

        /// <summary>
        /// Gets the entries that fall under a coarse moderation sin.
        /// </summary>
        /// <param name="sin">The coarse sin.</param>
        /// <returns>The entries; empty when none.</returns>
        public static IReadOnlyList<SinEntry> UnderCoarseSin(Sin sin) =>
            ByCoarse.TryGetValue(sin, out IReadOnlyList<SinEntry> list) ? list : Empty;
    }
}
