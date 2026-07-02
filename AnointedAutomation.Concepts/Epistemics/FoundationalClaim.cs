// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Epistemics
{
    /// <summary>
    /// The measuring stick of the ledger. Not an ontologically special "law": every foundational
    /// claim began as an assumption someone was willing to have proven wrong, so it is falsifiable
    /// by definition and functions as bedrock only by the weight of falsification it has survived.
    /// Examples: non-contradiction, causality, conservation of energy, entropy.
    /// </summary>
    public class FoundationalClaim
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FoundationalClaim"/> class.
        /// </summary>
        /// <param name="name">Unique name, e.g. "ConservationOfEnergy".</param>
        /// <param name="statement">The claim in plain words.</param>
        /// <param name="domain">Where the claim has authority.</param>
        /// <param name="asserts">Propositions the claim asserts.</param>
        /// <param name="denies">Propositions the claim denies.</param>
        /// <param name="survivedFalsificationWeight">How much testing it has survived, 0.0 to 1.0.</param>
        public FoundationalClaim(
            string name,
            string statement,
            LawDomain domain,
            System.Collections.Generic.IEnumerable<Proposition> asserts,
            System.Collections.Generic.IEnumerable<Proposition> denies,
            double survivedFalsificationWeight)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new System.ArgumentException("A foundational claim requires a name.", nameof(name));
            }

            if (statement == null)
            {
                throw new System.ArgumentNullException(nameof(statement));
            }

            if (asserts == null)
            {
                throw new System.ArgumentNullException(nameof(asserts));
            }

            if (denies == null)
            {
                throw new System.ArgumentNullException(nameof(denies));
            }

            if (survivedFalsificationWeight < 0.0 || survivedFalsificationWeight > 1.0)
            {
                throw new System.ArgumentOutOfRangeException(
                    nameof(survivedFalsificationWeight),
                    "Survived-falsification weight must be between 0.0 and 1.0.");
            }

            System.Collections.Generic.List<Proposition> assertList =
                new System.Collections.Generic.List<Proposition>(asserts);
            System.Collections.Generic.List<Proposition> denyList =
                new System.Collections.Generic.List<Proposition>(denies);

            if (assertList.Count == 0 && denyList.Count == 0)
            {
                throw new System.ArgumentException(
                    "A foundational claim must assert or deny at least one proposition.",
                    nameof(asserts));
            }

            Name = name;
            Statement = statement;
            Domain = domain;
            Asserts = assertList;
            Denies = denyList;
            SurvivedFalsificationWeight = survivedFalsificationWeight;
        }

        /// <summary>
        /// Unique name of the foundational claim.
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// The claim in plain words.
        /// </summary>
        public string Statement
        {
            get;
        }

        /// <summary>
        /// Where the claim has authority.
        /// </summary>
        public LawDomain Domain
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
        /// Always true. An unfalsifiable foundational claim would be a decree, not science; the
        /// faith step at the root of the method is explicit in the model.
        /// </summary>
        public bool Falsifiable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// How much falsification the claim has survived, 0.0 to 1.0. This weight, not type-level
        /// specialness, is why it functions as bedrock.
        /// </summary>
        public double SurvivedFalsificationWeight
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
