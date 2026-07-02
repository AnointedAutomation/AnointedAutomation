// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Epistemics
{
    /// <summary>
    /// The engine. Holds foundational claims (the measuring stick) and admitted theological claims
    /// (the theory layer), examines claims against both, and records tensions where traditions
    /// collide. It does not decide theology: contradiction and undecidability are output data,
    /// never errors, and the domain boundaries on foundational claims keep it neutral about
    /// anything that cannot be tested from inside the universe.
    /// </summary>
    public class EpistemicLedger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EpistemicLedger"/> class over a set of
        /// foundational claims.
        /// </summary>
        /// <param name="foundations">The hand-curated foundational claims.</param>
        public EpistemicLedger(System.Collections.Generic.IEnumerable<FoundationalClaim> foundations)
        {
            if (foundations == null)
            {
                throw new System.ArgumentNullException(nameof(foundations));
            }

            this.foundations = new System.Collections.Generic.List<FoundationalClaim>();
            System.Collections.Generic.HashSet<string> names =
                new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);
            foreach (FoundationalClaim foundation in foundations)
            {
                if (!names.Add(foundation.Name))
                {
                    throw new System.ArgumentException(
                        "Duplicate foundational claim name: " + foundation.Name,
                        nameof(foundations));
                }

                this.foundations.Add(foundation);
            }

            admitted = new System.Collections.Generic.List<TheologicalClaim>();
            tensions = new System.Collections.Generic.List<Tension>();
        }

        private readonly System.Collections.Generic.List<FoundationalClaim> foundations;
        private readonly System.Collections.Generic.List<TheologicalClaim> admitted;
        private readonly System.Collections.Generic.List<Tension> tensions;

        /// <summary>
        /// Examines a claim against the foundational claims. Pure: nothing is stored. Order
        /// encodes the epistemology: foundational collisions first, then the falsifiability
        /// boundary, then support, else undetermined.
        /// </summary>
        /// <param name="claim">The claim to examine.</param>
        /// <returns>The examination, with verdict, standing, confidence, and derivation.</returns>
        public Examination Examine(TheologicalClaim claim)
        {
            if (claim == null)
            {
                throw new System.ArgumentNullException(nameof(claim));
            }

            System.Collections.Generic.List<DerivationStep> derivation =
                new System.Collections.Generic.List<DerivationStep>();
            System.Collections.Generic.List<Proposition> touched =
                new System.Collections.Generic.List<Proposition>(claim.Asserts);
            touched.AddRange(claim.Denies);

            // 1. Foundational check: collisions with the measuring stick, honoring domains.
            foreach (Proposition proposition in touched)
            {
                foreach (FoundationalClaim foundation in foundations)
                {
                    if (foundation.Domain == LawDomain.IntraUniverse
                        && proposition.Testability == Testability.BeyondObservation)
                    {
                        if (foundation.AssertsProposition(proposition) || foundation.DeniesProposition(proposition))
                        {
                            derivation.Add(new DerivationStep(
                                foundation.Name,
                                proposition.Name,
                                "domain skip: intra-universe authority does not reach beyond observation"));
                        }

                        continue;
                    }

                    bool collision =
                        (claim.AssertsProposition(proposition) && foundation.DeniesProposition(proposition))
                        || (claim.DeniesProposition(proposition) && foundation.AssertsProposition(proposition));
                    if (collision)
                    {
                        derivation.Add(new DerivationStep(
                            foundation.Name,
                            proposition.Name,
                            "collision: the claim and the foundational claim cannot both stand"));
                        double contradictionConfidence = System.Math.Min(
                            claim.Confidence, foundation.SurvivedFalsificationWeight);
                        return new Examination(
                            claim, Verdict.Contradicts, contradictionConfidence, derivation);
                    }
                }
            }

            // 2. Falsifiability boundary: if nothing the claim touches can ever be observed from
            // inside the universe, the honest verdict is unfalsifiable, symmetrically for all
            // traditions.
            bool anyTestable = false;
            foreach (Proposition proposition in touched)
            {
                if (proposition.Testability == Testability.EmpiricallyTestable)
                {
                    anyTestable = true;
                    break;
                }
            }

            if (!anyTestable)
            {
                derivation.Add(new DerivationStep(
                    "Ledger",
                    touched[0].Name,
                    "unfalsifiable from inside the universe"));
                return new Examination(claim, Verdict.Unfalsifiable, claim.Confidence, derivation);
            }

            // 3. Support: an applicable foundational claim agreeing with the claim.
            foreach (Proposition proposition in touched)
            {
                foreach (FoundationalClaim foundation in foundations)
                {
                    if (foundation.Domain == LawDomain.IntraUniverse
                        && proposition.Testability == Testability.BeyondObservation)
                    {
                        continue;
                    }

                    bool support =
                        (claim.AssertsProposition(proposition) && foundation.AssertsProposition(proposition))
                        || (claim.DeniesProposition(proposition) && foundation.DeniesProposition(proposition));
                    if (support)
                    {
                        derivation.Add(new DerivationStep(
                            foundation.Name,
                            proposition.Name,
                            "support: the foundational claim agrees"));
                        double supportConfidence = System.Math.Min(
                            claim.Confidence, foundation.SurvivedFalsificationWeight);
                        return new Examination(
                            claim, Verdict.Consistent, supportConfidence, derivation);
                    }
                }
            }

            // 4. Testable in principle, but nothing on the ledger bears on it.
            derivation.Add(new DerivationStep(
                "Ledger",
                touched[0].Name,
                "no applicable foundational claim bears on it"));
            return new Examination(claim, Verdict.Undetermined, claim.Confidence, derivation);
        }
    }
}
