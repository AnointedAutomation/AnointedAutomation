// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// A facet of God's character (Love, Justice, Mercy, Faithfulness). God embodies each, yet is not
    /// equal to any one of them ("God is love", 1 John 4:8, read as embodiment, not equation). Every
    /// facet is always live: a facet never accepts or rejects an act, it only reads how fully the act
    /// coheres with this aspect of God's character. The facets harmonize into one response in
    /// <see cref="DivineCharacter"/>.
    ///
    /// <para>
    /// A facet no longer carries a list of fact-strings. It reads an <see cref="Act"/> by asking each
    /// <see cref="MoralConcept"/> in the deed how it stands toward this facet. The moral vocabulary
    /// lives on the concepts themselves.
    /// </para>
    /// </summary>
    public abstract class DivineAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DivineAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of this facet of God's character.</param>
        protected DivineAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of this facet of God's character.
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// Reads how fully an act coheres with this facet of God's character, from 0.0 (wholly
        /// against it) through 0.5 (indifferent to it) to 1.0 (fully embodying it). An act that
        /// embodies this facet without offending it reads fully coherent; one that offends it without
        /// embodying it reads wholly against; mixed or indifferent acts sit at the neutral midpoint.
        /// </summary>
        /// <param name="act">The deed, composed of the moral concepts it embodies and offends.</param>
        /// <returns>The coherence of the act with this facet, from 0.0 to 1.0.</returns>
        public double Read(Act act)
        {
            if (act == null)
            {
                throw new System.ArgumentNullException(nameof(act));
            }

            bool upheld = false;
            bool violated = false;
            foreach (MoralConcept concept in act.Concepts)
            {
                if (concept.Upholds(this, act))
                {
                    upheld = true;
                }

                if (concept.Violates(this, act))
                {
                    violated = true;
                }
            }

            double reading = 0.5;
            if (upheld)
            {
                reading = reading + 0.5;
            }

            if (violated)
            {
                reading = reading - 0.5;
            }

            return reading;
        }
    }
}
