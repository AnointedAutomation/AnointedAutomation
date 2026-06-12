// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    /// <summary>
    /// Love as a facet of God's character: agape, "God is love" (1 John 4:8) read as embodiment, not
    /// equation. This facet reuses the existing <see cref="Love"/> model of 1 Corinthians 13:4-8 and
    /// the sacrificial love of John 15:13; the backing <see cref="Agape"/> carries that full
    /// character. Which acts embody or offend Love is declared by the moral concepts themselves. It
    /// wraps the existing <see cref="Love"/> hierarchy rather than replacing it, so all prior love
    /// behavior is intact.
    /// </summary>
    public class LoveFacet : DivineAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoveFacet"/> class backed by perfect agape.
        /// </summary>
        public LoveFacet() : this(Concepts.Love.Agape())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoveFacet"/> class backed by a given love.
        /// God's love is agape; a lesser love is a lesser facet.
        /// </summary>
        /// <param name="agape">The love whose 1 Corinthians 13 character this facet expresses.</param>
        public LoveFacet(Love agape) : base("Love")
        {
            if (agape == null)
            {
                throw new System.ArgumentNullException(nameof(agape));
            }

            Agape = agape;
        }

        /// <summary>
        /// The love whose character this facet expresses (perfect agape for God's own love).
        /// </summary>
        public Love Agape
        {
            get;
        }
    }
}
