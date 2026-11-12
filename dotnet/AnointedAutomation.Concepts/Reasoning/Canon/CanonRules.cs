// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System.Collections.Generic;

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// The default rule set of the <see cref="CanonReasoner"/>, in evaluation order.
    /// </summary>
    public static class CanonRules
    {
        /// <summary>
        /// Gets the default rules: the sin, category, instance and witness rules, then a
        /// <see cref="FactRule"/> for each relation the canon states directly.
        /// </summary>
        public static IReadOnlyList<ICanonRule> Default { get; } = new List<ICanonRule>
        {
            new SinViolatesRule(),
            new CategoryRule(),
            new IsInstanceOfRule(),
            new WitnessRule(),
            new FactRule(RelationKind.Upholds),
            new FactRule(RelationKind.Violates),
            new FactRule(RelationKind.Commands),
            new FactRule(RelationKind.Forbids),
            new FactRule(RelationKind.GroundedIn),
        }.AsReadOnly();
    }
}
