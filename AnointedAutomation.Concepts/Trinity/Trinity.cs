// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-10-03 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// One God in three Persons: the Father, the Son (the Word), and the Holy Spirit. "We believe in
    /// one God, the Father almighty ... and in one Lord, Jesus Christ, the only Son of God, eternally
    /// begotten of the Father ... of one Being with the Father ... and in the Holy Spirit, the Lord,
    /// the giver of life, who proceeds from the Father." (Nicene Creed); "baptizing them in the name
    /// of the Father and of the Son and of the Holy Spirit" (Matthew 28:19, one name, three Persons).
    ///
    /// <para>
    /// The model keeps both halves of the confession. There is exactly one <see cref="Essence"/> (the
    /// whole <see cref="DivineCharacter"/>), and each <see cref="DivinePerson"/> holds that same
    /// instance, so the Persons are one God, not three gods. The Persons are three distinct objects,
    /// told apart only by their relations of origin, so they are not three masks of one actor.
    /// </para>
    ///
    /// <para>
    /// The Persons map onto the existing reality model rather than duplicating it: the Father is the
    /// ground (<see cref="Grounding.InGod"/>), the Son is the <see cref="Word"/> through which every
    /// deed meets reality (John 1:1-3), and the Holy Spirit is the <see cref="Presence"/> that applies
    /// and completes the work (John 14:16-17, 26). Their works toward creation are undivided: a work
    /// is always the work of all three, so <see cref="WorkThrough(DivinePersonKind, Act)"/> gives the
    /// same reading whichever Person it is appropriated to.
    /// </para>
    ///
    /// <para>
    /// Love requires three: a lover, a beloved, and the love between them (Augustine, De Trinitate
    /// VIII-IX), and perfect love shares its delight with a third (Richard of St. Victor, De Trinitate
    /// III, condilectio). "God is love" (1 John 4:8, 16) is therefore eternally true of God before
    /// anything was made. See <see cref="Bond"/>.
    /// </para>
    /// </summary>
    public sealed class Trinity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Trinity"/> class over a given reality, whose
        /// character is the one divine essence.
        /// </summary>
        /// <param name="reality">The reality the Triune God grounds, mediates, and fills.</param>
        public Trinity(Reality reality)
        {
            if (reality == null)
            {
                throw new System.ArgumentNullException(nameof(reality));
            }

            Reality = reality;
            Essence = reality.Character;
            Ground = Grounding.InGod();
            Word = new Word(reality);
            Presence = Presence.HolySpirit();

            Father = new DivinePerson(
                DivinePersonKind.Father,
                "the Father",
                "unbegotten, the Father almighty, maker of heaven and earth",
                "grounds: the source of all being, in whom reality is grounded",
                "Matthew 28:19; 1 Corinthians 8:6",
                Essence);
            Son = new DivinePerson(
                DivinePersonKind.Son,
                "the Son, the Word",
                "eternally begotten of the Father, of one Being with the Father",
                "conforms: all things were made through the Word, and every deed meets reality through Him",
                "John 1:1-3; John 1:14; Matthew 28:19",
                Essence);
            HolySpirit = new DivinePerson(
                DivinePersonKind.HolySpirit,
                "the Holy Spirit",
                "proceeds from the Father, with the Father and the Son worshiped and glorified",
                "empowers: the presence who applies, indwells, and completes the work",
                "John 14:16-17; John 14:26; John 15:26; Matthew 28:19",
                Essence);
            Persons = new[] { Father, Son, HolySpirit };
        }

        /// <summary>Gets the reality the Triune God grounds.</summary>
        public Reality Reality
        {
            get;
        }

        /// <summary>Gets the one divine essence, shared whole by every Person.</summary>
        public DivineCharacter Essence
        {
            get;
        }

        /// <summary>Gets the ground appropriated to the Father: the living God.</summary>
        public Grounding Ground
        {
            get;
        }

        /// <summary>Gets the Word, the medium appropriated to the Son.</summary>
        public Word Word
        {
            get;
        }

        /// <summary>Gets the presence appropriated to the Holy Spirit.</summary>
        public Presence Presence
        {
            get;
        }

        /// <summary>Gets the Father.</summary>
        public DivinePerson Father
        {
            get;
        }

        /// <summary>Gets the Son, the Word.</summary>
        public DivinePerson Son
        {
            get;
        }

        /// <summary>Gets the Holy Spirit.</summary>
        public DivinePerson HolySpirit
        {
            get;
        }

        /// <summary>Gets the three Persons in the order of Matthew 28:19.</summary>
        public System.Collections.Generic.IReadOnlyList<DivinePerson> Persons
        {
            get;
        }

        /// <summary>
        /// Whether the three Persons are one God: every Person holds the very same essence instance.
        /// </summary>
        /// <returns><c>true</c> when the essence is one.</returns>
        public bool IsOneEssence()
        {
            return Father.IsSameEssenceAs(Son) && Son.IsSameEssenceAs(HolySpirit);
        }

        /// <summary>
        /// Whether the three Persons are truly distinct: no Person is another Person, and none is
        /// an undetermined kind.
        /// </summary>
        /// <returns><c>true</c> when there are three distinct Persons.</returns>
        public bool ArePersonsDistinct()
        {
            foreach (DivinePerson person in Persons)
            {
                if (person.Kind == DivinePersonKind.Unknown)
                {
                    return false;
                }
            }

            return !Father.IsSamePersonAs(Son)
                && !Son.IsSamePersonAs(HolySpirit)
                && !Father.IsSamePersonAs(HolySpirit);
        }

        /// <summary>
        /// Looks up a Person by kind.
        /// </summary>
        /// <param name="kind">Which Person.</param>
        /// <returns>That Person.</returns>
        public DivinePerson PersonOf(DivinePersonKind kind)
        {
            foreach (DivinePerson person in Persons)
            {
                if (person.Kind == kind)
                {
                    return person;
                }
            }

            throw new System.ArgumentException("There is no Person of the Godhead of kind " + kind + ".", nameof(kind));
        }

        /// <summary>
        /// The undivided work of the Triune God on a deed: grounded in the Father, spoken through the
        /// Word, completed by the Spirit, witnessed and recorded by reality.
        /// </summary>
        /// <param name="act">The deed.</param>
        /// <returns>The resolution of the one work.</returns>
        public Resolution Work(Act act)
        {
            return Word.Speak(act, Ground, Presence);
        }

        /// <summary>
        /// A work appropriated to one Person. Because works toward creation are undivided, the reading
        /// is exactly that of <see cref="Work(Act)"/> whichever Person is named.
        /// </summary>
        /// <param name="kind">The Person the work is appropriated to.</param>
        /// <param name="act">The deed.</param>
        /// <returns>The resolution of the one work.</returns>
        public Resolution WorkThrough(DivinePersonKind kind, Act act)
        {
            PersonOf(kind);
            return Work(act);
        }

        /// <summary>
        /// The Triune God over reality as revealed (<see cref="Reality.Revealed"/>).
        /// </summary>
        /// <returns>A <see cref="Trinity"/> over the revealed reality.</returns>
        public static Trinity Revealed()
        {
            return new Trinity(Reality.Revealed());
        }
    }
}
