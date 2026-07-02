// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// The whole character of God, with every facet always live at once. Not a selector that picks
    /// one attribute: a wound draws Mercy forward and a theft draws Justice forward, yet no facet is
    /// ever switched off. The facets harmonize into a single <see cref="Resolution"/>.
    ///
    /// <para>
    /// Harmonization is deliberately not a veto. Coherence is the mean of every facet's reading, so
    /// an act that fully satisfies two facets at once stays fully coherent. This is what lets the
    /// cross be full Justice and full Mercy together (Romans 3:25-26), which a "lowest wins" rule
    /// could never express. Disorder, by contrast, is driven by the gravity of the gravest wrong the
    /// deed commits: a capital crime (murder, innocent blood) destabilizes reality far more than a
    /// minor wrong (Romans 8:20-22; Scripture grades sin, Matthew 23:23; John 19:11), and incidental good
    /// elsewhere does not excuse it.
    /// </para>
    /// </summary>
    public class DivineCharacter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DivineCharacter"/> class from the facets that
        /// are all always live.
        /// </summary>
        /// <param name="facets">The facets of God's character (Love, Justice, Mercy, Faithfulness).</param>
        public DivineCharacter(params DivineAttribute[] facets)
        {
            if (facets == null)
            {
                throw new System.ArgumentNullException(nameof(facets));
            }

            if (facets.Length == 0)
            {
                throw new System.ArgumentException(
                    "God's character is never empty; at least one facet must be live.", nameof(facets));
            }

            this.facets = facets;
        }

        private readonly DivineAttribute[] facets;

        /// <summary>
        /// Witnesses a situation under the whole of God's character at once and harmonizes every
        /// facet's reading into one resolution.
        /// </summary>
        /// <param name="act">The deed to witness.</param>
        /// <returns>The harmonized <see cref="Resolution"/>.</returns>
        public Resolution Harmonize(Act act)
        {
            if (act == null)
            {
                throw new System.ArgumentNullException(nameof(act));
            }

            System.Collections.Generic.Dictionary<string, double> readings =
                new System.Collections.Generic.Dictionary<string, double>();

            double total = 0.0;
            foreach (DivineAttribute facet in facets)
            {
                double reading = facet.Read(act);
                readings[facet.Name] = reading;
                total = total + reading;
            }

            double coherence = total / facets.Length;
            double disorder = Disorder(act);
            double restoration = Restoration(act);

            return new Resolution(coherence, disorder, readings, restoration);
        }

        /// <summary>
        /// How much a deed restores the order sin has broken: the strongest restorative concept it
        /// carries (repentance, atonement, forgiveness, grace). 2 Chronicles 7:14; Romans 5:20.
        /// </summary>
        /// <param name="act">The deed being witnessed.</param>
        /// <returns>The restoration it offers, from 0.0 to 1.0.</returns>
        private static double Restoration(Act act)
        {
            double restoration = 0.0;
            foreach (MoralConcept concept in act.Concepts)
            {
                if (concept.Restoration > restoration)
                {
                    restoration = concept.Restoration;
                }
            }

            return restoration;
        }

        /// <summary>
        /// The disorder a deed unleashes into reality: the gravity of the gravest wrong it commits.
        /// Scripture grades sin (Matthew 23:23; John 19:11), so a capital wrong (murder, innocent
        /// blood) destabilizes reality far more than a minor one (a rude word). A deed that offends
        /// no facet of God's character introduces no disorder at all.
        /// </summary>
        /// <param name="act">The deed being witnessed.</param>
        /// <returns>The disorder introduced, from 0.0 to 1.0.</returns>
        private double Disorder(Act act)
        {
            double disorder = 0.0;
            foreach (MoralConcept concept in act.Concepts)
            {
                if (Offends(concept, act))
                {
                    double weight = Weight(concept.Gravity);
                    if (weight > disorder)
                    {
                        disorder = weight;
                    }
                }
            }

            return disorder;
        }

        /// <summary>
        /// Whether a concept offends any facet of this character of God, read in the deed's context.
        /// </summary>
        /// <param name="concept">The moral concept to test.</param>
        /// <param name="act">The whole deed, carrying its context.</param>
        /// <returns><c>true</c> if the concept violates at least one facet here.</returns>
        private bool Offends(MoralConcept concept, Act act)
        {
            foreach (DivineAttribute facet in facets)
            {
                if (concept.Violates(facet, act))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The disorder weight of a gravity, from 0.0 (none) to 1.0 (a capital wrong).
        /// </summary>
        /// <param name="gravity">The gravity of a wrong.</param>
        /// <returns>The fraction of total disorder it unleashes.</returns>
        private static double Weight(Gravity gravity)
        {
            return (double)(int)gravity / (double)(int)Gravity.Capital;
        }
    }
}
