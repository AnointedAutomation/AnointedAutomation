// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System.Collections.Generic;

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// An engine's answer to a <see cref="Query"/>: the truth value, the confidence it was reached
    /// with, the concepts the situation was classified under, why, and where it came from.
    /// </summary>
    public sealed record Answer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Answer"/> record.
        /// </summary>
        /// <param name="query">The query answered.</param>
        /// <param name="truth">The three-valued result; Unknown means not established.</param>
        /// <param name="confidence">Confidence, 0.0 to 1.0.</param>
        /// <param name="classification">The concepts the situation was classified under; may be empty, never null.</param>
        /// <param name="why">The justification.</param>
        /// <param name="provenance">Where the answer came from.</param>
        public Answer(
            Query query,
            TruthValue truth,
            double confidence,
            IReadOnlyList<ConceptRef> classification,
            Justification why,
            Provenance provenance)
        {
            Query = ReasoningGuard.NotNull(query, nameof(query));
            Truth = ReasoningGuard.Defined(truth, nameof(truth));
            if (double.IsNaN(confidence) || confidence < 0.0 || confidence > 1.0)
            {
                throw new System.ArgumentOutOfRangeException(
                    nameof(confidence),
                    confidence,
                    "Confidence must be between 0.0 and 1.0.");
            }

            Confidence = confidence;
            Classification = ReasoningGuard.CopyOf(classification, nameof(classification));
            Why = ReasoningGuard.NotNull(why, nameof(why));
            Provenance = ReasoningGuard.NotNull(provenance, nameof(provenance));
        }

        /// <summary>
        /// The query answered.
        /// </summary>
        public Query Query { get; }

        /// <summary>
        /// The three-valued result.
        /// </summary>
        public TruthValue Truth { get; }

        /// <summary>
        /// Confidence, 0.0 to 1.0.
        /// </summary>
        public double Confidence { get; }

        /// <summary>
        /// The concepts the situation was classified under.
        /// </summary>
        public IReadOnlyList<ConceptRef> Classification { get; }

        /// <summary>
        /// The justification.
        /// </summary>
        public Justification Why { get; }

        /// <summary>
        /// Where the answer came from.
        /// </summary>
        public Provenance Provenance { get; }
    }
}
