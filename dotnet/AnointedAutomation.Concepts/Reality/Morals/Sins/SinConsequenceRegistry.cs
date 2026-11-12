// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System;
using System.Collections.Generic;

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// Consequences keyed by <see cref="SinId"/>. Not thread safe for concurrent registration;
    /// register at startup, then apply.
    /// </summary>
    /// <typeparam name="TTarget">What the consequences act on.</typeparam>
    public sealed class SinConsequenceRegistry<TTarget>
    {
        private readonly Dictionary<SinId, List<ISinConsequence<TTarget>>> bySin = new Dictionary<SinId, List<ISinConsequence<TTarget>>>();

        /// <summary>
        /// Registers a consequence for a catalogued sin.
        /// </summary>
        /// <param name="id">A catalogued, non-omitted sin id.</param>
        /// <param name="consequence">The consequence.</param>
        /// <exception cref="ArgumentNullException">Thrown when the consequence is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the id is omitted.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the id is not catalogued.</exception>
        public void Register(SinId id, ISinConsequence<TTarget> consequence)
        {
            if (consequence == null)
            {
                throw new ArgumentNullException(nameof(consequence));
            }

            if (SinCatalog.Get(id).IsOmitted)
            {
                throw new ArgumentException("Cannot attach a consequence to an omitted entry.", nameof(id));
            }

            if (!bySin.TryGetValue(id, out List<ISinConsequence<TTarget>> list))
            {
                list = new List<ISinConsequence<TTarget>>();
                bySin.Add(id, list);
            }

            list.Add(consequence);
        }

        /// <summary>
        /// Gets the consequences registered for a sin.
        /// </summary>
        /// <param name="id">The sin id.</param>
        /// <returns>The consequences in registration order; empty when none.</returns>
        public IReadOnlyList<ISinConsequence<TTarget>> For(SinId id) =>
            bySin.TryGetValue(id, out List<ISinConsequence<TTarget>> list) ? list.AsReadOnly() : Array.AsReadOnly(new ISinConsequence<TTarget>[0]);

        /// <summary>
        /// Applies every consequence registered for a sin, in registration order.
        /// </summary>
        /// <param name="id">The sin committed.</param>
        /// <param name="target">What the consequences act on.</param>
        /// <returns>How many consequences ran.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the id is not catalogued.</exception>
        public int Apply(SinId id, TTarget target)
        {
            SinEntry sin = SinCatalog.Get(id);
            IReadOnlyList<ISinConsequence<TTarget>> list = For(id);
            foreach (ISinConsequence<TTarget> consequence in list)
            {
                consequence.Apply(sin, target);
            }

            return list.Count;
        }
    }
}
