// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Epistemics
{
    /// <summary>
    /// One step in how a verdict was reached: which authority (foundational claim or peer claim)
    /// touched which proposition, and what happened, including recorded domain skips so the
    /// engine's neutrality is auditable.
    /// </summary>
    public class DerivationStep
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DerivationStep"/> class.
        /// </summary>
        /// <param name="authority">Name of the foundational or peer claim involved.</param>
        /// <param name="propositionName">The proposition the step turned on.</param>
        /// <param name="outcome">What happened, in plain words.</param>
        public DerivationStep(string authority, string propositionName, string outcome)
        {
            if (string.IsNullOrWhiteSpace(authority))
            {
                throw new System.ArgumentException("A step requires an authority.", nameof(authority));
            }

            if (string.IsNullOrWhiteSpace(propositionName))
            {
                throw new System.ArgumentException("A step requires a proposition.", nameof(propositionName));
            }

            if (string.IsNullOrWhiteSpace(outcome))
            {
                throw new System.ArgumentException("A step requires an outcome.", nameof(outcome));
            }

            Authority = authority;
            PropositionName = propositionName;
            Outcome = outcome;
        }

        /// <summary>
        /// Name of the foundational or peer claim involved.
        /// </summary>
        public string Authority
        {
            get;
        }

        /// <summary>
        /// The proposition the step turned on.
        /// </summary>
        public string PropositionName
        {
            get;
        }

        /// <summary>
        /// What happened, in plain words.
        /// </summary>
        public string Outcome
        {
            get;
        }
    }
}
