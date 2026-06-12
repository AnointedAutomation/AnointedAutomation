// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    /// <summary>
    /// What an agent stands on. From 1 Meqabyan: God alone gives life, and the idols "do not truly
    /// live, they cannot give life." To be grounded in God is to stand on the source of being; to be
    /// grounded in an idol is to stand on what has no ground, so even an outwardly good deed cannot
    /// hold and drifts toward non-being.
    /// </summary>
    public class Grounding
    {
        private Grounding(string name, bool isInGod, double lifelessness)
        {
            Name = name;
            IsInGod = isInGod;
            this.lifelessness = lifelessness;
        }

        /// <summary>
        /// How little life the foundation can give, from 0.0 (the living God) to 1.0 (an utterly
        /// dead idol). This is how far a deed standing on it drifts toward non-being.
        /// </summary>
        private readonly double lifelessness;

        /// <summary>
        /// Gets the name of what the agent is grounded in.
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// Gets whether the agent is grounded in the living God, the source of being.
        /// </summary>
        public bool IsInGod
        {
            get;
        }

        /// <summary>
        /// Grounded in the living God, the one who gives life (1 Meqabyan 1; John 14:6, "the life").
        /// </summary>
        /// <returns>A grounding in God.</returns>
        public static Grounding InGod()
        {
            return new Grounding("God", true, 0.0);
        }

        /// <summary>
        /// Grounded in an idol, which cannot give life (1 Meqabyan 1). A deed standing on it drifts
        /// toward non-being.
        /// </summary>
        /// <param name="name">The name of the idol.</param>
        /// <returns>A grounding in an idol.</returns>
        public static Grounding InIdol(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.ArgumentNullException(nameof(name));
            }

            return new Grounding(name, false, 0.5);
        }

        /// <summary>
        /// Grounded in a divided heart, partly toward God and partly toward an idol: the lukewarm and
        /// the double-minded. "How long will you waver between two opinions?" (1 Kings 18:21);
        /// "a double-minded man, unstable in all he does" (James 1:8); "because you are lukewarm" (Revelation 3:16).
        /// A deed on a divided foundation keeps part of its life and drifts part of the way.
        /// </summary>
        /// <returns>A divided grounding, between the living God and a dead idol.</returns>
        public static Grounding Divided()
        {
            return new Grounding("a divided heart", false, 0.25);
        }

        /// <summary>
        /// Bears a deed on this foundation. What stands on the living God keeps its life unchanged;
        /// what stands on an idol loses life and gains disorder, because the foundation cannot hold.
        /// </summary>
        /// <param name="deed">The resolution of the deed as God's character read it.</param>
        /// <returns>The deed as its foundation can sustain it.</returns>
        public Resolution Bear(Resolution deed)
        {
            if (deed == null)
            {
                throw new System.ArgumentNullException(nameof(deed));
            }

            if (IsInGod)
            {
                return deed;
            }

            double coherence = deed.Coherence * (1.0 - lifelessness);
            double disorder = deed.Disorder;
            if (lifelessness > disorder)
            {
                disorder = lifelessness;
            }

            return new Resolution(coherence, disorder, deed.Readings, deed.Restoration);
        }
    }
}
