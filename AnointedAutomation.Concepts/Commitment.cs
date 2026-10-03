// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-10-03 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// The state of the will toward a love. Love is chosen: "I have set before you life and death,
    /// blessings and curses. Now choose life" (Deuteronomy 30:19); "choose for yourselves this day
    /// whom you will serve" (Joshua 24:15). The gate is binary: a love is committed or it is not.
    /// </summary>
    public enum Commitment
    {
        /// <summary>No choice has been made yet. An undecided love is not committed and cannot act.</summary>
        Unknown = 0,

        /// <summary>The will has chosen this love. The gate is open (1).</summary>
        Committed = 1,

        /// <summary>The will has withdrawn from this love. The gate is closed (0).</summary>
        Withdrawn = 2
    }
}
