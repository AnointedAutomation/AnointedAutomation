// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// The finer grouping a catalogued sin belongs to. Coarser than a single sin, finer than the
    /// moderation set in <c>AnointedAutomation.Enums.Sin</c>. 0 is reserved for <see cref="Unknown"/>.
    /// </summary>
    public enum SinCategory : uint
    {
        /// <summary>Not determined (only the omitted placeholder entry uses it).</summary>
        Unknown = 0,

        /// <summary>Against God directly: unbelief, rebellion, irreverence, prayerlessness (Matthew 22:37).</summary>
        AgainstGod = 1,

        /// <summary>Idolatry, false worship and the occult (Exodus 20:3-5; Deuteronomy 18:10-12).</summary>
        FalseWorshipAndOccult = 2,

        /// <summary>Against Scripture and sound doctrine: twisting, adding to, or ignoring the Word (Revelation 22:18-19).</summary>
        ScriptureAndDoctrine = 3,

        /// <summary>Sins of the tongue (James 3:5-8).</summary>
        Speech = 4,

        /// <summary>Against a neighbour in general (Leviticus 19:18).</summary>
        AgainstOthers = 5,

        /// <summary>Anger, hatred and violence (Matthew 5:21-22).</summary>
        AngerAndViolence = 6,

        /// <summary>Sexual immorality and lust (1 Corinthians 6:18).</summary>
        Sexual = 7,

        /// <summary>Greed and the love of money (1 Timothy 6:10).</summary>
        GreedAndMoney = 8,

        /// <summary>Lying, fraud and theft (Exodus 20:15-16).</summary>
        DishonestyAndTheft = 9,

        /// <summary>Pride and self-exaltation (Proverbs 16:18).</summary>
        Pride = 10,

        /// <summary>Sins of the heart and mind: evil thoughts, envy, fear, worry (Mark 7:21-22).</summary>
        HeartAndMind = 11,

        /// <summary>Excess and neglect of the body: drunkenness, gluttony, sloth (Galatians 5:21).</summary>
        BodyAndExcess = 12,

        /// <summary>Within family and marriage (Ephesians 5:22-6:4).</summary>
        FamilyAndMarriage = 13,

        /// <summary>Toward rulers, employers, elders and law (Romans 13:1-7).</summary>
        AuthorityAndWork = 14,

        /// <summary>Good left undone (James 4:17).</summary>
        Omission = 15,

        /// <summary>Love of the world and the flesh (1 John 2:15-17).</summary>
        Worldliness = 16,

        /// <summary>Failures of preachers, teachers and shepherds (Ezekiel 34:2-4).</summary>
        Leadership = 17,

        /// <summary>Against fellow believers, God's children and servants (Matthew 18:6).</summary>
        AgainstBelievers = 18,
    }
}
