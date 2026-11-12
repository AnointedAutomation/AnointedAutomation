// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System.Globalization;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// Shared helpers for canon rules: empty results and numbered proof steps.
    /// </summary>
    internal static class CanonProof
    {
        /// <summary>No proof steps.</summary>
        internal static readonly ProofStep[] NoSteps = new ProofStep[0];

        /// <summary>No grounding facts.</summary>
        internal static readonly CanonFact[] NoGrounds = new CanonFact[0];

        /// <summary>
        /// Returns Unknown with empty steps and grounds.
        /// </summary>
        /// <param name="steps">Set to no steps.</param>
        /// <param name="grounds">Set to no grounds.</param>
        /// <returns><see cref="TruthValue.Unknown"/>.</returns>
        internal static TruthValue Unknown(out ProofStep[] steps, out CanonFact[] grounds)
        {
            steps = NoSteps;
            grounds = NoGrounds;
            return TruthValue.Unknown;
        }

        /// <summary>
        /// Numbers proof steps from pairs of (symbolic form, plain reading).
        /// </summary>
        /// <param name="formAndReading">Alternating symbolic forms and readings.</param>
        /// <returns>The numbered steps.</returns>
        internal static ProofStep[] Steps(params string[] formAndReading)
        {
            ProofStep[] steps = new ProofStep[formAndReading.Length / 2];
            for (int i = 0; i < steps.Length; i++)
            {
                steps[i] = new ProofStep(i + 1, formAndReading[2 * i], formAndReading[(2 * i) + 1]);
            }

            return steps;
        }

        /// <summary>
        /// Describes an assertion as "subject Relation object".
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>The description.</returns>
        internal static string Describe(Assertion assertion)
        {
            return assertion.Subject + " " + assertion.Relation + " " + assertion.Object;
        }

        /// <summary>
        /// Formats a reading the same way on every machine.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The invariant text.</returns>
        internal static string Number(double value)
        {
            return value.ToString("R", CultureInfo.InvariantCulture);
        }
    }
}
