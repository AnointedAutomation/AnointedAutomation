// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests
{
    /// <summary>
    /// The shared oracle for the canon tests. Scripture is the regression standard: the engine must
    /// always agree with the Biblical record. A deed is composed of moral concepts, witnessed under
    /// God's whole revealed character, and judged by one of three verdicts.
    /// </summary>
    internal static class OracleHarness
    {
        /// <summary>Witnesses a deed (composed of moral concepts) on reality grounded in God.</summary>
        public static Resolution Witness(string deed, params MoralConcept[] concepts)
        {
            return Reality.Revealed().Witness(new Act(deed, concepts));
        }

        /// <summary>Witnesses a deed done by an agent grounded in something other than God.</summary>
        public static Resolution Grounded(string deed, Grounding grounding, params MoralConcept[] concepts)
        {
            return Reality.Revealed().Witness(new Act(deed, concepts), grounding);
        }

        /// <summary>
        /// Witnesses a deed framed by context (war, divine command, defense of the innocent), so a
        /// context-sensitive concept can be read in its light.
        /// </summary>
        public static Resolution WitnessInContext(string deed, Circumstance[] context, params MoralConcept[] concepts)
        {
            return Reality.Revealed().Witness(new Act(deed, concepts).InContext(context));
        }

        /// <summary>An act of God or of Christ: never incoherent, never disorders reality.</summary>
        public static void DivineAct(Resolution r)
        {
            Assert.True(r.Coherence >= 0.5, "An act of God can never read incoherent.");
            Assert.Equal(0.0, r.Disorder);
        }

        /// <summary>A righteous human deed: coherent, and it destabilizes nothing.</summary>
        public static void Righteous(Resolution r)
        {
            Assert.True(r.Coherence > 0.5, "A righteous deed must read coherent.");
            Assert.Equal(0.0, r.Disorder);
        }

        /// <summary>Sin: always reads incoherent, and always introduces disorder (1 Enoch 80).</summary>
        public static void Sin(Resolution r)
        {
            Assert.True(r.Coherence < 0.5, "Sin must read incoherent.");
            Assert.True(r.Disorder > 0.0, "Sin must introduce disorder into reality.");
        }

        /// <summary>
        /// The fullness: an act in which the whole of God's character is satisfied at once, reading
        /// perfectly coherent with no disorder (the cross; Micah 6:8). Stricter than DivineAct, which
        /// only requires non-incoherence. This pins the exact arithmetic so a regression fails loudly.
        /// </summary>
        public static void FullyDivine(Resolution r)
        {
            Assert.Equal(1.0, r.Coherence);
            Assert.Equal(0.0, r.Disorder);
        }

        /// <summary>Asserts a single facet read an exact value, so per-facet nuance is pinned.</summary>
        public static void Facet(Resolution r, string facet, double expected)
        {
            Assert.Equal(expected, r.Reading(facet));
        }

        /// <summary>Asserts exact coherence and disorder, the finest-grained verdict.</summary>
        public static void Exact(Resolution r, double coherence, double disorder)
        {
            Assert.Equal(coherence, r.Coherence);
            Assert.Equal(disorder, r.Disorder);
        }
    }
}
