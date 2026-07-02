// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// What <see cref="Reality"/> gives back when it witnesses a situation. Not a pass/fail verdict:
    /// a reading of how well an act coheres with the grounding (its truth) and how much disorder the
    /// act introduces into reality. Acting against God's character does not "fail"; it destabilizes
    /// the world, the way sin warps the courses of the stars (Romans 8:20-22).
    /// </summary>
    public class Resolution
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Resolution"/> class.
        /// </summary>
        /// <param name="coherence">How well the act fits the grounding (its truth).</param>
        /// <param name="disorder">How much disorder the act introduces into reality (Romans 8:20-22).</param>
        public Resolution(double coherence, double disorder)
            : this(coherence, disorder, new System.Collections.Generic.Dictionary<string, double>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resolution"/> class carrying the per-facet
        /// readings that harmonized into it, so a resolution is transparent: you can see that the
        /// cross read full Justice and full Mercy at once.
        /// </summary>
        /// <param name="coherence">How well the act fits the grounding (its truth).</param>
        /// <param name="disorder">How much disorder the act introduces into reality (Romans 8:20-22).</param>
        /// <param name="readings">Each facet's reading of the act, keyed by facet name.</param>
        public Resolution(
            double coherence,
            double disorder,
            System.Collections.Generic.IReadOnlyDictionary<string, double> readings)
            : this(coherence, disorder, readings, 0.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resolution"/> class carrying, in addition, how
        /// much the deed restores the order it touches: repentance, atonement, and grace heal the
        /// record they meet (2 Chronicles 7:14; 1 John 1:9; Romans 5:20, "where sin increased, grace
        /// increased all the more").
        /// </summary>
        /// <param name="coherence">How well the act fits the grounding (its truth).</param>
        /// <param name="disorder">How much disorder the act introduces into reality (Romans 8:20-22).</param>
        /// <param name="readings">Each facet's reading of the act, keyed by facet name.</param>
        /// <param name="restoration">How much the deed restores reality's order, from 0.0 to 1.0.</param>
        public Resolution(
            double coherence,
            double disorder,
            System.Collections.Generic.IReadOnlyDictionary<string, double> readings,
            double restoration)
        {
            if (readings == null)
            {
                throw new System.ArgumentNullException(nameof(readings));
            }

            Coherence = Clamp(coherence);
            Disorder = Clamp(disorder);
            Restoration = Clamp(restoration);
            this.readings = readings;
        }

        /// <summary>
        /// How much this deed restores the standing order of reality, from 0.0 (no restoration) to
        /// 1.0 (full restoration). Repentance and atonement heal; a bare deed restores nothing.
        /// </summary>
        public double Restoration
        {
            get;
        }

        private readonly System.Collections.Generic.IReadOnlyDictionary<string, double> readings;

        /// <summary>
        /// The reading a single facet of God's character gave the witnessed act.
        /// </summary>
        /// <param name="facet">The facet name (for example, "Justice" or "Mercy").</param>
        /// <returns>That facet's coherence reading.</returns>
        public double Reading(string facet)
        {
            if (string.IsNullOrEmpty(facet))
            {
                throw new System.ArgumentNullException(nameof(facet));
            }

            double reading;
            if (readings.TryGetValue(facet, out reading))
            {
                return reading;
            }

            throw new System.Collections.Generic.KeyNotFoundException(
                "No facet named '" + facet + "' contributed to this resolution.");
        }

        /// <summary>
        /// Every facet's reading of the witnessed act, keyed by facet name.
        /// </summary>
        public System.Collections.Generic.IReadOnlyDictionary<string, double> Readings
        {
            get
            {
                return readings;
            }
        }

        /// <summary>
        /// Confines a reading to the unit range. Reality cannot be more than fully coherent, nor
        /// less than fully incoherent.
        /// </summary>
        /// <param name="value">The raw reading.</param>
        /// <returns>The reading confined to 0.0 through 1.0.</returns>
        private static double Clamp(double value)
        {
            if (value < 0.0)
            {
                return 0.0;
            }

            if (value > 1.0)
            {
                return 1.0;
            }

            return value;
        }

        /// <summary>
        /// Gets how well the witnessed act fits the grounding (its truth), from 0.0 to 1.0.
        /// </summary>
        public double Coherence
        {
            get;
        }

        /// <summary>
        /// Gets how much disorder the witnessed act introduces into reality (Romans 8:20-22), from 0.0
        /// to 1.0.
        /// </summary>
        public double Disorder
        {
            get;
        }
    }
}
