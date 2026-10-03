// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-10-03 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// The three Persons of the one God, "in the name of the Father and of the Son and of the Holy
    /// Spirit" (Matthew 28:19). Zero is reserved for an undetermined value, never a Person.
    /// </summary>
    public enum DivinePersonKind
    {
        /// <summary>Not determined. Never a Person of the Godhead.</summary>
        Unknown = 0,

        /// <summary>The Father, unbegotten, the ground of all being.</summary>
        Father = 1,

        /// <summary>The Son, the Word, eternally begotten of the Father (John 1:1-3).</summary>
        Son = 2,

        /// <summary>The Holy Spirit, who proceeds from the Father (John 15:26).</summary>
        HolySpirit = 3
    }
}
