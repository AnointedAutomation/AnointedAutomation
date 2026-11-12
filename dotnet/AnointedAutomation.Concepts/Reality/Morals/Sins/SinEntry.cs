// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System;
using System.Collections.Generic;
using AnointedAutomation.Enums;

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// One catalogued sin: its stable id, its number in the source checklist, its short label, its
    /// category, the coarse moderation <see cref="Sin"/> it falls under (when one fits), and the
    /// Scripture it is grounded in. Immutable.
    /// </summary>
    public sealed class SinEntry
    {
        /// <summary>
        /// Creates a catalogued sin.
        /// </summary>
        /// <param name="id">Stable id; never <see cref="SinId.Unknown"/>.</param>
        /// <param name="documentNumber">The entry's number in the source checklist.</param>
        /// <param name="name">Short label.</param>
        /// <param name="category">Category; never <see cref="SinCategory.Unknown"/>.</param>
        /// <param name="coarseSin">The coarse moderation sin it falls under, or null when none fits.</param>
        /// <param name="note">A note on a source quirk, or empty.</param>
        /// <param name="references">At least one Scripture reference.</param>
        /// <exception cref="ArgumentException">Thrown when the id, name, category or references are missing.</exception>
        public SinEntry(SinId id, int documentNumber, string name, SinCategory category, Sin? coarseSin, string note, params ScriptureReference[] references)
            : this(id, documentNumber, name, category, coarseSin, note, false, references)
        {
        }

        private SinEntry(SinId id, int documentNumber, string name, SinCategory category, Sin? coarseSin, string note, bool omitted, ScriptureReference[] references)
        {
            if (id == SinId.Unknown)
            {
                throw new ArgumentException("SinId.Unknown is not a sin.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("A catalogued sin needs a name.", nameof(name));
            }

            if (!omitted && category == SinCategory.Unknown)
            {
                throw new ArgumentException("A catalogued sin needs a category.", nameof(category));
            }

            if (references == null || (!omitted && references.Length == 0))
            {
                throw new ArgumentException("A catalogued sin needs at least one Scripture reference.", nameof(references));
            }

            if (note == null)
            {
                throw new ArgumentNullException(nameof(note));
            }

            Id = id;
            DocumentNumber = documentNumber;
            Name = name;
            Category = category;
            CoarseSin = coarseSin;
            Note = note;
            IsOmitted = omitted;
            References = Array.AsReadOnly((ScriptureReference[])references.Clone());
        }

        /// <summary>
        /// Creates the placeholder for a number the source checklist skips.
        /// </summary>
        /// <param name="id">Stable id.</param>
        /// <param name="documentNumber">The skipped number.</param>
        /// <param name="note">Why it is a placeholder.</param>
        /// <returns>An omitted entry with no category, coarse sin or references.</returns>
        public static SinEntry Omitted(SinId id, int documentNumber, string note) =>
            new SinEntry(id, documentNumber, "(omitted)", SinCategory.Unknown, null, note, true, new ScriptureReference[0]);

        /// <summary>Stable id; persist this.</summary>
        public SinId Id { get; }

        /// <summary>The entry's number in the source checklist (two entries share 566).</summary>
        public int DocumentNumber { get; }

        /// <summary>Short label.</summary>
        public string Name { get; }

        /// <summary>Category.</summary>
        public SinCategory Category { get; }

        /// <summary>The coarse moderation sin it falls under, or null when none fits.</summary>
        public Sin? CoarseSin { get; }

        /// <summary>A note on a source quirk, or empty.</summary>
        public string Note { get; }

        /// <summary>True for a placeholder of a number the source skips.</summary>
        public bool IsOmitted { get; }

        /// <summary>The Scripture this sin is grounded in (empty only when omitted).</summary>
        public IReadOnlyList<ScriptureReference> References { get; }

        /// <summary>
        /// Returns the label.
        /// </summary>
        /// <returns>The label.</returns>
        public override string ToString() => Name;
    }
}
