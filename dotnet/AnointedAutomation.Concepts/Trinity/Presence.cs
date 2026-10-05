// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-10-03 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// The presence of the Holy Spirit: the Person who applies, indwells, and completes a work. "And I
    /// will ask the Father, and he will give you another advocate to help you and be with you forever,
    /// the Spirit of truth ... he lives with you and will be in you." (John 14:16-17); "the Holy
    /// Spirit, whom the Father will send in my name, will teach you all things" (John 14:26).
    ///
    /// <para>
    /// In the triadic rule of <see cref="Reality.Witness(Act, Grounding, Presence)"/> the Spirit is
    /// the third factor: a deed grounded in the Father and conformed to the Word is still not brought
    /// to completion without the Spirit's empowering ("not by might nor by power, but by my Spirit",
    /// Zechariah 4:6). Presence is binary on purpose; the Spirit is present or quenched
    /// (1 Thessalonians 5:19), never partly a Person.
    /// </para>
    /// </summary>
    public sealed class Presence
    {
        private Presence(string name, bool isPresent, string scripture)
        {
            Name = name;
            IsPresent = isPresent;
            Scripture = scripture;
        }

        /// <summary>Gets the name of this presence.</summary>
        public string Name
        {
            get;
        }

        /// <summary>Gets whether the Holy Spirit is present to empower the work.</summary>
        public bool IsPresent
        {
            get;
        }

        /// <summary>Gets the Scripture this presence answers to.</summary>
        public string Scripture
        {
            get;
        }

        /// <summary>
        /// The Spirit's factor in the triadic rule: 1.0 when present, 0.0 when absent.
        /// </summary>
        public double Empowerment
        {
            get
            {
                if (IsPresent)
                {
                    return 1.0;
                }

                return 0.0;
            }
        }

        /// <summary>
        /// The Holy Spirit present, indwelling and empowering (John 14:16-17, 26).
        /// </summary>
        /// <returns>The presence of the Spirit.</returns>
        public static Presence HolySpirit()
        {
            return new Presence("the Holy Spirit", true, "John 14:16-17; John 14:26");
        }

        /// <summary>
        /// The Spirit absent or quenched. "Do not quench the Spirit." (1 Thessalonians 5:19); "apart
        /// from me you can do nothing." (John 15:5).
        /// </summary>
        /// <returns>An absent presence.</returns>
        public static Presence Absent()
        {
            return new Presence("no presence", false, "1 Thessalonians 5:19; John 15:5");
        }

        /// <summary>
        /// Completes a deed in the Spirit: its coherence is multiplied by <see cref="Empowerment"/>, so a
        /// deed without the Spirit keeps its disorder and restoration but coheres not at all.
        /// </summary>
        /// <param name="deed">The deed as grounded and conformed so far.</param>
        /// <returns>The deed as the Spirit completes it.</returns>
        public Resolution Complete(Resolution deed)
        {
            if (deed == null)
            {
                throw new System.ArgumentNullException(nameof(deed));
            }

            return new Resolution(deed.Coherence * Empowerment, deed.Disorder, deed.Readings, deed.Restoration);
        }
    }
}
