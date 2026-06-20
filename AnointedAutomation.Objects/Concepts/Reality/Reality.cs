// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    /// <summary>
    /// The Universe: the one thing agents address. Reality is grounded in God; it does not contain
    /// the grounding. The Father (the ground of being) is never an object here, He is what reality is
    /// grounded in and pervaded by, presupposed by there being any runnable world at all
    /// (Colossians 1:17, "in him all things hold together"; Hebrews 1:3, "sustaining all things by
    /// his powerful word"). We act on reality; we never construct God.
    ///
    /// <para>
    /// To witness a situation is, in one act, both to judge its truth and to record it: state and
    /// truth are one system. The deed is harmonized under God's whole <see cref="DivineCharacter"/>
    /// and written onto the <see cref="HeavenlyTablets"/>.
    /// </para>
    /// </summary>
    public class Reality
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Reality"/> class grounded in a given
        /// character of God, upon fresh, fully ordered tablets.
        /// </summary>
        /// <param name="character">The whole character of God reality is grounded in.</param>
        public Reality(DivineCharacter character)
        {
            if (character == null)
            {
                throw new System.ArgumentNullException(nameof(character));
            }

            this.character = character;
            Tablets = new HeavenlyTablets();
        }

        private readonly DivineCharacter character;

        /// <summary>
        /// The one record of all that reality has witnessed, and the source of its standing order
        /// (Revelation 20:12; Malachi 3:16).
        /// </summary>
        public HeavenlyTablets Tablets
        {
            get;
        }

        /// <summary>
        /// Witnesses a situation: harmonizes it under the whole of God's character, writes it onto
        /// the tablets, and returns the resolution. The same act produces both the truth of the deed
        /// and the change to reality's record, because they are one system.
        /// </summary>
        /// <param name="act">The deed reality witnesses.</param>
        /// <returns>The harmonized <see cref="Resolution"/>.</returns>
        public Resolution Witness(Act act)
        {
            return Witness(act, Grounding.InGod());
        }

        /// <summary>
        /// Witnesses a situation as done by an agent standing on a given foundation. The deed is
        /// harmonized under God's character, then borne on its <see cref="Grounding"/> (a deed on the
        /// living God keeps its life; a deed on an idol drifts toward non-being, 1 Meqabyan), and the
        /// borne deed is what is written onto the tablets.
        /// </summary>
        /// <param name="act">The deed reality witnesses.</param>
        /// <param name="grounding">What the acting agent is grounded in.</param>
        /// <returns>The harmonized resolution as its foundation can sustain it.</returns>
        public Resolution Witness(Act act, Grounding grounding)
        {
            if (act == null)
            {
                throw new System.ArgumentNullException(nameof(act));
            }

            if (grounding == null)
            {
                throw new System.ArgumentNullException(nameof(grounding));
            }

            Resolution resolution = character.Harmonize(act);
            Resolution borne = grounding.Bear(resolution);
            Tablets.Record(borne);
            return borne;
        }

        /// <summary>
        /// Reveals reality already grounded in God's character. "In the beginning was the Word ...
        /// all things were made through him." (John 1:1-3). The standard facets are all live at once.
        /// </summary>
        /// <returns>A <see cref="Reality"/> grounded in the revealed character of God.</returns>
        public static Reality Revealed()
        {
            return new Reality(new DivineCharacter(
                new LoveFacet(), new Justice(), new Mercy(), new Faithfulness()));
        }
    }
}
