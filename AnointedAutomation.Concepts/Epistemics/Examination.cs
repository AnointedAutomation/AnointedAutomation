// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Epistemics
{
    /// <summary>
    /// The result of examining one claim: a verdict, its three-valued standing, the confidence it
    /// was reached with (a conclusion is only as strong as its weakest premise), and the derivation
    /// showing its work.
    /// </summary>
    public class Examination
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Examination"/> class.
        /// </summary>
        /// <param name="claim">The claim that was examined.</param>
        /// <param name="verdict">The verdict reached.</param>
        /// <param name="confidence">Minimum confidence along the derivation chain, 0.0 to 1.0.</param>
        /// <param name="derivation">The ordered steps that produced the verdict.</param>
        public Examination(
            TheologicalClaim claim,
            Verdict verdict,
            double confidence,
            System.Collections.Generic.IReadOnlyList<DerivationStep> derivation)
        {
            if (claim == null)
            {
                throw new System.ArgumentNullException(nameof(claim));
            }

            if (derivation == null)
            {
                throw new System.ArgumentNullException(nameof(derivation));
            }

            if (confidence < 0.0 || confidence > 1.0)
            {
                throw new System.ArgumentOutOfRangeException(
                    nameof(confidence),
                    "Confidence must be between 0.0 and 1.0.");
            }

            Claim = claim;
            Verdict = verdict;
            Confidence = confidence;
            Derivation = derivation;
        }

        /// <summary>
        /// The claim that was examined.
        /// </summary>
        public TheologicalClaim Claim
        {
            get;
        }

        /// <summary>
        /// The verdict reached.
        /// </summary>
        public Verdict Verdict
        {
            get;
        }

        /// <summary>
        /// The three-valued standing of the claim. Consistent leans true (provisional, like every
        /// scientific claim), Contradicts is false relative to the current unfalsified set, and
        /// both Unfalsifiable and Undetermined are honestly null. Null is never defaulted away.
        /// </summary>
        public bool? Standing
        {
            get
            {
                if (Verdict == Verdict.Consistent)
                {
                    return true;
                }

                if (Verdict == Verdict.Contradicts)
                {
                    return false;
                }

                return null;
            }
        }

        /// <summary>
        /// Minimum confidence along the derivation chain, 0.0 to 1.0.
        /// </summary>
        public double Confidence
        {
            get;
        }

        /// <summary>
        /// The ordered steps that produced the verdict, including recorded domain skips.
        /// </summary>
        public System.Collections.Generic.IReadOnlyList<DerivationStep> Derivation
        {
            get;
        }
    }
}
