// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System.Collections.Generic;

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// One fact of the canon: an <see cref="Reasoning.Assertion"/> and the Scripture that grounds it.
    /// A canon fact always cites at least one reference, so any verdict built on facts cites Scripture.
    /// </summary>
    public sealed class CanonFact
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CanonFact"/> class.
        /// </summary>
        /// <param name="assertion">What the fact asserts.</param>
        /// <param name="scripture">The references that ground it; at least one.</param>
        /// <exception cref="System.ArgumentNullException">An argument is null.</exception>
        /// <exception cref="System.ArgumentException">No reference is given.</exception>
        public CanonFact(Assertion assertion, IReadOnlyList<ScriptureReference> scripture)
        {
            Assertion = ReasoningGuard.NotNull(assertion, nameof(assertion));
            Scripture = ReasoningGuard.CopyOf(scripture, nameof(scripture));
            if (Scripture.Count == 0)
            {
                throw new System.ArgumentException("A canon fact needs at least one Scripture reference.", nameof(scripture));
            }
        }

        /// <summary>
        /// Gets what the fact asserts.
        /// </summary>
        public Assertion Assertion { get; }

        /// <summary>
        /// Gets the references that ground the fact (never empty).
        /// </summary>
        public IReadOnlyList<ScriptureReference> Scripture { get; }

        /// <summary>
        /// Returns the assertion and its first reference, for diagnostics.
        /// </summary>
        /// <returns>The fact as text.</returns>
        public override string ToString()
        {
            return Assertion.Subject + " " + Assertion.Relation + " " + Assertion.Object + " (" + Scripture[0] + ")";
        }
    }
}
