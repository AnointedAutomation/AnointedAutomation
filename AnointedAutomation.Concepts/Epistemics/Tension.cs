// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Epistemics
{
    /// <summary>
    /// A first-class record of a contradiction between two theological claims: both claims and the
    /// shared proposition they disagree on. Neither claim is deleted or rejected; the ledger holds
    /// both, source-tagged, plus this record. Contradictions are output, never a crash.
    /// </summary>
    public class Tension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tension"/> class.
        /// </summary>
        /// <param name="first">One of the disagreeing claims.</param>
        /// <param name="second">The other disagreeing claim.</param>
        /// <param name="proposition">The proposition one asserts and the other denies.</param>
        public Tension(TheologicalClaim first, TheologicalClaim second, Proposition proposition)
        {
            if (first == null)
            {
                throw new System.ArgumentNullException(nameof(first));
            }

            if (second == null)
            {
                throw new System.ArgumentNullException(nameof(second));
            }

            if (proposition == null)
            {
                throw new System.ArgumentNullException(nameof(proposition));
            }

            First = first;
            Second = second;
            Proposition = proposition;
        }

        /// <summary>
        /// One of the disagreeing claims.
        /// </summary>
        public TheologicalClaim First
        {
            get;
        }

        /// <summary>
        /// The other disagreeing claim.
        /// </summary>
        public TheologicalClaim Second
        {
            get;
        }

        /// <summary>
        /// The proposition one claim asserts and the other denies.
        /// </summary>
        public Proposition Proposition
        {
            get;
        }
    }
}
