// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    /// <summary>
    /// The one record where state and truth are the same thing: the books from which every deed is
    /// judged. "The dead were judged ... as recorded in the books." (Revelation 20:12); "A scroll of
    /// remembrance was written in his presence." (Malachi 3:16); "The court was seated, and the books
    /// were opened." (Daniel 7:10); "All the days ordained for me were written in your book."
    /// (Psalm 139:16). It holds the earlier and the later together. Every deed reality witnesses is
    /// written here, and the standing <see cref="Coherence"/> of the world is read off the whole
    /// record.
    /// </summary>
    public class HeavenlyTablets
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeavenlyTablets"/> class. A new record holds
        /// no deeds yet, so reality stands fully ordered.
        /// </summary>
        public HeavenlyTablets()
        {
        }

        private readonly System.Collections.Generic.List<Resolution> record =
            new System.Collections.Generic.List<Resolution>();

        /// <summary>
        /// Writes a witnessed deed onto the record. What is recorded is permanent; the tablets hold
        /// "the earlier and the later" (Psalm 139:16).
        /// </summary>
        /// <param name="resolution">The resolution of a deed reality has witnessed.</param>
        public void Record(Resolution resolution)
        {
            if (resolution == null)
            {
                throw new System.ArgumentNullException(nameof(resolution));
            }

            record.Add(resolution);
        }

        /// <summary>
        /// The whole record of deeds witnessed so far, in the order they happened.
        /// </summary>
        /// <returns>A read-only view of every recorded resolution.</returns>
        public System.Collections.Generic.IReadOnlyList<Resolution> History()
        {
            return record;
        }

        /// <summary>
        /// The standing coherence of all reality recorded so far, from 0.0 (wholly disordered) to
        /// 1.0 (fully ordered). A new record reads 1.0; disorder introduced by witnessed deeds
        /// degrades it (Romans 8:20-22, where in the days of sinners the courses of the heavens transgress
        /// their order).
        /// </summary>
        /// <returns>The standing coherence of reality.</returns>
        public double Coherence()
        {
            double coherence = 1.0;
            foreach (Resolution resolution in record)
            {
                coherence = coherence * (1.0 - resolution.Disorder);
            }

            return coherence;
        }
    }
}
