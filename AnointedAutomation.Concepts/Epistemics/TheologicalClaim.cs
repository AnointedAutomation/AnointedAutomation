// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Epistemics
{
    /// <summary>
    /// A claim in the theory layer: a theological or metaphysical statement made by a tradition,
    /// carrying its source and a confidence. Claims are data; they interact with foundational
    /// claims and with one another only through the shared proposition vocabulary. "Materialist
    /// cosmology" claims sit here on exactly the same footing as "Genesis 1:1" claims.
    /// </summary>
    public class TheologicalClaim
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TheologicalClaim"/> class.
        /// </summary>
        /// <param name="statement">The claim in plain words.</param>
        /// <param name="source">Tradition plus citation, e.g. "Genesis 1:1" or "materialist cosmology".</param>
        /// <param name="confidence">How firmly the source holds it, 0.0 to 1.0.</param>
        /// <param name="asserts">Propositions the claim asserts.</param>
        /// <param name="denies">Propositions the claim denies.</param>
        public TheologicalClaim(
            string statement,
            string source,
            double confidence,
            System.Collections.Generic.IEnumerable<Proposition> asserts,
            System.Collections.Generic.IEnumerable<Proposition> denies)
        {
            if (string.IsNullOrWhiteSpace(statement))
            {
                throw new System.ArgumentException("A claim requires a statement.", nameof(statement));
            }

            if (string.IsNullOrWhiteSpace(source))
            {
                throw new System.ArgumentException("A claim requires a source.", nameof(source));
            }

            if (asserts == null)
            {
                throw new System.ArgumentNullException(nameof(asserts));
            }

            if (denies == null)
            {
                throw new System.ArgumentNullException(nameof(denies));
            }

            if (confidence < 0.0 || confidence > 1.0)
            {
                throw new System.ArgumentOutOfRangeException(
                    nameof(confidence),
                    "Confidence must be between 0.0 and 1.0.");
            }

            System.Collections.Generic.List<Proposition> assertList =
                new System.Collections.Generic.List<Proposition>(asserts);
            System.Collections.Generic.List<Proposition> denyList =
                new System.Collections.Generic.List<Proposition>(denies);

            if (assertList.Count == 0 && denyList.Count == 0)
            {
                throw new System.ArgumentException(
                    "A claim must assert or deny at least one proposition.",
                    nameof(asserts));
            }

            Statement = statement;
            Source = source;
            Confidence = confidence;
            Asserts = assertList;
            Denies = denyList;
        }

        /// <summary>
        /// The claim in plain words.
        /// </summary>
        public string Statement
        {
            get;
        }

        /// <summary>
        /// Tradition plus citation, e.g. "Genesis 1:1" or "materialist cosmology".
        /// </summary>
        public string Source
        {
            get;
        }

        /// <summary>
        /// How firmly the source holds the claim, 0.0 to 1.0.
        /// </summary>
        public double Confidence
        {
            get;
        }

        /// <summary>
        /// Propositions the claim asserts.
        /// </summary>
        public System.Collections.Generic.IReadOnlyCollection<Proposition> Asserts
        {
            get;
        }

        /// <summary>
        /// Propositions the claim denies.
        /// </summary>
        public System.Collections.Generic.IReadOnlyCollection<Proposition> Denies
        {
            get;
        }

        /// <summary>
        /// Whether this claim asserts the given proposition (by vocabulary name).
        /// </summary>
        /// <param name="proposition">The proposition to look for.</param>
        /// <returns>True when the proposition is asserted.</returns>
        public bool AssertsProposition(Proposition proposition)
        {
            if (proposition == null)
            {
                throw new System.ArgumentNullException(nameof(proposition));
            }

            return Contains(Asserts, proposition);
        }

        /// <summary>
        /// Whether this claim denies the given proposition (by vocabulary name).
        /// </summary>
        /// <param name="proposition">The proposition to look for.</param>
        /// <returns>True when the proposition is denied.</returns>
        public bool DeniesProposition(Proposition proposition)
        {
            if (proposition == null)
            {
                throw new System.ArgumentNullException(nameof(proposition));
            }

            return Contains(Denies, proposition);
        }

        private static bool Contains(
            System.Collections.Generic.IReadOnlyCollection<Proposition> propositions,
            Proposition proposition)
        {
            foreach (Proposition candidate in propositions)
            {
                if (candidate.Equals(proposition))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
